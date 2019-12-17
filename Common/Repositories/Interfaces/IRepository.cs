using System.Collections.Generic;

namespace Common.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        int Save(T entity);

        void Delete(int id);

        void Delete(T entity);

        List<T> Find(T filterEntity);
        List<T> Find(T filterEntity, string executionName);

        List<T> GetAll();

        void Execute(T filterEntity, string executionName);
        void ExecuteQuery(string query, string dbName);

        int ExecuteScalarScript(string query, string dbName);
    }
}
