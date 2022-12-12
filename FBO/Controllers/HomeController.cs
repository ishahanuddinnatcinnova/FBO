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
        private readonly FboMainService _fboMainService;

        public HomeController(ILogger<HomeController> logger, UtilitiesService utility, FboMainService fboMainService)
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

        public async Task<IActionResult> CompanyManage(int companyID, string fuel)
        {
            
            LoginViewModel vm = _utility.CheckLogin(this.Request);

            var fbo = await _fboMainService.CheckQuery(companyID,vm.userID,fuel);

            if(vm.isUser)
            {

                return View(fbo);
            }
            else
            {
                return Redirect("/myflightdept/account.aspx");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}