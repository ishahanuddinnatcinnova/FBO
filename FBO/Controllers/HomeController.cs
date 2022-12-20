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

        [HttpPost]
        public async Task<IActionResult> BasicServiceUpdate(FBOManagement_UpdateBasicServices_Result updatebasic)
        {

            string response = await _fboMainService.PostBasicServicesUpdate(updatebasic);
            if(response== "Success")
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
        public async Task<IActionResult> ExtendedServiceUpdate(FBOManagement_UpdateExtendedServices_Result updateextended)
        {

            string response = await _fboMainService.PostExtendedServicesUpdate(updateextended);
            if(response== "Success")
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
        public async Task<IActionResult> FuelCardUpdate(FuelCardDiscountsModel fuel)
        {

            string response = await _fboMainService.PostFuelCardUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("fuelcards", new { companyID = fuel.companyID });
        }
        [HttpPost]
        public async Task<IActionResult> FuelPriceUpdate(FuelPriceUpdateModel fuel)
        {
          
            string response = await _fboMainService.PostFuelPriceUpdate(fuel);
            if (response == "success")
            {
                TempData["success"] = "true";
            }
            else
            {
                TempData["success"] = "false";
            }
            return RedirectToAction("fuel", new { companyID = fuel.companyID });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}