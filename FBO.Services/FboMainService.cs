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

namespace FBO.Services
{
    public class FboMainService
    {
        private static Dapperr _dapper;
        private static GeneralService _generalService;
        public String fuel_status = "";
        public FboMainService(Dapperr dapper, GeneralService generalService)
        {
            _dapper = dapper;
            _generalService = generalService;
        }
        public async Task<FboResultMainModel> CheckQuery(int companyID, string userID, string fuel)
        {
            FboResultMainModel fboResultMainModel = new FboResultMainModel();
            fboResultMainModel= await _generalService.GetDates(fboResultMainModel);
            fboResultMainModel.fboStats = await _generalService.GetFBOs_Totals(companyID, fboResultMainModel.startDate, fboResultMainModel.endDate);
            if (fuel != null && fuel.ToString() != "")
            {
                fuel_status = fuel.ToString();
            fuel_status = fuel_status.ToLower().Trim();
            if (fuel_status == "current")
            {
                _generalService.UpdateLastUpdated(companyID, fuel);
            }
            }
            if (companyID != 0 && companyID.ToString() != "")
            {

                fboResultMainModel = await _generalService.GetFBO(companyID, userID,fboResultMainModel);
                return fboResultMainModel;
            }
            else
            {
                fboResultMainModel.FBOs = await _generalService.GetFBOs(userID);
                return fboResultMainModel;
            }


        }




    }
}
