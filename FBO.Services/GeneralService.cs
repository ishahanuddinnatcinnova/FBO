using Dapper;
using FBO.Dapper;
using FBO.ViewModels;
using GlobalAir.Data;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static System.Net.WebRequestMethods;

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
                Log.Error("Error in (Get FBO's) function with user ID" + userID + " Exception is:", ex);
                return null;
            }
        }

        public async Task<FBOResult> GetFBO(string companyID)
        {
            FBOResult fboResultMainModel = new FBOResult();
            fboResultMainModel.FBO = await FboResult(Convert.ToInt16(companyID));

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
            fboResultMainModel.fboIsExpired = CheckFboExpired(fboResultMainModel);
            fboResultMainModel.newReviews = await CheckNewReviewsCount(Convert.ToInt16(companyID));
            fboResultMainModel.averageprices = await GetFuelAverages(Convert.ToInt16(companyID));
            fboResultMainModel.averageFuelPrice = Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_JETA), 2) + Math.Round(Convert.ToDecimal(fboResultMainModel.averageprices.Average_100LL), 2);
            await GetDates(fboResultMainModel);
            fboResultMainModel.fboStats = await getFBOStats(Convert.ToInt16(companyID), fboResultMainModel.startDate, fboResultMainModel.endDate);
            fboResultMainModel.isUpgradeEligible = await CheckUpgradeEligibleAsync(companyID);
            fboResultMainModel.ratingStats = await getRatingStatsAsync(Convert.ToInt16(companyID));

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
                Log.Error("Error in---Get FBO function---with company ID" + companyID + " Exception is:", ex);
                return null;
            }
        }
        public async Task<List<FBOManagement_GetCustomServices_Result>> GetCustomServices(string companyID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                var customer = await Task.FromResult(_dapper.GetAll<FBOManagement_GetCustomServices_Result>("FBOManagement_GetCustomServices", dynamicParameters, commandType: CommandType.StoredProcedure));

                return customer;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetCustomServices function---with company ID" + companyID + " Exception is:", ex);
                return null;
            }
        }
        public async Task<ARC_SingleFBO_Result> SingleFboRes(string companyID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("fboID", companyID);
                var fbo = await Task.FromResult(_dapper.Get<ARC_SingleFBO_Result>("arc.ARC_SingleFBO", dynamicParameters, commandType: CommandType.StoredProcedure));
                if (fbo.CompID != 0)
                {
                    return fbo;

                }
                return fbo;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---SingleFboRes function---with company ID" + companyID + " Exception is:", ex);
                return null;
            }
        }
        public async Task<FBOLogoServiceModel> GetLogoService(string companyID)
        {
            FBOLogoServiceModel logo = new FBOLogoServiceModel();
            String fbo_LogoServices = "";
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                var fbo = await Task.FromResult(_dapper.Get<FBOManagement_GetFBO_Result>("FBOManagement_GetFBO", dynamicParameters, commandType: CommandType.StoredProcedure));
                fbo_LogoServices = fbo.logoservices;
                String l = "logos," + fbo_LogoServices + ",";

                var logos = Logo(logo, l);
                return logos;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetLogoService function---with company ID" + companyID + " Exception is:", ex);
                return null;
            }
        }
        public async Task<services_Accepted_GetCreditCards_Result> GetCreditCardInfo(string companyID)
        {
            services_Accepted_GetCreditCards_Result logo = new services_Accepted_GetCreditCards_Result();

            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                var cardInfo = await Task.FromResult(_dapper.Get<services_Accepted_GetCreditCards_Result>("services_Accepted_GetCreditCards", dynamicParameters, commandType: CommandType.StoredProcedure));


                return cardInfo;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetCreditCardInfo function---with company ID" + companyID + " Exception is:", ex);
                return null;
            }
        }
        public async Task<List<uspGetAllLocations_Result>> GetLocations()
        {


            try
            {

                List<uspGetAllLocations_Result> cardInfo = await Task.FromResult(_dapper.GetAll<uspGetAllLocations_Result>("Web.uspGetAllLocations", null, commandType: CommandType.StoredProcedure));


                return cardInfo;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetLocations function---with company ID" + " Exception is:", ex);
                return null;
            }
        }
        public string DeleteLogo(String companyID, String logo)
        {

            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("fboCompID", companyID);
                dynamicParameters.Add("fboLogo", logo);
                _dapper.Execute("Services_FBO_DeleteLogo", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---DeleteLogo function---with company ID" + companyID + " Exception is:", ex);
                return "failed";
            }

        }
        public string DeleteManagerPic(String companyID, String ManagerPic)
        {

            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("ManagerPhoto", ManagerPic);
                _dapper.Execute("Services_DeleteManagerPhoto", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---DeleteManagerPic function---with company ID" + companyID + " Exception is:", ex);
                return "failed";
            }

        }


        public FBOLogoServiceModel Logo(FBOLogoServiceModel logo, string l)
        {
            if (l.IndexOf(",1,") > 0)
            {
                logo.chkLogoService01 = true;
            }
            if (l.IndexOf(",2,") > 0)
            {
                logo.chkLogoService02 = true;
            }
            if (l.IndexOf(",3,") > 0)
            {
                logo.chkLogoService03 = true;
            }
            if (l.IndexOf(",4,") > 0)
            {
                logo.chkLogoService04 = true;
            }
            if (l.IndexOf(",5,") > 0)
            {
                logo.chkLogoService05 = true;
            }
            if (l.IndexOf(",6,") > 0)
            {
                logo.chkLogoService06 = true;
            }
            if (l.IndexOf(",11,") > 0)
            {
                logo.chkLogoService11 = true;
            }
            if (l.IndexOf(",13,") > 0)
            {
                logo.chkLogoService13 = true;
            }
            if (l.IndexOf(",14,") > 0)
            {
                logo.chkLogoService14 = true;
            }
            if (l.IndexOf(",15,") > 0)
            {
                logo.chkLogoService15 = true;
            }
            if (l.IndexOf(",16,") > 0)
            {
                logo.chkLogoService16 = true;
            }
            if (l.IndexOf(",17,") > 0)
            {
                logo.chkLogoService17 = true;
            }
            if (l.IndexOf(",18,") > 0)
            {
                logo.chkLogoService18 = true;
            }
            if (l.IndexOf(",19,") > 0)
            {
                logo.chkLogoService19 = true;
            }
            if (l.IndexOf(",20,") > 0)
            {
                logo.chkLogoService20 = true;
            }
            if (l.IndexOf(",21,") > 0)
            {
                logo.chkLogoService21 = true;
            }
            if (l.IndexOf(",22,") > 0)
            {
                logo.chkLogoService22 = true;
            }

            if (l.IndexOf(",26,") > 0)
            {
                logo.chkLogoService26 = true;
            }

            if (l.IndexOf(",30,") > 0)
            {
                logo.chkLogoService30 = true;
            }
            if (l.IndexOf(",31,") > 0)
            {
                logo.chkLogoService31 = true;
            }
            if (l.IndexOf(",32,") > 0)
            {
                logo.chkLogoService32 = true;
            }
            if (l.IndexOf(",33,") > 0)
            {
                logo.chkLogoService33 = true;
            }
            if (l.IndexOf(",34,") > 0)
            {
                logo.chkLogoService34 = true;
            }
            if (l.IndexOf(",35,") > 0)
            {
                logo.chkLogoService35 = true;
            }
            if (l.IndexOf(",36,") > 0)
            {
                logo.chkLogoService36 = true;
            }
            if (l.IndexOf(",37,") > 0)
            {
                logo.chkLogoService37 = true;
            }
            if (l.IndexOf(",38,") > 0)
            {
                logo.chkLogoService38 = true;
            }
            if (l.IndexOf(",39,") > 0)
            {
                logo.chkLogoService39 = true;
            }
            if (l.IndexOf(",40,") > 0)
            {
                logo.chkLogoService40 = true;
            }

            if (l.IndexOf(",42,") > 0)
            {
                logo.chkLogoService42 = true;
            }
            if (l.IndexOf(",45,") > 0)
            {
                logo.chkLogoService45 = true;
            }
            if (l.IndexOf(",46,") > 0)
            {
                logo.chkLogoService46 = true;
            }
            if (l.IndexOf(",47,") > 0)
            {
                logo.chkLogoService47 = true;
            }

            if (l.IndexOf(",54,") > 0)
            {
                logo.chkLogoService54 = true;
            }
            if (l.IndexOf(",56,") > 0)
            {
                logo.chkLogoService56 = true;
            }

            if (l.IndexOf(",60,") > 0)
            {
                logo.chkLogoService60 = true;
            }

            if (l.IndexOf(",63,") > 0)
            {
                logo.chkLogoService63 = true;
            }

            if (l.IndexOf(",72,") > 0)
            {
                logo.chkLogoService72 = true;
            }

            if (l.IndexOf(",73,") > 0)
            {
                logo.chkLogoService73 = true;
            }

            if (l.IndexOf(",74,") > 0)
            {
                logo.chkLogoService74 = true;
            }

            if (l.IndexOf(",75,") > 0)
            {
                logo.chkLogoService75 = true;
            }

            if (l.IndexOf(",76,") > 0)
            {
                logo.chkLogoService76 = true;
            }

            if (l.IndexOf(",77,") > 0)
            {
                logo.chkLogoService77 = true;
            }

            if (l.IndexOf(",78,") > 0)
            {
                logo.chkLogoService78 = true;
            }

            if (l.IndexOf(",79,") > 0)
            {
                logo.chkLogoService79 = true;
            }

            if (l.IndexOf(",80,") > 0)
            {
                logo.chkLogoService80 = true;
            }

            if (l.IndexOf(",81,") > 0)
            {
                logo.chkLogoService81 = true;
            }

            if (l.IndexOf(",82,") > 0)
            {
                logo.chkLogoService82 = true;
            }

            if (l.IndexOf(",83,") > 0)
            {
                logo.chkLogoService83 = true;
            }
            return logo;
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
        public async Task<int> CheckNewReviewsCount(int companyID)
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
                Log.Error("Error in (CheckNewReviewsCount) function with company ID" + companyID + " Exception is:", ex);
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
                    fuelaverages.Average_SAF = fuelavg.Average_SAF;
                    fuelaverages.FAARegionCode = fuelavg.FAARegionCode;
                    fuelaverages.RegionName = fuelavg.RegionName;

                }

                return fuelaverages;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetFuelAverages Function---with company ID" + companyID + " Exception is:", ex);
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
        public async Task<FBOManagement_Stats_Result> getFBOStats(int CompanyID, String date_start, String date_end)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", CompanyID);
                dynamicParameters.Add("startdate", date_start);
                dynamicParameters.Add("enddate", date_end);
                var fboStats = await Task.FromResult(_dapper.Get<FBOManagement_Stats_Result>("FBOManagement_Stats", dynamicParameters, commandType: CommandType.StoredProcedure));

                return fboStats;
            }
            catch (Exception ex)
            {
                Log.Error("Error in---GetFBOStats Function---with company ID" + CompanyID + " Exception is:", ex);
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
                Log.Error("Error in---UpdateLastUpdated Function---with company ID" + CompanyID + " Exception is:", ex);
            }
        }
        public string SaveButtonBasicService(FBOManagement_UpdateBasicServices_Result basic)
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
                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---SaveButtonBasicService Function---with company ID" + basic.companyID + " Exception is:", ex);
                return "failed";
            }
        }
        public string SaveButtonUpdateFboInfo(FBOInfoUpdateModel info, string user)
        {
            int companyID = 0;
            companyID = Convert.ToInt32(info.companyID);
            try
            {
                string userName = user;
                int cc_mastercard = 0;
                int cc_visa = 0;
                int cc_discover = 0;
                int cc_amex = 0;
                String ManagerName = info.ManagerName != null ? info.ManagerName.Trim() : "";
                String Address1 = info.Address1 != null ? info.Address1.Trim() : "";
                String Address2 = info.Address2 != null ? info.Address2.Trim() : "";
                String City = info.City != null ? info.City.Trim() : "";
                int stateid;
                int.TryParse(info.stateid, out stateid);
                String Zipcode = info.Zipcode != null ? info.Zipcode.Trim() : "";
                String Phone = info.Phone != null ? info.Phone.Trim() : "";
                String Fax = info.Fax != null ? info.Fax.Trim() : "";
                String Arinc = info.Arinc != null ? info.Arinc.Trim() : "";
                String Unicom = info.Unicom != null ? info.Unicom.Trim() : "";
                String FAARepair = info.FAARepair != null ? info.FAARepair.Trim() : "";
                String Email = info.Email != null ? info.Email.Trim() : "";
                String URL = info.URL != null ? info.URL.Trim() : "";
                String OpHours = info.OpHours != null ? info.OpHours.Trim() : "";
                String RampDescription = info.RampDescription != null ? info.RampDescription.Trim() : "";
                String ComDescription = info.ComDescription != null ? info.ComDescription.Trim() : "";
                String strRampFee = info.strRampFee != null ? info.strRampFee.Trim() : "";
                int rampFee = strRampFee != null ? (strRampFee == "Yes" ? 1 : 0) : 0;

                ManagerName = ManagerName.Replace("\"", "");
                Address1 = Address1.Replace("\"", "");
                Address2 = Address2.Replace("\"", "");
                City = City.Replace("\"", "");
                FAARepair = FAARepair.Replace("\"", "");
                OpHours = OpHours.Replace("\"", "");
                if (string.IsNullOrEmpty(info.username))
                {
                    userName = "*UNKNOWN";
                }
                else
                {
                    userName = user;
                }
                if (info.cc_mastercard == true)
                {
                    cc_mastercard = 1;
                }
                else
                {
                    cc_mastercard = 0;
                }

                if (info.cc_visa == true)
                {
                    cc_visa = 1;
                }
                else
                {
                    cc_visa = 0;
                }

                if (info.cc_discover == true)
                {
                    cc_discover = 1;
                }
                else
                {
                    cc_discover = 0;
                }

                if (info.cc_amex == true)
                {
                    cc_amex = 1;
                }
                else
                {
                    cc_amex = 0;
                }
                DynamicParameters dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("manager", ManagerName);
                dynamicParameters.Add("Addr1", Address1);
                dynamicParameters.Add("Addr2", Address2);
                dynamicParameters.Add("City", City);
                dynamicParameters.Add("StateId", stateid);
                dynamicParameters.Add("Zip", Zipcode);
                dynamicParameters.Add("Phone", Phone);
                dynamicParameters.Add("Fax", Fax);
                dynamicParameters.Add("Arinc", Arinc);
                dynamicParameters.Add("Unicom", Unicom);
                dynamicParameters.Add("faarepair", FAARepair);
                dynamicParameters.Add("Email", Email);
                dynamicParameters.Add("URL", URL);
                dynamicParameters.Add("ophours", OpHours);
                dynamicParameters.Add("rampfee", rampFee);
                dynamicParameters.Add("rampdesc", RampDescription);
                dynamicParameters.Add("Description", ComDescription);
                dynamicParameters.Add("UserName", userName);
                dynamicParameters.Add("CompanyID", info.companyID);
                _dapper.Execute("Web.FBOManagement_UpdateCompanyInfo", dynamicParameters, commandType: CommandType.StoredProcedure);

                DynamicParameters dynamicParametersForcard = new DynamicParameters();
                dynamicParametersForcard.Add("CompanyID", companyID);
                dynamicParametersForcard.Add("IsMastercard", cc_mastercard);
                dynamicParametersForcard.Add("IsVisa", cc_visa);
                dynamicParametersForcard.Add("IsDiscover", cc_discover);
                dynamicParametersForcard.Add("IsAMEX", cc_amex);
                dynamicParametersForcard.Add("UserName", userName);
                _dapper.Execute("dbo.services_Accepted_SaveCreditCards", dynamicParametersForcard, commandType: CommandType.StoredProcedure);
                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---SaveButtonUpdateFboInfo Function---with company ID" + companyID + " Exception is:", ex);
                return "failed";
            }


        }
        public string SaveButtonExtendedService(FBOManagement_UpdateExtendedServices_Result extended)
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
                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---SaveButtonExtendedService Function---with company ID" + companyID + " Exception is:", ex);
                return "failed";

            }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        public async Task<string> UploadFboLogoBtn(IFormFile myFile, string companyID)
        {
            string ARCPath = "\\\\globalweb-new\\resources.globalair.com\\wwwroot\\airport\\images\\complogo\\";
            String ARCURL = "https://resources.globalair.com/airport/images/complogo/";

            int uploadedWidth = 0;
            int uploadedHeight = 0;
            int resizedWidth = 175;
            int resizedHeight = 0;
            String logoFilename = "";

            if (myFile != null && companyID != "")
            {
                Log.Information("myFile is not null and company ID is also not empty");
                logoFilename = companyID;
                logoFilename = logoFilename.Replace("%7B", "");
                logoFilename = logoFilename.Replace("%2D", "-");
                logoFilename = logoFilename.Replace("%7D", "");

                if (logoFilename.Length > 8)
                {
                    logoFilename = logoFilename.Substring(0, 8);
                    Log.Information("my file name is greater then 8");
                }

                logoFilename = logoFilename + ".jpg";

                long nFileLen = myFile.Length;
                if (nFileLen == 0)
                {
                    Log.Error("The file you tried to upload was empty or could not be read. Please try again.");
                    return "The file you tried to upload was empty or could not be read. Please try again.";
                }
                else
                {
                    if (Path.GetExtension(myFile.FileName).ToLower() != ".jpg" && Path.GetExtension(myFile.FileName).ToLower() != ".jpeg")
                    {
                        Log.Error("The file must have an extension of .jpg or .jpeg. Please try again.");
                        return "The file must have an extension of .jpg or .jpeg. Please try again.";
                    }
                    else
                    {
                        Log.Information("The file is of jpg and jpeg format");
                        var myData = await GetBytes(myFile);

                        String sFilename = "";
                        sFilename = logoFilename;

                        int file_append = 0;
                        try
                        {
                            while (System.IO.File.Exists(ARCPath + sFilename))
                            {
                                Log.Information("File already exist add append ");
                                file_append++;
                                sFilename = Path.GetFileNameWithoutExtension(logoFilename) +
                                            file_append.ToString() + ".jpg";
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "file append error, path: " + ARCPath + sFilename);
                            return "failed";
                        }
                        try
                        {
                            using (FileStream newFile = new FileStream(ARCPath + sFilename, FileMode.Create))
                            {
                                Log.Information("Copy to New File");
                                newFile.Write(myData, 0, myData.Length);
                                newFile.Flush();
                                newFile.Close();
                            }
                        }
                        catch (IOException ex)
                        {
                            Log.Error(ex, "Copying of Bytes to New File Failed, path: " + ARCPath + sFilename);
                            return "failed";
                        }
                        
                        Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                        Bitmap myBitmap;
                        try
                        {
                            myBitmap = new Bitmap(ARCPath + sFilename);
                            try
                            {
                                Log.Information("File Resize");

                                uploadedWidth = myBitmap.Width;
                                uploadedHeight = myBitmap.Height;
                                try
                                {

                                    if (uploadedWidth > 175 && uploadedHeight > 0)
                                    {
                                        resizedHeight = (int)(((double)resizedWidth / (double)uploadedWidth) * (double)uploadedHeight);
                                        if (resizedHeight < 1)
                                        {
                                            resizedHeight = uploadedHeight;
                                        }
                                        Image myThumbnail = myBitmap.GetThumbnailImage(resizedWidth, resizedHeight, myCallBack, IntPtr.Zero);

                                        int resize_append = 0;

                                        while (System.IO.File.Exists(ARCPath + sFilename))
                                        {
                                            resize_append++;
                                            sFilename = Path.GetFileNameWithoutExtension(sFilename) +
                                                        resize_append.ToString() + ".jpg";
                                        }
                                        myThumbnail.Save(ARCPath + sFilename);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "File Resize failed");
                                    return "failed";
                                }

                                //copy the FBO logo to the / sm folder
                                try
                                {
                                    Log.Information("copy the FBO logo to the / sm folder, filepath = " + ARCPath + sFilename);
                                    String arc_filepath = ARCPath + sFilename;
                                    String sm_filepath = ARCPath + @"\sm\" + sFilename;
                                    Log.Information("copy the FBO logo to the / sm folder, filepath = " + ARCPath + @"\sm\" + sFilename);

                                    System.IO.File.Copy(arc_filepath, sm_filepath, true);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "copy the FBO logo to the / sm folder Failed");
                                    return "failed";
                                }
                                if (companyID != "" && sFilename != "")
                                {
                                    try
                                    {
                                        Log.Information("Adding File Name to FBO");
                                        DynamicParameters dynamicParameters = new DynamicParameters();
                                        dynamicParameters.Add("fboCompID", companyID);
                                        dynamicParameters.Add("fboLogo", sFilename);
                                        _dapper.Execute("Services_FBO_UploadLogo", dynamicParameters, commandType: CommandType.StoredProcedure);
                                        return "success";
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex, "Adding File Name to FBO failed");
                                        return "failed";

                                    }
                                }
                                return "success";
                            }
                            finally
                            {
                                myBitmap.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "File Resize, Upload Logo Name,Copy To SM Folder Failed. path" + ARCPath + sFilename);
                            System.IO.File.Delete(ARCPath + sFilename);
                            return "failed";
                        }
                    }
                }
            }
            else
            {
                Log.Error("No file Found");
                return "failed";
            }
        }
        public string UpdateExistingCustomService(int existingserviceID, String service)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("serviceID", existingserviceID);
                dynamicParameters.Add("service", service);
                _dapper.Execute("FBOManagement_UpdateCustomService", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---UpdateExistingCustomService Function---with serviceID ID" + existingserviceID + " Exception is:", ex);
                return "failed";
            }
        }
        public string SaveCustomService(int companyID, string newcustomservice)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", companyID);
                dynamicParameters.Add("service", newcustomservice);
                _dapper.Execute("FBOManagement_InsertCustomService", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---SaveCustomService Function---with company ID" + companyID + " Exception is:", ex);
                return "failed";
            }

        }
        public string DeleteCustomService(int serviceID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("serviceID", serviceID);

                _dapper.Execute("FBOManagement_DeleteCustomService", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in---DeleteCustomService Function---with serviceID ID" + serviceID + " Exception is:", ex);
                return "failed";
            }

        }
        public async Task<string> UploadManagerPicBtn(IFormFile myFile, string companyID)
        {
            string ARCPath = "\\\\globalweb-new\\resources.globalair.com\\wwwroot\\airport\\images\\managerphotos\\";

            int uploadedWidth = 0;
            int uploadedHeight = 0;
            int resizedWidth = 100;
            int resizedHeight = 0;

            String logoFilename = "";

            if (myFile != null && companyID != "")
            {
                logoFilename = companyID;
                logoFilename = logoFilename.Replace("%7B", "");
                logoFilename = logoFilename.Replace("%2D", "-");
                logoFilename = logoFilename.Replace("%7D", "");

                if (logoFilename.Length > 8)
                {
                    logoFilename = logoFilename.Substring(0, 8);
                }
                
                logoFilename = logoFilename + ".jpg";
                long nFileLen = myFile.Length;
                
                if (nFileLen == 0)
                {
                    Log.Error("Error in---UploadManagerPicBtn Function---with companyID ID" + companyID + "Ex is The file you tried to upload was empty or could not be read. Please try again.");
                    return "The file you tried to upload was empty or could not be read. Please try again.";
                }
                else
                {
                    if (Path.GetExtension(myFile.FileName).ToLower() != ".jpg" && Path.GetExtension(myFile.FileName).ToLower() != ".jpeg")
                    {
                        Log.Error("Error in---UploadManagerPicBtn Function---with companyID ID" + companyID + "Ex is The file must have an extension of .jpg or .jpeg. Please try again.");
                        return "The file must have an extension of .jpg or .jpeg. Please try again.";
                    }
                    else
                    {
                        var myData = await GetBytes(myFile);

                        String sFilename = "";
                        sFilename = logoFilename;

                        int file_append = 0;

                        while (System.IO.File.Exists(ARCPath + sFilename))
                        {
                            file_append++;
                            sFilename = Path.GetFileNameWithoutExtension(logoFilename) +
                                        file_append.ToString() + ".jpg";
                        }
                        using (FileStream newFile = new FileStream(ARCPath + sFilename, FileMode.Create))
                        {
                            newFile.Write(myData, 0, myData.Length);
                            newFile.Flush();
                            newFile.Close();
                        }

                        Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                        Bitmap myBitmap;

                        try
                        {
                            myBitmap = new Bitmap(ARCPath + sFilename);
                            try
                            {
                                uploadedWidth = myBitmap.Width;
                                uploadedHeight = myBitmap.Height;

                                if (uploadedWidth > 175 && uploadedHeight > 0)
                                {
                                    resizedHeight = (int)(((double)resizedWidth / (double)uploadedWidth) * (double)uploadedHeight);
                                    if (resizedHeight < 1)
                                    {
                                        resizedHeight = uploadedHeight;
                                    }
                                    
                                    Image myThumbnail = myBitmap.GetThumbnailImage(resizedWidth, resizedHeight, myCallBack, IntPtr.Zero);
                                    int resize_append = 0;

                                    while (System.IO.File.Exists(ARCPath + sFilename))
                                    {
                                        resize_append++;
                                        sFilename = Path.GetFileNameWithoutExtension(sFilename) +
                                                    resize_append.ToString() + ".jpg";
                                    }
                                    myThumbnail.Save(ARCPath + sFilename);
                                }


                                if (companyID != "" && sFilename != "")
                                {
                                    try
                                    {
                                        DynamicParameters dynamicParameters = new DynamicParameters();
                                        dynamicParameters.Add("CompanyID", companyID);
                                        dynamicParameters.Add("ManagerPhoto", sFilename);
                                        _dapper.Execute("Services_SaveManagerPhoto", dynamicParameters, commandType: CommandType.StoredProcedure);
                                        return "success";
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("Error in---UploadManagerPicBtn Function---with companyID ID" + companyID + "Ex is Services_SaveManagerPhoto failed");
                                        return "failed";
                                    }
                                }
                            }
                            finally
                            {
                                myBitmap.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.IO.File.Delete(ARCPath + sFilename);
                            Log.Error("Error in---UploadManagerPicBtn Function---with companyID ID" + companyID + "Ex is Resize or append failed");
                            return "failed";
                        }
                        return "success";
                    }
                }

            }
            return "No file found";
        }
        public static async Task<byte[]> GetBytes(IFormFile formFile)
        {
            Log.Information("GetBytes func called");
            try
            {
                await using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                Log.Information("GetBytes func returned successfully");
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetBytes func error");
                return null;
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
                Log.Error("Error in---GetFuelCards Function---with companyID ID" + companyID + "Ex is "+ex);
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
            catch(Exception ex)
            {
                Log.Error("Error in---GetFuelCards Function---with companyID ID" + fueldis.companyID + "Ex is " + ex);
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
                Log.Error("Error in---ClearFuelCards Function---with companyID ID" + companyID + "Ex is " + ex);
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
                Log.Error("Error in---AddFuelCard Function---with companyID ID" + companyID + "Ex is " + ex);
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
            catch(Exception ex)
            {
                Log.Error("Error in---BtnFuelPriceSaveClick Function---with companyID ID" + fueldis.companyID + "Ex is " + ex);
                return "failed";
            }
        }
        public string BtnLogoServicesSaveClick(FBOLogoServiceModel logoser)
        {
            string respose = "";

            try
            {

                String ls = "";
                int logoCount = 0;

                if (logoser.chkLogoService01 == true)
                {
                    ls = ls + "1,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService02 == true)
                {
                    ls = ls + "2,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService03 == true)
                {
                    ls = ls + "3,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService04 == true)
                {
                    ls = ls + "4,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService05 == true)
                {
                    ls = ls + "5,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService06 == true)
                {
                    ls = ls + "6,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService11 == true)
                {
                    ls = ls + "11,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService13 == true)
                {
                    ls = ls + "13,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService14 == true)
                {
                    ls = ls + "14,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService15 == true)
                {
                    ls = ls + "15,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService16 == true)
                {
                    ls = ls + "16,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService17 == true)
                {
                    ls = ls + "17,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService18 == true)
                {
                    ls = ls + "18,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService19 == true)
                {
                    ls = ls + "19,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService20 == true)
                {
                    ls = ls + "20,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService21 == true)
                {
                    ls = ls + "21,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService22 == true)
                {
                    ls = ls + "22,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService26 == true)
                {
                    ls = ls + "26,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService30 == true)
                {
                    ls = ls + "30,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService31 == true)
                {
                    ls = ls + "31,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService32 == true)
                {
                    ls = ls + "32,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService33 == true)
                {
                    ls = ls + "33,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService34 == true)
                {
                    ls = ls + "34,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService35 == true)
                {
                    ls = ls + "35,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService36 == true)
                {
                    ls = ls + "36,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService37 == true)
                {
                    ls = ls + "37,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService38 == true)
                {
                    ls = ls + "38,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService39 == true)
                {
                    ls = ls + "39,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService40 == true)
                {
                    ls = ls + "40,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService42 == true)
                {
                    ls = ls + "42,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService45 == true)
                {
                    ls = ls + "45,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService46 == true)
                {
                    ls = ls + "46,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService47 == true)
                {
                    ls = ls + "47,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService54 == true)
                {
                    ls = ls + "54,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService56 == true)
                {
                    ls = ls + "56,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService60 == true)
                {
                    ls = ls + "60,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService63 == true)
                {
                    ls = ls + "63,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService72 == true)
                {
                    ls = ls + "72,";
                    logoCount = logoCount + 1;
                }
                if (logoser.chkLogoService73 == true)
                {
                    ls = ls + "73,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService74 == true)
                {
                    ls = ls + "74,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService75 == true)
                {
                    ls = ls + "75,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService76 == true)
                {
                    ls = ls + "76,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService77 == true)
                {
                    ls = ls + "77,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService78 == true)
                {
                    ls = ls + "78,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService79 == true)
                {
                    ls = ls + "79,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService80 == true)
                {
                    ls = ls + "80,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService81 == true)
                {
                    ls = ls + "81,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService82 == true)
                {
                    ls = ls + "82,";
                    logoCount = logoCount + 1;
                }

                if (logoser.chkLogoService83 == true)
                {
                    ls = ls + "83,";
                    logoCount = logoCount + 1;
                }

                String strlogoCount = logoCount.ToString();

                if (ls.Length > 1)
                {
                    ls = ls.Substring(0, ls.LastIndexOf(","));
                }

                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("CompanyID", logoser.companyID);
                dynamicParameters.Add("service", ls);

                _dapper.Execute("FBOManagement_UpdateLogoService", dynamicParameters, commandType: CommandType.StoredProcedure);

                return "success";


            }
            catch(Exception ex)
            {
                Log.Error("Error in---BtnLogoServicesSaveClick Function---with companyID ID" + logoser.companyID + "Ex is " + ex);
                return "failed";
            }
        }
        public async Task<bool> CheckUpgradeEligibleAsync(string companyID)
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
                Log.Error("Error in---GetFBOStats Function---with company ID" + companyID + " Exception is:", ex);
                return false;
            }
            return isUpgradeEligible;

        }
        public async Task<RatingStats> getRatingStatsAsync(int fboID)
        {
            #region local variables declaration
            int userTotal = 0;
            int total_ratings = 0;
            double avgRating1 = 0.0;
            double avgRating2 = 0.0;
            double avgRating3 = 0.0;
            double avgRating4 = 0.0;
            double avgRating5 = 0.0;
            double totalRating1 = 0.0;
            double totalRating2 = 0.0;
            double totalRating3 = 0.0;
            double totalRating4 = 0.0;
            double totalRating5 = 0.0;
            double avgOverallRating = 0.0;
            RatingStats ratingStats = new RatingStats();
            #endregion local variables declaration

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("fbo_id", fboID);
            services_Ratings_FBORatingsTotal_Result temp = await Task.FromResult(_dapper.Get<services_Ratings_FBORatingsTotal_Result>("services_Ratings_FBORatingsTotal", dynamicParameters, commandType: CommandType.StoredProcedure));
            userTotal = Convert.ToInt16(temp.TotalUsersRated);
            totalRating1 = Convert.ToInt16(temp.TotalRating1);
            totalRating2 = Convert.ToInt16(temp.TotalRating2);
            totalRating3 = Convert.ToInt16(temp.TotalRating3);
            totalRating4 = Convert.ToInt16(temp.TotalRating4);
            totalRating5 = Convert.ToInt16(temp.TotalRating5);

            if (userTotal != 0)
            {
                //Calculate average ratings for each categories and round to first decimal place.
                avgRating1 = (double)Math.Round(totalRating1 / userTotal, 1);
                avgRating2 = (double)Math.Round(totalRating2 / userTotal, 1);
                avgRating3 = (double)Math.Round(totalRating3 / userTotal, 1);
                avgRating4 = (double)Math.Round(totalRating4 / userTotal, 1);
                avgRating5 = (double)Math.Round(totalRating5 / userTotal, 1);

                //Calculate overall average ratings
                avgOverallRating = (avgRating1 + avgRating2 + avgRating3 + avgRating4 + avgRating5) / 5;

                ratingStats.avgRating1 = avgRating1;
                ratingStats.avgRating2 = avgRating2;
                ratingStats.avgRating3 = avgRating3;
                ratingStats.avgRating4 = avgRating4;
                ratingStats.avgRating5 = avgRating5;

                ratingStats.averageRating = (double)Math.Round(avgOverallRating, 1);
                ratingStats.RatedBy = userTotal.ToString();
            }
            try
            {

            int reviewcount = await Task.FromResult(_dapper.Get<int>("select count(*) from reviews_of_ratings r inner join services_ratings s on r.ratingID = s.ratingsID where s.FBOID_FK = " + fboID, dynamicParameters, commandType: CommandType.Text));
            ratingStats.TotalReviews = reviewcount;
            }
            catch(Exception ex)
            {
                Log.Error("Error in---getRatingStatsAsync Function---with fbo ID" + fboID + " Exception is:", ex);
            }
            return ratingStats;
        }
    }
}