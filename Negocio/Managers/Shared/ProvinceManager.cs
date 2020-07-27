using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using System.Collections.Generic;

namespace Negocio.Managers.Shared
{
    public class ProvinceManager
    {
        private readonly IRepository<Province> _Repository;
        public ProvinceManager()    
        {
            _Repository = new Repository<Province>();
        }

        public List<Province> Retrieve(Province filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

    }
}
