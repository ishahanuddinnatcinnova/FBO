using Dapper;
using FBO.ActionFilters;
using FBO.Dapper;
using FBO.Services;
using FBO.ViewModels;
using GlobalAir.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using System.ComponentModel.Design;
using System.Data;
using System.Web.Helpers;

namespace FBO.Controllers
{
    public class ServicePhotoController : Controller
    {
        private static Dapperr _dapper;
        private static UtilitiesService _utility;
        private static GeneralService _generalService;
        private readonly AppSettings _appSettings;
        private readonly ServicePhotoService _servicePhotoService;
        public ServicePhotoController(Dapperr dapper, GeneralService generalService, UtilitiesService utilities, AppSettings appSettings, ServicePhotoService servicePhotoService)
        {
            _dapper = dapper;
            _utility = utilities;
            _generalService = generalService;
            _appSettings = appSettings;
            _servicePhotoService = servicePhotoService;
        }
        // GET: ImageManagement/5
        [Route("ServicePhoto/index")]

        public async Task<ActionResult> Index(int companyId)
        {
            TempData["page"] = "servicephoto";

            var cookie = Request.Cookies["GlobalAir"];
            if (cookie == null) return Redirect("/myflightdept/account.aspx");

            // Authenticate
            if (!Authenticate(companyId, int.Parse(cookie)))
                return Redirect("/myflightdept/account.aspx");

            FBOResult res = await _generalService.GetFBO(companyId.ToString());

            // Create token for API calls
            var token = Token.Generate("", "");
            var imageList = _servicePhotoService.Get(companyId);

            if (imageList.Count > 0)
            {
                ViewBag.FBOURL = UtilitiesService.FormatARC_FBO(companyId + "", imageList[0].Company, imageList[0].APTCode);
            }

            ViewBag.CompanyId = companyId;
            ViewBag.AppID = token.AppId;
            ViewBag.Token = token.AuthToken;

            // Breadcrumb
            //var bclst = PersistentBreadCrumbList.BuildBreadCrumbLst("ServicePhoto", "Index");
            //bclst.MatchTag("[COMPANYID]", companyId.ToString());
            //ViewBag.BreadCrumb = bclst.BuildBreadCrumbHtml();

            ServiceResponseViewModel response = new ServiceResponseViewModel();
            response.data.FBO = res;
            response.data.Photos = imageList.ToList();

            return View("ServicePhoto", response);
        }

        [AuthorizationRequired]
        // POST: api/Image
        public async Task<JObject> Create(int companyId, int appId, string tokenId)
        {
            string result = "{{initialPreview: [{0}], initialPreviewConfig: [{1}]}}";
            string initialPreviewTemplate = "'{0}'";
            string initialPreviewConfigTemplate = "{{url: \"/fbo/ServicePhoto/Delete/{0}?appId=" + appId + "&tokenId=" + tokenId + "\", key: {1}, caption: \"{2}\" }}";
            string initialPreview = "";
            string initialPreviewConfig = "";

            for (int i = 0; i < HttpContext.Request.Form.Files.Count; i++)
            {
                IFormFile file = HttpContext.Request.Form.Files[i];
                byte[] myData1 = await _servicePhotoService.GetBytes(file);

                try
                {
                    var photo = _servicePhotoService.Create(myData1, Path.GetFileName(file.FileName), companyId);

                    if (initialPreview != "") initialPreview += ",";
                    initialPreview += string.Format(initialPreviewTemplate, photo.Photo);

                    if (initialPreviewConfig != "") initialPreviewConfig += ",";
                    initialPreviewConfig += string.Format(initialPreviewConfigTemplate, photo.PhotoID, photo.PhotoID, Path.GetFileName(photo.Photo));
                }
                catch (Exception ex)
                {
                    Log.Information("Service Photo Controller, error in Create. Exception Message: " + ex.Message + ". Exception: " + ex);

                    return JObject.Parse("{error: '" + ex.Message.Replace("'", "") + "'}");
                }
            }

            result = string.Format(result, initialPreview, initialPreviewConfig);
            return JObject.Parse(result);
        }

        [AuthorizationRequired]
        [HttpPost]
        // DELETE: api/Image/5
        public JObject Delete(int id)
        {
            try
            {
                _servicePhotoService.Delete(id);
                return JObject.Parse("{}");
            }
            catch (Exception ex)
            {
                return JObject.Parse("{error: '" + ex.Message.Replace("'", "") + "'}");
            }
        }

        private bool Authenticate(int companyId, int userId)
        {
            try
            {
                //var user = GetUser(mfd_userID);
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("UserId", userId);
                List<membership_IsUserFBO_Result> users = _dapper.GetAll<membership_IsUserFBO_Result>("dbo.membership_IsUserFBO", dynamicParameters, commandType: CommandType.StoredProcedure);
                if ((users != null))
                {
                    foreach (var user in users)
                    {
                        if (user.fboID == companyId.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
