using Common.Repositories.Interfaces;
using System.Collections.Generic;

namespace DataAccess.Concrete
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        public int Save(T entity)
        {
            return GenericDAO<T>.Save(entity);
        }

        public void Delete(int id)
        {
            GenericDAO<T>.Delete(id);
        }

        public void Delete(T entity)
        {
            GenericDAO<T>.Delete(entity);
        }

        public List<T> Find(T filterEntity)
        {
            return GenericDAO<T>.Find(filterEntity, null);
        }

        public List<T> Find(T filterEntity, string executionName)
        {
            return GenericDAO<T>.Find(filterEntity, executionName);
        }

        public List<T> GetAll()
        {
            return GenericDAO<T>.GetAll();
        }

        public List<T> Retrieve(T filterEntity = null)
        {
            if (filterEntity == null) return GetAll();

            return Find(filterEntity);
        }

        public void Execute(T filterEntity, string executionName)
        {
            GenericDAO<T>.Execute(filterEntity, executionName);
        }

        public void ExecuteQuery(string query, string dbName)
        {
            GenericDAO<T>.ExecuteQuery(query, dbName);
        }

        public int ExecuteScalarScript(string query, string dbName)
        {
            return GenericDAO<T>.ExecuteScalarQuery(query, dbName);
        }

    }
}
