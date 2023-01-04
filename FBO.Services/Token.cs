using Dapper;
using FBO.Dapper;
using GlobalAir.Data;
using Serilog;
using System.Data;

namespace FBO.Services
{
    public class Token
    {
        private static Dapperr _dapper;

        public int AppId { get; set; }

        public int UserId { get; set; }

        public string AuthToken { get; set; }

        public DateTime IssuedOn { get; set; }

        public DateTime ExpiresOn { get; set; }

        public Token()
        {
        }

        public Token(uspGetAuthCode_Result ds)
        {
            AppId = Convert.ToInt32(ds.AppId);
            UserId = ds.UserId;
            AuthToken = ds.AuthCode.ToString();
            ExpiresOn = ds.AuthExpires.Value.AddHours(2.0);
        }

        public static Token Generate(string email, string password)
        {
            try
            {
                _dapper = new Dapperr();
                //var user = GetUser(mfd_userID);
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("AppId", 0);
                dynamicParameters.Add("EmailAddr", string.IsNullOrEmpty(email) ? "" : email);
                dynamicParameters.Add("Password", string.IsNullOrEmpty(password) ? "" : password);
                uspGetAuthCode_Result user = _dapper.Get<uspGetAuthCode_Result>("Mag.uspGetAuthCode", dynamicParameters, commandType: CommandType.StoredProcedure);
                if ((user != null))
                {
                    return new Token(user);
                }
            }
            catch (Exception)
            {
                Log.Error("Mag.uspGetAuthCode returned no results from the GlobalApi.TokenServices.GenerateToken(0, " + email + ", " + password + ") method.");
            }

            throw new Exception("Mag.uspGetAuthCode returned no results from the GlobalApi.TokenServices.GenerateToken(0, " + email + ", " + password + ") method.");
        }

        public static bool Validate(int appId, string tokenId)
        {
            _dapper = new Dapperr();
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("AppId", appId);
            dynamicParameters.Add("AuthCode", tokenId);
            var isValid = _dapper.Get<bool>("Mag.uspValidateAuthToken", dynamicParameters, commandType: CommandType.StoredProcedure);
            return isValid;
        }
    }
}
