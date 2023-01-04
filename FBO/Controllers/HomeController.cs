using FBO.Models;
using FBO.Services;
using FBO.ViewModels;
using GlobalAir.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                TempData["page"] = "companymanage";
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
                TempData["page"] = "basicandextended";
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
                TempData["page"] = "fuelcards";
                return View(response.data);
            }
        }
        [Route("fuel.aspx")]
        public async Task<IActionResult> Fuel(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForFuelPrice(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                TempData["page"] = "fuel";
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
                TempData["page"] = "logoservices";
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
                TempData["page"] = "fboinformation";
                return View(response.data);
            }
        }
        [Route("custom.aspx")]
        public async Task<IActionResult> CustomServices(string companyID, string fuel)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForCustomServices(this.Request, companyID, fuel);
            if (response.isRedirect)
            {
                return Redirect(response.redirectURL);
            }
            else
            {
                TempData["page"] = "customservices";
                return View(response.data);
            }
        }
        [Route("upgrade.aspx")]
        public async Task<IActionResult> Upgrade(string companyID, string fuel, string level, string step)
        {
            ServiceResponseViewModel response = await _fboMainService.GetResponseForUpgradePage(this.Request, companyID, fuel);
            if (!response.isRedirect)
            {
                TempData["page"] = "upgrade";

                if (!String.IsNullOrEmpty(level) && !String.IsNullOrEmpty(step))
                    return View("Step", response.data);
                else
                    return View(response.data);
            }
            else
            {
                return Redirect(response.redirectURL);
            }
        }

        //public async Task<IActionResult> Step(ServiceResponseViewModel data)
        //{
        //    return View(data);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}