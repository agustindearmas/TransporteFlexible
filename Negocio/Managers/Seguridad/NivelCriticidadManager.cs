﻿using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using System;
using System.Collections.Generic;

namespace Negocio.Managers.Seguridad
{
    public class NivelCriticidadManager : DigitoVerificador<NivelCriticidad>, IManagerCrud<NivelCriticidad>
    {
        private readonly IRepository<NivelCriticidad> _Repository;
        public NivelCriticidadManager()
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

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<NivelCriticidad> ncs = Retrieve(null);
                foreach (NivelCriticidad nc in ncs)
                {
                    Save(nc);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<NivelCriticidad> Retrieve(NivelCriticidad filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public int Save(NivelCriticidad entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                AplicarIntegridadRegistro(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                try
                {
                    BitacoraManager _bitacoraMgr = new BitacoraManager();
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarNivelCriticidad", "Se produjo una excepción salvando NC. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        public override void ValidarIntegridadRegistros()
        {
            ValidarIntegridad(Retrieve(null));
        }

        protected override void AplicarIntegridadRegistro(NivelCriticidad entity)
        {
            throw new NotImplementedException();
        }

        protected override string ConcatenarPropiedadesDelObjeto(NivelCriticidad entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Descripcion);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
