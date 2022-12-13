using Dapper;
using FBO.Dapper;
using FBO.ViewModels;
using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FBO.Services
{
    public class GeneralService
    {
        private static Dapperr _dapper;
        public GeneralService(Dapperr dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<FBOManagement_GetFBOs_Result>> GetFBOs(string userID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("userID", userID);
                var fbos = await Task.FromResult(_dapper.GetAll<FBOManagement_GetFBOs_Result>("FBOManagement_GetFBOs", dynamicParameters, commandType: CommandType.StoredProcedure));
                return fbos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FBOResult> GetFBO(string companyID)
        {
            FBOResult fboResultMainModel = new FBOResult();
            //Fbo Result
            fboResultMainModel.FBO = await FboResult(Convert.ToInt16(companyID));
            //Fbo Result formatting
            //fboResultMainModel.FBO.FAARepairCode = "<a href=\"/airport/apt.airport.aspx?aptcode=" + fboResultMainModel.FBO.FAARepairCode + "\" target=\"_blank\" rel=\"noreferrer\">" + fboResultMainModel.FBO.FAARepairCode + "</a>";
            if (fboResultMainModel.FBO.IsApproved == true)
            {
                fboResultMainModel.fboIsApproved = "Yes";
            }
            else if (fboResultMainModel.FBO.IsApproved == false)
            {
                fboResultMainModel.fboIsApproved = "No";
            }
            fboResultMainModel.companyName = fboResultMainModel.FBO.Company;

            fboResultMainModel.companyfullAddress = fboResultMainModel.FBO.Company + "<br />" + fboResultMainModel.FBO.City + ", " + fboResultMainModel.FBO.State + " " + fboResultMainModel.FBO.Zip;
            //Check Expiry
            fboResultMainModel.fboIsExpired = CheckFboExpired(fboResultMainModel);
            //Check Review Count
            fboResultMainModel.reviews_of_ratings = await CheckReviewsCount(Convert.ToInt16(companyID));
            //Get Fuel Averages
            fboResultMainModel.averageprices = await GetFuelAverages(Convert.ToInt16(companyID));
            //Get Average Fuel Price
            fboResultMainModel.averageFuelPrice = Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_JETA), 2) + Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_100LL), 2);
            await GetDates(fboResultMainModel);
            fboResultMainModel.fboStats = await GetFBOs_Totals(Convert.ToInt16(companyID), fboResultMainModel.startDate, fboResultMainModel.endDate);

            return fboResultMainModel;
        }
        public async Task<FBOManagement_GetFBO_Result> FboResult(int companyID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                var fbo = await Task.FromResult(_dapper.Get<FBOManagement_GetFBO_Result>("FBOManagement_GetFBO", dynamicParameters, commandType: CommandType.StoredProcedure));
                if (fbo.CompanyID != 0)
                {
                    return fbo;

                }
                return fbo;
            }
            catch (Exception ex)
            {
                return null;
            }




        }
        public bool CheckFboExpired(FBOResult fbo)
        {
            //  FboResultMainModel fbo = new FboResultMainModel();
            DateTime lastUpdated;

            lastUpdated = Convert.ToDateTime(fbo.FBO.LastUpdated);
            DateTime currentDate = DateTime.Now;
            if ((currentDate - lastUpdated).TotalDays > 30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<int> CheckReviewsCount(int companyID)

        {

            FBOResult fbo = new FBOResult();
            FBOManagement_GetFBO_Result fboResult = new FBOManagement_GetFBO_Result();
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("companyID", companyID);
                int reviewcount = await Task.FromResult(_dapper.Get<int>("select count(*) from reviews_of_ratings r inner join services_ratings s on r.ratingID = s.ratingsID where s.FBOID_FK = " + companyID + " and IsSeen = 0", dynamicParameters, commandType: CommandType.Text));

                return reviewcount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
   

        public async Task<FBOManagement_GetRegionAverages_Result> GetFuelAverages(int companyID)
        {
            FBOManagement_GetRegionAverages_Result fuelaverages = new FBOManagement_GetRegionAverages_Result();
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("companyID", companyID);
                var fuelavg = await Task.FromResult(_dapper.Get<FBOManagement_GetRegionAverages_Result>("FBOManagement_GetRegionAverages", dynamicParameters, commandType: CommandType.StoredProcedure));
                if (fuelavg.CompanyID != 0)
                {
                    fuelaverages.Average_JETA = fuelavg.Average_JETA;
                    fuelaverages.Average_100LL = fuelavg.Average_100LL;
                    fuelaverages.FAARegionCode = fuelavg.FAARegionCode;
                    fuelaverages.RegionName = fuelavg.RegionName;

                }

                return fuelaverages;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Task<FBOResult> GetDates(FBOResult fbo)
        {
            DateTime dateDefault = System.DateTime.Now;
            DateTime dateToday = System.DateTime.Now;

            dateDefault = dateToday.AddMonths(-6); //Subtract 6 months from today's date

            if (fbo.startDate == null)
            {
                fbo.startDate = dateDefault.ToString("MM/dd/yyyy");
            }
            if (fbo.endDate == null)
            {
                fbo.endDate = dateToday.ToString("MM/dd/yyyy");
            }
            return Task.FromResult(fbo);

        }
        public async Task<FBOManagement_Stats_Result> GetFBOs_Totals(int CompanyID, String date_start, String date_end)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", CompanyID);
                dynamicParameters.Add("startdate", date_start);
                dynamicParameters.Add("enddate", date_end);
                var fbos = await Task.FromResult(_dapper.Get<FBOManagement_Stats_Result>("FBOManagement_Stats", dynamicParameters, commandType: CommandType.StoredProcedure));

                return fbos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public  void UpdateLastUpdated(int CompanyID,string fuel)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", CompanyID);
                _dapper.Execute("FBOManagement_LastUpdated", dynamicParameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

            }
        }
    }
}