using System.Collections.Generic;

namespace Common.Interfaces.Shared
{
    public interface IManagerCrud<T>
    {
        int Save(T entity);
        void Delete(int id);
        void Delete(T entity);
        List<T> Retrieve(T filter);
    }
}
