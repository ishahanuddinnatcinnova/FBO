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

        public HomeController(ILogger<HomeController> logger, UtilitiesService utility)
        {
            _logger = logger;
            _utility = utility;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CompanyManage()
        {
            LoginViewModel vm = _utility.CheckLogin(this.Request);
            if(vm.isUser)
            {
                return View(vm);
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