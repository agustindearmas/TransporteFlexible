﻿using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Shared
{
    public class ConfigManager : CheckDigit<Configuracion>, IManagerCrud<Configuracion>
    {
        private readonly IRepository<Configuracion> _Repository;

        public ConfigManager()
        {
            _Repository = new Repository<Configuracion>();
        }
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Configuracion entity)
        {
            throw new NotImplementedException();
        }

        public List<Configuracion> Retrieve(Configuracion filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public int Save(Configuracion entity)
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
                    LogManager _bitacoraMgr = new LogManager();
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarConfiguracion", "Se produjo una excepción salvando una Configuracion. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch{}
                throw e;
            }
        }

        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Configuracion entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Nombre,
                entity.Valor,
                entity.UsuarioCreacion.ToString(),
                entity.FechaCreacion.ToString(),
                entity.UsuarioModificacion.ToString(),
                entity.FechaModificacion.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override void AplicarIntegridadRegistro(Configuracion entity)
        {
            Configuracion config = Retrieve(entity).First();
            config.DVH = CalculateRegistryIntegrity(config);
            _Repository.Save(config);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Configuracion> configuraciones = Retrieve(null);
                foreach (Configuracion config in configuraciones)
                {
                    Save(config);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
