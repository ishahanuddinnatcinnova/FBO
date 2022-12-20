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
using static Dapper.SqlMapper;

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

            fboResultMainModel.isUpgradeEligible = await CheckUpgradeEligibleAsync(companyID);
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
            // FboResultMainModel fbo = new FboResultMainModel();
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


        public async Task<FBOAverageFuelPrices> GetFuelAverages(int companyID)
        {
            FBOAverageFuelPrices fuelaverages = new FBOAverageFuelPrices();
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("companyID", companyID);
                var fuelavg = await Task.FromResult(_dapper.Get<FBOAverageFuelPrices>("FBOManagement_GetRegionAverages", dynamicParameters, commandType: CommandType.StoredProcedure));
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
        public void UpdateLastUpdated(int CompanyID, string fuel)
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
        public async Task<string> SaveButtonBasicService(FBOManagement_UpdateBasicServices_Result basic)
        {
            int companyID = 0;
            companyID = Convert.ToInt32(basic.companyID);
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", basic.companyID);
                dynamicParameters.Add("IsCatering", basic.checkboxCatering ? 1 : 0);
                dynamicParameters.Add("IsHotel", basic.checkboxHotel ? 1 : 0);
                dynamicParameters.Add("IsCourtesy", basic.checkboxCourtesy ? 1 : 0);
                dynamicParameters.Add("Isweather", basic.checkboxWeather ? 1 : 0);
                dynamicParameters.Add("IsRepairs", basic.checkboxRepairs ? 1 : 0);
                dynamicParameters.Add("IsRentalCars", basic.checkboxRentalCars ? 1 : 0);
                _dapper.Execute("FBOManagement_UpdateBasicServices", dynamicParameters, commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                return "failed";
            }


        }
        public async Task<string> SaveButtonExtendedService(FBOManagement_UpdateExtendedServices_Result extended)
        {
            int companyID = 0;
            companyID = Convert.ToInt32(extended.companyID);
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("IsPilotLounge", extended.checkboxPilotLounge ? 1 : 0);
                dynamicParameters.Add("IsBroadBand", extended.checkboxBroadBand);
                dynamicParameters.Add("IsParking", extended.checkboxParking ? 1 : 0);
                dynamicParameters.Add("IsRestaurant", extended.checkboxRestaurant ? 1 : 0);
                dynamicParameters.Add("IsInternetAccess", extended.checkboxInternetAccess ? 1 : 0);
                dynamicParameters.Add("IsRestrooms", extended.checkboxRestrooms ? 1 : 0);
                dynamicParameters.Add("IsShowers", extended.checkboxShowers ? 1 : 0);
                dynamicParameters.Add("IsCrewCars", extended.checkboxCrewCars ? 1 : 0);
                dynamicParameters.Add("IsPublicTelephone", extended.checkboxPublicTelephone ? 1 : 0);
                dynamicParameters.Add("IsAircraftDetailing", extended.checkboxAircraftDetailing ? 1 : 0);
                dynamicParameters.Add("IsAircraftParts", extended.checkboxAircraftParts ? 1 : 0);
                dynamicParameters.Add("IsFlyingClub", extended.checkboxFlyingClub ? 1 : 0);
                dynamicParameters.Add("IsAircraftMods", extended.checkboxAircraftMods ? 1 : 0);
                dynamicParameters.Add("IsAircraftPainting", extended.checkboxAircraftPainting ? 1 : 0);
                dynamicParameters.Add("IsAircraftInterior", extended.checkboxAircraftInterior ? 1 : 0);
                dynamicParameters.Add("IsAirframeMain", extended.checkboxAirMaintenance ? 1 : 0);
                dynamicParameters.Add("IsPowerplantMain", extended.checkboxPowMaintenance ? 1 : 0);
                dynamicParameters.Add("IsAvionics", extended.checkboxAvionicsService ? 1 : 0);
                dynamicParameters.Add("IsPassTerminal", extended.checkboxPassengerTerminal ? 1 : 0);
                dynamicParameters.Add("IsAircraftRental", extended.checkboxAircraftRental ? 1 : 0);
                dynamicParameters.Add("IsCharters", extended.checkboxCharters ? 1 : 0);
                dynamicParameters.Add("IsOxygen", extended.checkboxOxygen ? 1 : 0);
                dynamicParameters.Add("IsHangars", extended.checkboxHangars ? 1 : 0);
                dynamicParameters.Add("IsTiedown", extended.checkboxTieDowns ? 1 : 0);
                dynamicParameters.Add("IsFlightInstr", extended.checkboxFlightInstruction ? 1 : 0);
                dynamicParameters.Add("IsLavService", extended.checkboxLavService ? 1 : 0);
                dynamicParameters.Add("IsQuickTurn", extended.checkboxQuickTurn ? 1 : 0);
                dynamicParameters.Add("IsDeicing", extended.checkboxDeIcing ? 1 : 0);
                dynamicParameters.Add("IsSnoozeRoom", extended.checkboxSnoozeRoom ? 1 : 0);
                dynamicParameters.Add("IsTelevision", extended.checkboxTelevision ? 1 : 0);
                dynamicParameters.Add("IsConferenceRoom", extended.checkboxConference ? 1 : 0);
                dynamicParameters.Add("IsVending", extended.checkboxVending ? 1 : 0);
                dynamicParameters.Add("IsFlightPlanning", extended.checkboxFlightPlanning ? 1 : 0);
                dynamicParameters.Add("IsBusinessCenter", extended.checkboxBusinessCenter ? 1 : 0);
                dynamicParameters.Add("IsFitnessCenter", extended.checkboxFitnessCenter ? 1 : 0);
                _dapper.Execute("FBOManagement_UpdateExtServices", dynamicParameters, commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                return "failed";

            }
        }

        public async Task<FuelCardDiscountsModel> GetFuelCards(string companyID)
        {
            FuelCardDiscountsModel fueldis = new FuelCardDiscountsModel();
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                var discountId = await Task.FromResult(_dapper.GetAll<string>("services_FuelDiscountServices_GetServices", dynamicParameters, commandType: CommandType.StoredProcedure));
                foreach (String logoID in discountId)
                {

                    if (logoID == "12")
                        fueldis.chkFuelCard12 = true;

                    else if (logoID == "44")
                        fueldis.chkFuelCard44 = true;

                    else if (logoID == "57")
                        fueldis.chkFuelCard57 = true;

                    else if (logoID == "61")
                        fueldis.chkFuelCard61 = true;

                    else if (logoID == "62")
                        fueldis.chkFuelCard62 = true;

                    else if (logoID == "63")
                        fueldis.chkFuelCard63 = true;

                    else if (logoID == "64")
                        fueldis.chkFuelCard64 = true;

                    else if (logoID == "65")
                        fueldis.chkFuelCard65 = true;

                    else if (logoID == "66")
                        fueldis.chkFuelCard66 = true;
                    else if (logoID == "68")
                        fueldis.chkFuelCard68 = true;

                    else if (logoID == "69")
                        fueldis.chkFuelCard69 = true;

                    else if (logoID == "70")
                        fueldis.chkFuelCard70 = true;

                    else if (logoID == "71")
                        fueldis.chkFuelCard71 = true;
                }
                return fueldis;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string BtnFuelCardSaveClick(FuelCardDiscountsModel fueldis)
        {
            var response = new List<string>();
            try
            {

                response.Add(ClearFuelCards(fueldis.companyID));
                if (fueldis.chkFuelCard12 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 12));

                if (fueldis.chkFuelCard44 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 44));

                if (fueldis.chkFuelCard57 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 57));

                if (fueldis.chkFuelCard61 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 61));

                if (fueldis.chkFuelCard62 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 62));

                if (fueldis.chkFuelCard63 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 63));

                if (fueldis.chkFuelCard64 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 64));

                if (fueldis.chkFuelCard65 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 65));

                if (fueldis.chkFuelCard66 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 66));

                if (fueldis.chkFuelCard68 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 68));

                if (fueldis.chkFuelCard69 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 69));

                if (fueldis.chkFuelCard70 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 70));

                if (fueldis.chkFuelCard71 == true)
                    response.Add(AddFuelCard(fueldis.companyID, 71));

                foreach (string res in response)
                {
                    if (res == "success")
                    {
                        return ("success");
                    }
                    else
                    {
                        return ("failed");
                    }
                }
                return ("success");
            }
            catch
            {
                return "failed";
            }
        }
        public string ClearFuelCards(string companyID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                _dapper.Execute("services_FuelCards_Clear", dynamicParameters, commandType: CommandType.StoredProcedure);
                return "success";
            }
            catch (Exception ex)
            {
                return "failed";
            }

        }
        public string AddFuelCard(string companyID, int discountid)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("DiscountID", discountid);
                dynamicParameters.Add("CompanyID", companyID);
                _dapper.Execute("services_FuelCards_Add", dynamicParameters, commandType: CommandType.StoredProcedure);
                return "success";
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }
        public string BtnFuelPriceSaveClick(FuelPriceUpdateModel fueldis)
        {
            string respose = "";

            try
            {
                String FsJETA = fueldis.Content_tbJETTA.Trim();
                String FsJETAPRIST = fueldis.Content_tbJETAPRIST.Trim();
                String Fs100LL = fueldis.Content_tb100LL.Trim();
                String FsUL94 = fueldis.Content_tbUL94.Trim();
                String FsMOGAS = fueldis.Content_tbMOGAS.Trim();
                String FsSAF = fueldis.Content_tbSAF.Trim();
                String FsSAFPRIST = fueldis.Content_tbSAFPRIST.Trim();

                String SsJETA = fueldis.Content_tbSSJETTA.Trim();
                String SsJETAPRIST = fueldis.Content_tbSSJETAPRIST.Trim();
                String Ss100LL = fueldis.Content_tbSS100LL.Trim();
                String SsUL94 = fueldis.Content_tbSSUL94.Trim();
                String SsMOGAS = fueldis.Content_tbSSMOGAS.Trim();
                String SsSAF = fueldis.Content_tbSSSAF.Trim();
                String SsSAFPRIST = fueldis.Content_tbSSSAFPRIST.Trim();

                if (FsJETA == "")
                {
                    FsJETA = "0";
                }
                if (FsJETAPRIST == "")
                {
                    FsJETAPRIST = "0";
                }
                if (Fs100LL == "")
                {
                    Fs100LL = "0";
                }
                if (FsUL94 == "")
                {
                    FsUL94 = "0";
                }
                if (FsMOGAS == "")
                {
                    FsMOGAS = "0";
                }
                if (FsSAF == "")
                {
                    FsSAF = "0";
                }
                if (FsSAFPRIST == "")
                {
                    FsSAFPRIST = "0";
                }

                if (SsJETA == "")
                {
                    SsJETA = "0";
                }
                if (SsJETAPRIST == "")
                {
                    SsJETAPRIST = "0";
                }
                if (Ss100LL == "")
                {
                    Ss100LL = "0";
                }
                if (SsUL94 == "")
                {
                    SsUL94 = "0";
                }
                if (SsMOGAS == "")
                {
                    SsMOGAS = "0";
                }
                if (SsSAF == "")
                {
                    SsSAF = "0";
                }
                if (SsSAFPRIST == "")
                {
                    SsSAFPRIST = "0";
                }

                Decimal decFsJETA = Convert.ToDecimal(FsJETA);
                Decimal decFsJETAPRIST = Convert.ToDecimal(FsJETAPRIST);
                Decimal decFs100LL = Convert.ToDecimal(Fs100LL);
                Decimal decFsUL94 = Convert.ToDecimal(FsUL94);
                Decimal decFsMOGAS = Convert.ToDecimal(FsMOGAS);
                Decimal decFsSAF = Convert.ToDecimal(FsSAF);
                Decimal decFsSAFPRIST = Convert.ToDecimal(FsSAFPRIST);
                Decimal decSsJETA = Convert.ToDecimal(SsJETA);
                Decimal decSsJETAPRIST = Convert.ToDecimal(SsJETAPRIST);
                Decimal decSs100LL = Convert.ToDecimal(Ss100LL);
                Decimal decSsUL94 = Convert.ToDecimal(SsUL94);
                Decimal decSsMOGAS = Convert.ToDecimal(SsMOGAS);
                Decimal decSsSAF = Convert.ToDecimal(SsSAF);
                Decimal decSsSAFPRIST = Convert.ToDecimal(SsSAFPRIST);
                int FuelBrandID = 0;
                int companyID = Convert.ToInt32(fueldis.companyID);
                FuelBrandID = Convert.ToInt32(fueldis.logoSelected);

                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("JETA", decFsJETA);
                dynamicParameters.Add("JETAPRIST", decFsJETAPRIST);
                dynamicParameters.Add("100LL", decFs100LL);
                dynamicParameters.Add("UL94", decFsUL94);
                dynamicParameters.Add("MOGAS", decFsMOGAS);
                dynamicParameters.Add("SSJETA", decSsJETA);
                dynamicParameters.Add("SSJETAPRIST", decSsJETAPRIST);
                dynamicParameters.Add("SS100LL", decSs100LL);
                dynamicParameters.Add("SSUL94", decSsUL94);
                dynamicParameters.Add("SSMOGAS", decSsMOGAS);
                dynamicParameters.Add("SAF", decFsSAF);
                dynamicParameters.Add("SSSAF", decSsSAF);
                dynamicParameters.Add("SAFPRIST", decFsSAFPRIST);
                dynamicParameters.Add("SSSAFPRIST", decSsSAFPRIST);
                dynamicParameters.Add("FuelBrandID_FK", FuelBrandID);
                _dapper.Execute("FBOManagement_UpdateFuelPrices", dynamicParameters, commandType: CommandType.StoredProcedure);
                return "success";
            }
            catch
            {
                return "failed";
            }
        }
        protected async Task<bool> CheckUpgradeEligibleAsync(string companyID)
        {
            bool isUpgradeEligible = false;
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                string check = await Task.FromResult(_dapper.Get<string>("FBOManagement_CheckUpgradeEligible", dynamicParameters, commandType: CommandType.StoredProcedure));

                if (check.Trim() == "Yes")
                {
                    isUpgradeEligible = true;
                }
            }
            catch (Exception ex)
            {
                //lblTest.Text = "Error in FBOManagement_CheckUpgradeEligible: " + ex.Message.ToString();
            }
            return isUpgradeEligible;

        }
        //public async Task<RatingStats> getRatingStatsAsync(int fboID)
        //{
        //    #region local variables declaration
        //    int userTotal = 0;
        //    int total_ratings = 0;
        //    double avgRating1 = 0.0;
        //    double avgRating2 = 0.0;
        //    double avgRating3 = 0.0;
        //    double avgRating4 = 0.0;
        //    double avgRating5 = 0.0;
        //    double totalRating1 = 0.0;
        //    double totalRating2 = 0.0;
        //    double totalRating3 = 0.0;
        //    double totalRating4 = 0.0;
        //    double totalRating5 = 0.0;
        //    double avgOverallRating = 0.0;
        //    RatingStats ratingStats = new RatingStats();
        //    #endregion local variables declaration
        //    using (var ctx = new globalairARC())
        //    {
        //        DynamicParameters dynamicParameters = new DynamicParameters();
        //        dynamicParameters.Add("fbo_id", fboID);
        //        services_Ratings_FBORatingsTotal_Result temp = await Task.FromResult(_dapper.Get<services_Ratings_FBORatingsTotal_Result>("services_Ratings_FBORatingsTotal", dynamicParameters, commandType: CommandType.StoredProcedure));
        //        userTotal = Convert.ToInt16(temp.TotalUsersRated);
        //        totalRating1 = Convert.ToInt16(temp.TotalRating1);
        //        totalRating2 = Convert.ToInt16(temp.TotalRating2);
        //        totalRating3 = Convert.ToInt16(temp.TotalRating3);
        //        totalRating4 = Convert.ToInt16(temp.TotalRating4);
        //        totalRating5 = Convert.ToInt16(temp.TotalRating5);
        //    }

        //    if (userTotal != 0)
        //    {
        //        //Calculate average ratings for each categories and round to first decimal place.
        //        avgRating1 = (double)Math.Round(totalRating1 / userTotal, 1);
        //        avgRating2 = (double)Math.Round(totalRating2 / userTotal, 1);
        //        avgRating3 = (double)Math.Round(totalRating3 / userTotal, 1);
        //        avgRating4 = (double)Math.Round(totalRating4 / userTotal, 1);
        //        avgRating5 = (double)Math.Round(totalRating5 / userTotal, 1);

        //        //Calculate overall average ratings
        //        avgOverallRating = (avgRating1 + avgRating2 + avgRating3 + avgRating4 + avgRating5) / 5;

        //        ratingStats.avgRating1 = avgRating1;
        //        ratingStats.avgRating2 = avgRating2;
        //        ratingStats.avgRating3 = avgRating3;
        //        ratingStats.avgRating4 = avgRating4;
        //        ratingStats.avgRating5 = avgRating5;

        //        ratingStats.averageRating = (double)Math.Round(avgOverallRating, 1);
        //        ratingStats.RatedBy = userTotal.ToString();
        //    }
        //    return ratingStats;
        //}


    }
}