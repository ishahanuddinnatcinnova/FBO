using Dapper;
using FBO.Dapper;
using FBO.ViewModels;
using GlobalAir.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        public async Task<FBOManagement_GetFBO_Result> FboResult(int companyID)
        {
            FBOManagement_GetFBO_Result fboResult = new FBOManagement_GetFBO_Result();
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("companyID", companyID);
                var fbo = _dapper.Get<FBOManagement_GetFBO_Result>("FBOManagement_GetFBO", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (fbo.CompanyID != 0)
                {
                    fboResult.Company = fbo.Company;
                    fboResult.Phone = fbo.Phone;
                    fboResult.Email = fbo.Email;
                    fboResult.FBOLevel = fbo.FBOLevel;
                    fboResult.FAARepairCode = fbo.FAARepairCode;
                    fboResult.Address1 = fbo.Address1;
                    fboResult.City = fbo.City;
                    fboResult.State = fbo.State;
                    fboResult.Zip = fbo.Zip;
                    fboResult.Country = fbo.Country;
                    fboResult.IsApproved = fbo.IsApproved;
                    fboResult.LastUpdated = fbo.LastUpdated;
                    fboResult.FuelJETA = fbo.FuelJETA;
                    fboResult.FuelJETAPRIST = fbo.FuelJETAPRIST;
                    fboResult.Fuel100LL = fbo.Fuel100LL;
                    fboResult.FuelUL94 = fbo.FuelUL94;
                    fboResult.FuelMOGAS = fbo.FuelMOGAS;
                    fboResult.FuelSAF = fbo.FuelSAF;
                    fboResult.FuelSAFPRIST = fbo.FuelSAFPRIST;
                    fboResult.FuelSSJETA = fbo.FuelSSJETA;
                    fboResult.FuelSSJETAPRIST = fbo.FuelSSJETAPRIST;
                    fboResult.FuelSS100LL = fbo.FuelSS100LL;
                    fboResult.FuelSSUL94 = fbo.FuelSSUL94;
                    fboResult.FuelSSMOGAS = fbo.FuelSSMOGAS;
                    fboResult.FuelSSSAF = fbo.FuelSSSAF;
                    fboResult.FuelSSSAFPRIST = fbo.FuelSSSAFPRIST;

                }





                return fboResult;
            }
            catch (Exception ex)
            {
                return null;
            }




        }
        public bool CheckFboExpired(FboResultMainModel fbo)
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

            FboResultMainModel fbo = new FboResultMainModel();
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
        public async Task<int> CountFbo(string userID)
        {
           
        
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("userID", userID);
                var fboCount = await Task.FromResult(_dapper.GetAll<List<FBOManagement_GetFBO_Result>>("FBOManagement_GetFBOs", dynamicParameters, commandType: CommandType.StoredProcedure));
 
                return fboCount.Count;

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
                var fuelavg = _dapper.Get<FBOManagement_GetRegionAverages_Result>("FBOManagement_GetRegionAverages", dynamicParameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<FBOManagement_GetFBOs_Result>> GetFbos(string userID)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("userID", userID);
                var fbos = await Task.FromResult(_dapper.GetAll <FBOManagement_GetFBOs_Result>("FBOManagement_GetFBOs", dynamicParameters, commandType: CommandType.StoredProcedure));

                return fbos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}