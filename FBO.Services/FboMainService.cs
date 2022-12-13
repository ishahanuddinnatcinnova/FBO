using FBO.Dapper;
using FBO.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GlobalAir.Data;
using Microsoft.AspNetCore.Http;

namespace FBO.Services
{
    public class FBOMainService
    {
        private static Dapperr _dapper;
        private static GeneralService _generalService;
        private static UtilitiesService _utility;
        public String fuel_status = "";
        public FBOMainService(Dapperr dapper, GeneralService generalService, UtilitiesService utilities)
        {
            _dapper = dapper;
            _generalService = generalService;
            _utility = utilities;
        }
        public async Task<ServiceResponseViewModel> GetResponse(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);
                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        List<FBOManagement_GetFBOs_Result> fbos = await _generalService.GetFBOs(userData.userID);
                        response.data.FBOs = fbos;

                        if (fbos.Count > 1)
                        {
                            response.isRedirect = false;
                        }
                        else
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/home/companymanage?companyid=" + userData.companyID;
                        }
                    }

                    //var fbo = await _fboMainService.CheckQuery(Convert.ToInt32(companyID), userData.userID, fuel);
                    //return View(fbo);
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }

        //public async Task<FBOResult> CheckQuery(int companyID, string userID, string fuel)
        //{
        //    FBOResult fboResultMainModel = new FBOResult();
        //    fboResultMainModel= await _generalService.GetDates(fboResultMainModel);
        //    fboResultMainModel.fboStats = await _generalService.GetFBOs_Totals(companyID, fboResultMainModel.startDate, fboResultMainModel.endDate);
        //    if (fuel != null && fuel.ToString() != "")
        //    {
        //        fuel_status = fuel.ToString();
        //    fuel_status = fuel_status.ToLower().Trim();
        //    if (fuel_status == "current")
        //    {
        //        _generalService.UpdateLastUpdated(companyID, fuel);
        //    }
        //    }
        //    if (companyID != 0 && companyID.ToString() != "")
        //    {

        //        fboResultMainModel = await _generalService.GetFBO(companyID, userID,fboResultMainModel);
        //        return fboResultMainModel;
        //    }
        //    else
        //    {
        //        //fboResultMainModel.FBOs = await _generalService.GetFBOs(userID);
        //        return fboResultMainModel;
        //    }


        //}




    }
}
