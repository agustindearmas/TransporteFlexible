using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using System.Collections.Generic;

namespace Negocio.Managers.Shared
{
    public class LocationManager
    {
        private readonly IRepository<Location> _Repository;
        public LocationManager()
        {
            _Repository = new Repository<Location>();
        }

        public List<Location> Retrieve(Location filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }
    }
}
