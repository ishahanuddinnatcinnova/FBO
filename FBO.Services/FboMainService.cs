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
using Dapper;
using GlobalAir.Data;
using Microsoft.AspNetCore.Http;

namespace FBO.Services
{
    public class FBOMainService
    {
        private static Dapperr _dapper;
        private static GeneralService _generalService;
        private static UtilitiesService _utility;
        public String fuel_status = "";
        public FBOMainService(Dapperr dapper, GeneralService generalService, UtilitiesService utilities)
        {
            _dapper = dapper;
            _generalService = generalService;
            _utility = utilities;
        }
        public async Task<ServiceResponseViewModel> GetResponse(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);
                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        List<FBOManagement_GetFBOs_Result> fbos = await _generalService.GetFBOs(userData.userID);
                        response.data.FBOs = fbos;

                        if (fbos.Count > 1)
                        {
                            response.isRedirect = false;
                        }
                        else
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/fbo/companymanage.aspx?companyid=" + userData.companyID;
                        }
                    }

                    //var fbo = await _fboMainService.CheckQuery(Convert.ToInt32(companyID), userData.userID, fuel);
                    //return View(fbo);
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }
        public async Task<ServiceResponseViewModel> GetResponseForServices(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                FBOManagement_UpdateBasicServices_Result basic = new FBOManagement_UpdateBasicServices_Result();
                FBOManagement_UpdateExtendedServices_Result extended = new FBOManagement_UpdateExtendedServices_Result();

                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);

                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        response.isRedirect = true;
                        response.redirectURL = "/myflightdept/account.aspx";
                    }
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }
        public async Task<ServiceResponseViewModel> GetResponseForFuelCardsSelected(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);

                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);
                        FuelCardDiscountsModel fuelcards = await _generalService.GetFuelCards(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.data.fuelcards = fuelcards;
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        response.isRedirect = true;
                        response.redirectURL = "/myflightdept/account.aspx";
                    }
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }
        
        public async Task<ServiceResponseViewModel> GetResponseForFuelPrice(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);

                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                     
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        response.isRedirect = true;
                        response.redirectURL = "/myflightdept/account.aspx";
                    }
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }
        public async Task<ServiceResponseViewModel> GetResponseForLogoService(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);

                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        FBOResult res = await _generalService.GetFBO(companyID);
                        FBOLogoServiceModel logo = await _generalService.GetLogoService(companyID);
                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.data.fbologoser = logo;
                            response.isRedirect = false;
                        }
                    }
                    else
                    {
                        response.isRedirect = true;
                        response.redirectURL = "/myflightdept/account.aspx";
                    }
                }
                else
                {
                    response.isRedirect = true;
                    response.redirectURL = "/myflightdept/account.aspx";
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                response.isRedirect = true;
                response.redirectURL = "/myflightdept/account.aspx";
                return response;
            }
        }
        public string PostBasicServicesUpdate(FBOManagement_UpdateBasicServices_Result updatebasic)
        {
            string response = "";
            try
            {
                if (updatebasic.companyID != null && updatebasic.companyID != "")
                {
                    response =  _generalService.SaveButtonBasicService(updatebasic);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string PostExtendedServicesUpdate(FBOManagement_UpdateExtendedServices_Result updateextended)
        {
            string response = "";
            try
            {
                if (updateextended.companyID != null && updateextended.companyID != "")
                {
                    response =  _generalService.SaveButtonExtendedService(updateextended);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public  string PostFuelCardUpdate(FuelCardDiscountsModel fuel)
        {
            string response = "";
            try
            {
                if (fuel.companyID != null && fuel.companyID != "")
                {
                    response =  _generalService.BtnFuelCardSaveClick(fuel);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string PostFuelPriceUpdate(FuelPriceUpdateModel fuel)
        {
            string response ="" ;
            try
            {
                if (fuel.companyID != null && fuel.companyID != "")
                {
                    response =  _generalService.BtnFuelPriceSaveClick(fuel);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string PostLogoServicesUpdate(FBOLogoServiceModel logo)
        {
            string response = "";
            try
            {
                if (logo.companyID != null && logo.companyID != "")
                {
                    response =  _generalService.BtnLogoServicesSaveClick(logo);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
    }
}
