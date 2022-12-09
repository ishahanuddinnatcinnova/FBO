using FBO.Dapper;
using FBO.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.Services
{
    public class FboMainService
    {
        private static Dapperr _dapper;
        private static GeneralService _generalService;
        public FboMainService(Dapperr dapper, GeneralService generalService)
        {
            _dapper = dapper;
            _generalService = generalService;
        }
        public async Task<FboResultMainModel> CheckQuery(int companyID, string userID, string fuel)
        {
            FboResultMainModel fboResultMainModel = new FboResultMainModel();
            if (companyID != 0 && companyID.ToString() != "")
            {

                fboResultMainModel = await getFBO(companyID, userID);
                return fboResultMainModel;
            }
            else
            {
                fboResultMainModel.FBOs = await _generalService.GetFbos(userID);
                return fboResultMainModel;
            }


        }

            //}
            public async Task<FboResultMainModel> getFBO(int companyID,string userID)
        {
            FboResultMainModel fboResultMainModel = new FboResultMainModel();
            //Fbo Result
            fboResultMainModel.FBO = await _generalService.FboResult(companyID);
            //Fbo Result formatting
            fboResultMainModel.FBO.Email = "<a href=\"mailto:" + fboResultMainModel.FBO.Email + "\">" + fboResultMainModel.FBO.Email + "</a>";
            fboResultMainModel.FBO.FAARepairCode = "<a href=\"/airport/apt.airport.aspx?aptcode=" + fboResultMainModel.FBO.FAARepairCode + "\" target=\"_blank\" rel=\"noreferrer\">" + fboResultMainModel.FBO.FAARepairCode + "</a>";
            if (fboResultMainModel.FBO.IsApproved == true)
            {
                fboResultMainModel.fboIsApproved = "Yes";
            }
            else if (fboResultMainModel.FBO.IsApproved == false)
            {
                fboResultMainModel.fboIsApproved = "No";
            }
            fboResultMainModel.companyName = fboResultMainModel.FBO.Company + " " + "(" + fboResultMainModel.FBO.FAARepairCode + ")";

            fboResultMainModel.companyfullAddress = fboResultMainModel.FBO.Company + "<br />" + fboResultMainModel.FBO.City + ", " + fboResultMainModel.FBO.State + " " + fboResultMainModel.FBO.Zip;
            //Check Expiry
            fboResultMainModel.fboIsExpired = _generalService.CheckFboExpired(fboResultMainModel);
            //Check Review Count
            fboResultMainModel.reviews_of_ratings =await _generalService.CheckReviewsCount(companyID);
            //Get Fuel Averages
            fboResultMainModel.averageprices = await _generalService.GetFuelAverages(companyID);
            //Get Average Fuel Price
            fboResultMainModel.averageFuelPrice = Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_JETA), 2) + Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_100LL), 2);
            // Get FBO Count
            fboResultMainModel.fbo_count = await _generalService.CountFbo(userID);
            return fboResultMainModel;
        }
    }
}
