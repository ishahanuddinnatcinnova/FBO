﻿using FBO.Services;
using FBO.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using GlobalAir.Data;
using Newtonsoft.Json.Linq;

namespace FBO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FBOHelperAPI : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly GeneralService _generalService;
        private readonly FBOMainService _fboMainService;

        public FBOHelperAPI(ILogger<HomeController> logger, UtilitiesService utility, GeneralService generalService, FBOMainService fboMainService)
        {
            _logger = logger;
            _generalService = generalService;
        _fboMainService = fboMainService;

        }
        
        [Route("GetFBOStats")]
        public async Task<FBOManagement_Stats_Result> GetFBOStats(string companyID,string StartDate,string EndDate)
        {
            FBOManagement_Stats_Result stats = new FBOManagement_Stats_Result();
            stats = await _generalService.getFBOStats(Convert.ToInt16(companyID), StartDate, EndDate);
            return stats;
        }

        [Route("DeleteCustomService")]
        public async Task<string> DeleteCustomService(string serviceID)
        {
            
            string response = _fboMainService.DeleteCustomService(Convert.ToInt32(serviceID));
            return response;
     
        }
    }
}
