﻿using Dapper;
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

        public async Task<string> BtnFuelCardSaveClick(FuelCardDiscountsModel fueldis)
        {
            var response=new List<string>();
            try
            {

                response.Add(await ClearFuelCards(fueldis.companyID));
            if (fueldis.chkFuelCard12 == true)
                    response.Add(await  AddFuelCard(fueldis.companyID, 12));

            if (fueldis.chkFuelCard44 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 44));

            if (fueldis.chkFuelCard57 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 57));

            if (fueldis.chkFuelCard61 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 61));

            if (fueldis.chkFuelCard62 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 62));

            if (fueldis.chkFuelCard63 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 63));

            if (fueldis.chkFuelCard64 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 64));

            if (fueldis.chkFuelCard65 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 65));

            if (fueldis.chkFuelCard66 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 66));

            if (fueldis.chkFuelCard68 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 68));

            if (fueldis.chkFuelCard69 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 69));

            if (fueldis.chkFuelCard70 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 70));

            if (fueldis.chkFuelCard71 == true)
                    response.Add(await AddFuelCard(fueldis.companyID, 71));

            foreach(string res in response)
                {
                    if(res== "success")
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
        public async Task<string> ClearFuelCards(string companyID)
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
        public async Task<string> AddFuelCard(string companyID, int discountid)
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


    }
}