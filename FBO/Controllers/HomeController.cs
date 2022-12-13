using FBO.Models;
using FBO.Services;
using FBO.ViewModels;
using Microsoft.AspNetCore.Mvc;
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}