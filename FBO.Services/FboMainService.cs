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
using System.Dynamic;

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


        public async Task<ServiceResponseViewModel> GetResponseForFboInformation(HttpRequest request, string companyID, string fuel)
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
                        var locations = await _generalService.GetLocations();
                        var singleFbores = await _generalService.SingleFboRes(companyID);
                        services_Accepted_GetCreditCards_Result cardinfo = await _generalService.GetCreditCardInfo(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.data.fboCreditCards = cardinfo;
                            response.data.locations = locations;
                            response.data.singleFBO = singleFbores;
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

        public async Task<ServiceResponseViewModel> GetResponseForCustomServices(HttpRequest request, string companyID, string fuel)
        {
            try
            {
                ServiceResponseViewModel response = new ServiceResponseViewModel();
                UserViewModel userData = _utility.CheckLogin(request);
                //FBO
                if (userData.isUser)
                {
                    if (companyID != null && companyID != "")
                    {
                        
                        FBOResult res = await _generalService.GetFBO(companyID);
                        var customSer = await _generalService.GetCustomServices(companyID);

                        if (res.FBO.UserID.ToString() != userData.userID)
                        {
                            response.isRedirect = true;
                            response.redirectURL = "/myflightdept/account.aspx";
                        }
                        else
                        {
                            response.data.FBO = res;
                            response.data.customServices = customSer;
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
                    response = _generalService.SaveButtonBasicService(updatebasic);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public async Task <string> PostFboInfoUpdate(FBOInfoUpdateModel updateinfo,HttpRequest request)
        {
            string response = "";
            try
            {
                if (updateinfo.companyID != null && updateinfo.companyID != "")
                {
                    UserViewModel userData = _utility.CheckLogin(request);
                    response = await _generalService.UploadFboLogoBtn(updateinfo.logo, updateinfo.companyID);
                    response = await _generalService.UploadManagerPicBtn(updateinfo.managerpic, updateinfo.companyID);
                    response = _generalService.SaveButtonUpdateFboInfo(updateinfo,userData.userFirstname);
                }
                return response;
            }
            catch
            {
                return response;
            }
        }
        public async Task<string> SaveUpdateCustomServices(FBOManagement_GetCustomServices_Result res)
        {
            string response = "";
            try
            {
                if (res.CompanyID != null )
                {
                   if(res.serviceID !=0 && res.serviceID != null)
                    {
                        response =  _generalService.UpdateExistingCustomService(res.serviceID, res.customservice);
                    }
                   else
                    {
                        response = _generalService.SaveCustomService(res.CompanyID, res.customservice);
                    }
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        //public string SaveFboLogo(IFormFile logofile, string companyID, string logo)
        //{
        //    string response = "";
        //    try
        //    {
        //        if (companyID != null && companyID != "")
        //        {
        //            response = _generalService.UploadFboLogoBtn(logofile, companyID, logo);
        //        }
        //        return response;
        //    }
        //    catch
        //    {
        //        return response;
        //    }


        //}
        public string DeleteFboLogo(string companyID, string logo)
        {
            string response = "";
            try
            {
                if (companyID != null && companyID != "")
                {
               
                    response = _generalService.DeleteLogo(companyID, logo);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string DeleteManagerPic(string companyID, string managerpic)
        {
            string response = "";
            try
            {
                if (companyID != null && companyID != "")
                {
               
                    response = _generalService.DeleteManagerPic(companyID, managerpic);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string DeleteCustomService(int serviceID)
        {
            string response = "";
            try
            {
                if (serviceID != null && serviceID !=0 )
                {

                    response = _generalService.DeleteCustomService(serviceID);
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
                    response = _generalService.SaveButtonExtendedService(updateextended);
                }
                return response;
            }
            catch
            {
                return response;
            }


        }
        public string PostFuelCardUpdate(FuelCardDiscountsModel fuel)
        {
            string response = "";
            try
            {
                if (fuel.companyID != null && fuel.companyID != "")
                {
                    response = _generalService.BtnFuelCardSaveClick(fuel);
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
            string response = "";
            try
            {
                if (fuel.companyID != null && fuel.companyID != "")
                {
                    response = _generalService.BtnFuelPriceSaveClick(fuel);
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
                    response = _generalService.BtnLogoServicesSaveClick(logo);
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
