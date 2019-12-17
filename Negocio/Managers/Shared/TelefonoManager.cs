using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Shared
{
    public class TelefonoManager : IManagerCrud<Telefono>
    {
        private readonly IRepository<Telefono> _Repository;
        private readonly BitacoraManager _bitacoraMgr;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Shared.Telefono";
        public TelefonoManager()
        {
            _Repository = new Repository<Telefono>();
            _digitoVerificadorMgr = new TablaDVVManager();
            _bitacoraMgr = new BitacoraManager();
        }
        public int Create(string telefono, int usuarioCreacion)
        {
            Telefono tel = new Telefono
            {
                NumeroTelefono = EncriptacionManager.EncriptarAES(telefono),
                UsuarioCreacion = usuarioCreacion,
                UsuarioModificacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow,
                DVH = 0
            };
            return Save(tel);
        }
        public int Save(Telefono entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                GenerarEImpactarDVH(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarTelefono", "Se produjo una excepción salvando un Telefono. Exception: " + e.Message, 1); // 1 Usuario sistema
                throw e;
            }
        }
        public void Delete(int id)
        {
            _Repository.Delete(id);
        }
        public void Delete(Telefono entity)
        {
            _Repository.Delete(entity);
        }
        public List<Telefono> Retrieve(Telefono filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        #region Metodos Privados
        private void GenerarEImpactarDVH(Telefono entity)
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
        private string ConcatenarDVH(Telefono entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.NumeroTelefono,
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
            List<Telefono> telefonos = Retrieve(new Telefono());
            TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
            int acumulador = 0;
            foreach (Telefono telefono in telefonos)
            {
                string cadena = ConcatenarDVH(telefono);
                Save(telefono);
                acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
            }
            return acumulador;
        }
        #endregion
    }
}
