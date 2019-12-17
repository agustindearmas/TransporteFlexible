using DataAccess.Interfaces;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DataAccess.Concrete
{
    public class DataBase : IDataBase
    {
        private SqlConnection GetConnection()
        {
            var parameters = System.Web.HttpContext.Current.Request.Headers["IP"];

            var connectionString = string.Format(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, System.Web.HttpContext.Current.User.Identity.Name.ToString() + "|" + parameters);

            return new SqlConnection(connectionString);
        }

        public void ExecuteSPWithResultSet(string storeProcedureName, Action<DbDataReader> action, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                using (var reader = cmd.ExecuteReader())
                {
                    action(reader);
                }
            }
            conn.Dispose();
        }

        public int ExecuteSPNonQuery(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                var ouputParameter = cmd.Parameters["@RETURN_VALUE"];

                cmd.ExecuteNonQuery();

                conn.Dispose();

                if ((int)ouputParameter.Value == -1)
                {
                    throw new Exception(string.Format("Error al ejecutarse el SP: {0}", storeProcedureName));
                }
                else
                {
                    return (int)ouputParameter.Value;
                }
            }

        }

        public bool ExecuteScriptNonQuery(string script, string @base = "")
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                if (!string.IsNullOrEmpty(@base))
                {
                    cmd.Connection.ChangeDatabase(@base);
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = script;

                cmd.ExecuteNonQuery();

                conn.Dispose();

                return true;
            }

        }

        public int ExecuteScalarScript(string script, string @base = "")
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                if (!string.IsNullOrEmpty(@base))
                {
                    cmd.Connection.ChangeDatabase(@base);
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = script;

                int sum = (int)cmd.ExecuteScalar();

                conn.Dispose();

                return sum;
            }
        }

        public string ExecuteSPScalar(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                var res = cmd.ExecuteScalar().ToString();

                conn.Dispose();

                return res;
            }

        }
    }
}
