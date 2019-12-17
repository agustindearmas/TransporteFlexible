using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using DataAccess.Concrete;
using System;
using System.Collections.Generic;

namespace Negocio.Managers.Seguridad
{
    public class CriticidadBitacoraManager : IManagerCrud<NivelCriticidad>
    {
        private readonly IRepository<NivelCriticidad> _Repository;

        public CriticidadBitacoraManager()
        {
            _Repository = new Repository<NivelCriticidad>();
        }
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(NivelCriticidad entity)
        {
            throw new NotImplementedException();
        }

        public List<NivelCriticidad> Retrieve(NivelCriticidad filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public int Save(NivelCriticidad entity)
        {
            throw new NotImplementedException();
        }
    }
}
