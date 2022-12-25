using FBO.Models;
using FBO.Services;
using FBO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace FBO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UtilitiesService _utility;
        private readonly FBOMainService _fboMainService;

        public HomeController(ILogger<HomeController> logger, UtilitiesService utility, FBOMainService fboMainService)
        {
            _logger = logger;
            _utility = utility;
            _fboMainService = fboMainService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("companymanage.aspx")]
        public async Task<IActionResult> CompanyManage(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponse(this.Request, companyID, fuel);
            if(response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                return View(response.data);
            }
        }

        [Route("extended.aspx")]
        public async Task<IActionResult> BasicAndExtended(string companyID, string fuel)
        {
            
            ServiceResponseViewModel response = await _fboMainService.GetResponseForServices(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                return View(response.data);
            }
        }
        
        [Route("fuelcards.aspx")]
        public async Task<IActionResult> FuelCards(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForFuelCardsSelected(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                return View(response.data);
            }
        }
        [Route("fuel.aspx")]
        public async Task<IActionResult> Fuel (string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForFuelPrice(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                return View(response.data);
            }
        }
        [Route("logoservices.aspx")]
        public async Task<IActionResult> LogoServices(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForLogoService(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                return View(response.data);
            }
        }
        [Route("information.aspx")]
        public async Task<IActionResult> FboInformation(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForFboInformation(this.Request, companyID, fuel);
            if (response.isRedirect)


            {
                return Redirect(response.redirectURL);
            }
            
            else
            {
                return View(response.data);
            }
        }
        [HttpPost]
        public  IActionResult BasicServiceUpdate(FBOManagement_UpdateBasicServices_Result updatebasic)
        {
            
            string response =  _fboMainService.PostBasicServicesUpdate(updatebasic);
            if(response== "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("BasicAndExtended", new {companyID = updatebasic.companyID });
        }

        [HttpPost]
        public  IActionResult ExtendedServiceUpdate(FBOManagement_UpdateExtendedServices_Result updateextended)
        {

            string response =  _fboMainService.PostExtendedServicesUpdate(updateextended);
            if(response== "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            
            return RedirectToAction("BasicAndExtended", new {companyID = updateextended.companyID });
        }
        [HttpPost]
        public  IActionResult FuelCardUpdate(FuelCardDiscountsModel fuel)
        {

            string response =  _fboMainService.PostFuelCardUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FuelCards", new { companyID = fuel.companyID });
        }
        [HttpPost]
        public  IActionResult FuelPriceUpdate(FuelPriceUpdateModel fuel)
        {
          
            string response =  _fboMainService.PostFuelPriceUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("Fuel", new { companyID = fuel.companyID });
        }
        [HttpPost]
        public  IActionResult LogoServiceUpdate(FBOLogoServiceModel logo)
        {

            string response =  _fboMainService.PostLogoServicesUpdate(logo);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("LogoServices", new { companyID = logo.companyID });
        }
        public async Task<RatingStats> GetFBOStats (int companyID)
        {
            RatingStats stats = new RatingStats();
            return stats;
        }
        [HttpPost]
        public async Task<IActionResult> FboInformationUpdate(FBOInfoUpdateModel updatebasic)
        {

            string response = await _fboMainService.PostFboInfoUpdate(updatebasic,this.Request);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FboInformation", new { companyID = updatebasic.companyID });
        }


        [HttpPost]
        public IActionResult DeleteFboLogo(string companyID, string logo)
        {

            string response = _fboMainService.DeleteFboLogo(companyID,logo);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("FboInformation", new { companyID = companyID });
        }  public IActionResult DeleteManagerPic(string companyID, string managerpic)
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
            return RedirectToAction("FboInformation", new { companyID = companyID });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}