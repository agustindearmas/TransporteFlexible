﻿using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class PermisoManager : DigitoVerificador<Permiso>, IManagerCrud<Permiso>
    {
        private readonly IRepository<Permiso> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public PermisoManager()
        {
            _Repository = new Repository<Permiso>();
            _bitacoraMgr = new BitacoraManager();
        }

        public List<Permiso> Retrieve(Permiso filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public int Save(Permiso entity)
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarUsuario", "Se produjo una excepción salvando un usuario. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch{}
                throw e;
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Permiso entity)
        {
            throw new NotImplementedException();
        }        

        public List<Permiso> ObtenerPermisosPorRolId(int idRol)
        {
            try
            {
                RolManager _rolMgr = new RolManager();
                Rol rol = new Rol { Id = idRol };
                rol = _rolMgr.Retrieve(rol).First();
                return rol.Permisos;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<int> ObtenerSoloIdsDePermisos(List<Permiso> permisos)
        {
            try
            {
                List<int> idPermisos = new List<int>();
                foreach (var permiso in permisos)
                {
                    idPermisos.Add(permiso.Id);
                }
                return idPermisos;
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }


        #region Digito Verificador

        public override void ValidarIntegridadRegistros()
        {
            ValidarIntegridad(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Permiso entity)
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

        protected override void AplicarIntegridadRegistro(Permiso entity)
        {
            Permiso permiso = Retrieve(entity).First();
            permiso.DVH = CalcularIntegridadRegistro(permiso);
            _Repository.Save(permiso);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Permiso> permisos = Retrieve(null);
                foreach (Permiso permiso in permisos)
                {
                    Save(permiso);
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
