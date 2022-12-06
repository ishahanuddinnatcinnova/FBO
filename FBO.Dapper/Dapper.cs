using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace FBO.Dapper
{
    public class Dapperr
    {
        private string Connectionstring = "Data Source=GlobalSql01;Initial Catalog=globalair;Uid=db_gan;Pwd=gan911brook;";

        public void Dispose()
        {

        }
        public void Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            db.Execute(sp, parms, commandType: commandType);
        }

        public string ExecuteScalar(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            var ratingId =  db.ExecuteScalar(sp, parms, commandType: commandType);

            return ratingId.ToString();
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(Connectionstring);
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }

        public SqlMapper.GridReader GetAllTables<T>(string sp, DynamicParameters parms, IDbConnection db, CommandType commandType = CommandType.StoredProcedure)
        {
            return db.QueryMultiple(sp, parms, commandType: commandType);
        }

        public DbConnection GetDbconnection()
        {
            return new SqlConnection(Connectionstring);
        }


    }
}
