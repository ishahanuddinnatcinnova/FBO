using FBO.Services;
using FBO.ViewModels;
using GlobalAir.Data;
using Microsoft.AspNetCore.Mvc;

namespace FBO.Controllers
{
    public class FBOHelperController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UtilitiesService _utility;
        private readonly FBOMainService _fboMainService;

        public FBOHelperController(ILogger<HomeController> logger, UtilitiesService utility, FBOMainService fboMainService)
        {
            _logger = logger;
            _utility = utility;
            _fboMainService = fboMainService;
        }
        [HttpPost]
        public IActionResult BasicServiceUpdate(FBOManagement_UpdateBasicServices_Result updatebasic)
        {

            string response = _fboMainService.PostBasicServicesUpdate(updatebasic);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("BasicAndExtended", "Home" , new { companyID = updatebasic.companyID });
        }

        [HttpPost]
        public IActionResult ExtendedServiceUpdate(FBOManagement_UpdateExtendedServices_Result updateextended)
        {

            string response = _fboMainService.PostExtendedServicesUpdate(updateextended);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }

            return RedirectToAction("BasicAndExtended","Home", new { companyID = updateextended.companyID });
        }

        [HttpPost]
        public IActionResult FuelCardUpdate(FuelCardDiscountsModel fuel)
        {

            string response = _fboMainService.PostFuelCardUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FuelCards","Home", new { companyID = fuel.companyID });
        }
        [HttpPost]
        public IActionResult FuelPriceUpdate(FuelPriceUpdateModel fuel)
        {

            string response = _fboMainService.PostFuelPriceUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("Fuel","Home", new { companyID = fuel.companyID });
        }
        [HttpPost]
        public IActionResult LogoServiceUpdate(FBOLogoServiceModel logo)
        {

            string response = _fboMainService.PostLogoServicesUpdate(logo);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("LogoServices","Home", new { companyID = logo.companyID });
        }

        [HttpPost]
        public async Task<IActionResult> FboInformationUpdate(FBOInfoUpdateModel updatebasic)
        {

            string response = await _fboMainService.PostFboInfoUpdate(updatebasic, this.Request);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
                TempData["errorMesssage"] = response;

            }

            return RedirectToAction("FboInformation","Home", new { companyID = updatebasic.companyID });
        }
        [HttpPost]
        public async Task<IActionResult> SaveUpdateCustomServices(FBOManagement_GetCustomServices_Result res)
        {

            string response = await _fboMainService.SaveUpdateCustomServices(res);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("CustomServices","Home" ,new { companyID = res.CompanyID });
        }

        [HttpPost]
        public IActionResult DeleteFboLogo(string companyID, string logo)
        {

            string response = _fboMainService.DeleteFboLogo(companyID, logo);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FboInformation","Home", new { companyID = companyID });
        }
        [HttpPost]
        public IActionResult DeleteManagerPic(string companyID, string managerpic)
        {

            string response = _fboMainService.DeleteManagerPic(companyID, managerpic);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FboInformation","Home", new { companyID = companyID });
        }
        [HttpPost]
        public async Task<IActionResult> FboUpgrade(FBOUpgradeModel updatebasic)
        {

            string response = await _fboMainService.PostFboUpgrade(updatebasic, this.Request);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else if(response == "failedcc")
            {
                TempData["failedcc"] = "true";
            }
            else if(response=="failedcvv")
            {
                TempData["failedcvv"] = "true";

            }
            else if(response== "faileddate")
            {
                TempData["faileddate"] = "true";
            }
            else
            {
                TempData["success"]= "false";
            }
            return RedirectToAction("Upgrade", "Home", new { companyID = updatebasic.companyID,level=updatebasic.upgradeto,step=2 });
        }
    }
}
