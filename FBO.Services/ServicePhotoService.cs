using GlobalAir.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using FBO.ViewModels;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace FBO.Services
{
    public class ServicePhotoService
    {
        private static AppSettings _appSettings;
        public ServicePhotoService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public services_Photos Create(byte[] fileData, string fileName, int companyId)
        {
            _ = _appSettings.FBOPhotosFolder;
            fileName = fileName.Replace("#", "_").Replace("?", "_");
            string text = "FBO_" + companyId + "_Photo_[IMAGEID].jpg";
            int num = Add(companyId, text, fileName);
            if (num > 0)
            {
                text = text.Replace("[IMAGEID]", num.ToString());
                using (FileStream fileStream = new FileStream(_appSettings.FBOPhotosFolder + text, FileMode.Create))
                {
                    fileStream.Write(fileData, 0, fileData.Length);
                    fileStream.Close();
                }

                return new services_Photos
                {
                    CompanyID_FK = companyId,
                    OrginalFileName = fileName,
                    Photo = _appSettings.FBOPhotosURL + text,
                    PhotoID = num
                };
            }

            return null;
        }

        private static int Add(int companyID, string fileName, string originalFileName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.globalairConnectionString))
            {
                sqlConnection.Open();
                using SqlCommand sqlCommand = new SqlCommand("dbo.services_Photos_AddPhoto", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CompanyID", companyID);
                sqlCommand.Parameters.AddWithValue("@PhotoFilename", fileName);
                sqlCommand.Parameters.AddWithValue("@OriginalFileName", originalFileName);
                using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return sqlDataReader.GetInt32(0);
                }
            }

            return -1;
        }

        public static void Update(int photoID, int companyID, string fileName, int order)
        {
            using SqlConnection sqlConnection = new SqlConnection(_appSettings.globalairConnectionString);
            sqlConnection.Open();
            using (SqlCommand sqlCommand = new SqlCommand("services_Photos_UpdatePhoto", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PhotoID", photoID);
                sqlCommand.Parameters.AddWithValue("@CompanyID", companyID);
                sqlCommand.Parameters.AddWithValue("@PhotoFilename", fileName);
                sqlCommand.Parameters.AddWithValue("@FBOOrder", order);
                sqlCommand.ExecuteNonQuery();
            }

            sqlConnection.Close();
        }

        public void Delete(int photoID)
        {
            using SqlConnection sqlConnection = new SqlConnection(_appSettings.globalairConnectionString);
            sqlConnection.Open();
            using (SqlCommand sqlCommand = new SqlCommand("services_Photos_Delete_Photo", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PhotoID", photoID);
                sqlCommand.ExecuteNonQuery();
            }

            sqlConnection.Close();
        }

        public int GetPhotoID(int companyID, string fileName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.globalairConnectionString))
            {
                sqlConnection.Open();
                using SqlCommand sqlCommand = new SqlCommand("dbo.services_Photos_GetPhotoID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CompanyID", companyID);
                sqlCommand.Parameters.AddWithValue("@PhotoFilename", fileName);
                using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return Convert.ToInt32(sqlDataReader["PhotoID"].ToString());
                }
            }

            return -1;
        }

        public List<services_Photos> Get(int companyId)
        {
            var a = _appSettings.FBOPhotosFolder;

            using globalairDBO globalairDBO = new globalairDBO();
            var list = (from sp in globalairDBO.services_Photos
                        join sc in globalairDBO.services_Company on sp.CompanyID_FK equals sc.CompanyID
                        join aa in globalairDBO.airports_Airport on sc.FacID_FK equals aa.FacID
                        join fl2 in globalairDBO.fbos_levels on sc.CompanyID equals fl2.CompanyID
                        where sp.CompanyID_FK == (int?)companyId && fl2.FBOLevel != "Basic" && fl2.FBOLevel != "Silver"
                        select new
                        {
                            APTCode = aa.APTCode,
                            Company = sc.CompName,
                            Photo = sp.Photo,
                            PhotoID = sp.PhotoID,
                            FBOOrder = sp.FBOOrder,
                            OrginalFileName = sp.OrginalFileName
                        }).ToList();
            List<services_Photos> ret = new List<services_Photos>();
            list.ForEach(m =>
            {
                ret.Add(new services_Photos
                {
                    APTCode = m.APTCode,
                    Company = m.Company,
                    Photo = _appSettings.FBOPhotosURL + m.Photo,
                    PhotoID = m.PhotoID,
                    FBOOrder = m.FBOOrder,
                    OrginalFileName = m.OrginalFileName
                });
            });
            return ret;
        }
        public async Task<byte[]> GetBytes(IFormFile formFile)
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
    }
}
