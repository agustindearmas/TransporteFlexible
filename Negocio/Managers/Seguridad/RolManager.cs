using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using DataAccess.Concrete;

namespace Negocio.Managers.Seguridad
{
    public class RolManager : IManagerCrud<Rol>
    {
        private readonly IRepository<Rol> _Repository;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly BitacoraManager _bitacoraMgr;
        private readonly string _table = "Seguridad.Rol";
        public RolManager()
        {
            _Repository = new Repository<Rol>();
            _digitoVerificadorMgr = new TablaDVVManager();
            _bitacoraMgr = new BitacoraManager();
        }

        public int Save(Rol entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                GenerarEImpactarDVH(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarRol", "Se produjo una excepción salvando un Rol. Exception: " + e.Message, 1); // 1 Usuario sistema
                return 0;
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

        public Rol GetOne(int rolId)
        {
           return Retrieve(new Rol { Id = rolId }).First();
        }

        #region Metodos Privados
        private void GenerarEImpactarDVH(Rol entity)
        {
            try
            {
                entity = Retrieve(entity).First();
                entity.DVH = _digitoVerificadorMgr.CalcularImpactarDVH_DVV(ConcatenarDVH(entity), _table);
                _Repository.Save(entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string ConcatenarDVH(Rol entity)
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

        public int RecalcularDVH_DVV()
        {
            List<Rol> roles = Retrieve(new Rol());
            TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
            int acumulador = 0;
            foreach (Rol rol in roles)
            {
                string cadena = ConcatenarDVH(rol);                
                Save(rol);
                acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
            }
            return acumulador;
        }
        #endregion
    }
}
