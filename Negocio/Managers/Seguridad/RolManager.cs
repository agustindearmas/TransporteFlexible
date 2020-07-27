using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using DataAccess.Concrete;
using Negocio.CheckDigit;

namespace Negocio.Managers.Seguridad
{
    public class RolManager : CheckDigit<Rol>, IManagerCrud<Rol>
    {
        private readonly IRepository<Rol> _Repository;
        private readonly LogManager _bitacoraMgr;

        public RolManager()
        {
            _Repository = new Repository<Rol>();
            _bitacoraMgr = new LogManager();
        }

        public int Save(Rol entity)
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarRol", "Se produjo una excepción salvando un Rol. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch {}
                throw e;
            }
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }
        public void Delete(Rol entity)
        {
            _Repository.Delete(entity);
        }

        public List<Rol> Retrieve(Rol filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        #region DigitoVerificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Rol entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Descripcion,
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

        protected override void AplicarIntegridadRegistro(Rol entity)
        {
            Rol rol = Retrieve(entity).First();
            rol.DVH = CalculateRegistryIntegrity(rol);
            _Repository.Save(rol);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Rol> roles = Retrieve(null);
                foreach (Rol rol in roles)
                {
                    Save(rol);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
