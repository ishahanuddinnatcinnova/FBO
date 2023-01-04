using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FBO.Dapper;
using FBO.ViewModels;
using GlobalAir.Data;
using Microsoft.AspNetCore.Http;

namespace FBO.Services
{
    public class UtilitiesService
    {    
        private static Dapperr _dapper;
        public UtilitiesService(Dapperr dapper)
        {
            _dapper = dapper;
        }
        
        public UserViewModel CheckLogin(HttpRequest Request)
        {
            UserViewModel loginViewModel = new UserViewModel();

            loginViewModel.isUser = false;
            loginViewModel.userID = "";
            var cookie = Request.Cookies["GlobalAir"];
            if (cookie != null)
            {
                if (!(cookie != null && cookie != ""))
                {
                    loginViewModel.isUser = false;
                }
                else
                {
                    loginViewModel.userID = cookie.ToString();
                    loginViewModel.userID = loginViewModel.userID.Replace("%7B", "");
                    loginViewModel.userID = loginViewModel.userID.Replace("%2D", "-");
                    loginViewModel.userID = loginViewModel.userID.Replace("%7D", "");
                    try
                    {
                        //var user = GetUser(mfd_userID);
                        DynamicParameters dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("userID", loginViewModel.userID);
                        var user = _dapper.Get<FBOManagement_GetUser_Result>("FBOManagement_GetUser", dynamicParameters, commandType: CommandType.StoredProcedure);
                        if ((user != null) && (user.Username != "") && (user.Password != ""))
                        {
                            loginViewModel.isUser = true;
                            loginViewModel.companyID = user.CompanyID;
                            loginViewModel.userFirstname = user.FirstName;
                            loginViewModel.userID = user.UserID.ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return loginViewModel;
        }
        public static string FormatARC_FBO(string fbo_id, string fbo_compname, string fbo_aptcode)
        {
            string text = "";
            try
            {
                fbo_compname = fbo_compname.Trim();
                fbo_compname = fbo_compname.Replace(" ", "-");
                fbo_compname = fbo_compname.Replace("/", "-");
                fbo_compname = fbo_compname.Replace("&", "and");
                fbo_compname = fbo_compname.Replace(".", "");
                fbo_compname = fbo_compname.Replace(",", "");
                fbo_compname = fbo_compname.Replace("'", "");
                fbo_compname = fbo_compname.Replace("*", "");
                fbo_compname = fbo_compname.Replace(":", "-");
                fbo_compname = fbo_compname.Replace("---", "-");
                fbo_compname = fbo_compname.Replace("--", "-");
                text = "/airport/fbo-at-";
                text = text + fbo_aptcode + "-";
                text += fbo_compname;
                text = text + "-" + fbo_id;
                text += ".aspx";
                text = text.ToLower();
                return text;
            }
            catch (Exception)
            {
                return text;
            }
        }


    }
}
