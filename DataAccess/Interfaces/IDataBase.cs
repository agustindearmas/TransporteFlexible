using System;
using System.Data.Common;

namespace DataAccess.Interfaces
{
    public interface IDataBase
    {
        bool ExecuteScriptNonQuery(string script, string @base = "");
        int ExecuteScalarScript(string script, string @base = "");
        int ExecuteSPNonQuery(string storeProcedureName, Action<DbParameterCollection> fillParameters = null);
        string ExecuteSPScalar(string storeProcedureName, Action<DbParameterCollection> fillParameters = null);
        void ExecuteSPWithResultSet(string storeProcedureName, Action<DbDataReader> action, Action<DbParameterCollection> fillParameters = null);
    }
}
