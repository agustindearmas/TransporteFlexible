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
    public class ConfiguracionManager : IManagerCrud<Configuracion>
    {
        private readonly IRepository<Configuracion> _Repository;

        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Seguridad.Configuracion";
        public ConfiguracionManager()
        {
            _Repository = new Repository<Configuracion>();
            _digitoVerificadorMgr = new TablaDVVManager();
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
                GenerarEImpactarDVH(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                try
                {
                    BitacoraManager _bitacoraMgr = new BitacoraManager();
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarConfiguracion", "Se produjo una excepción salvando una Configuracion. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch{}
                throw e;
            }
        }

        private void GenerarEImpactarDVH(Configuracion entity)
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

        private string ConcatenarDVH(Configuracion entity)
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

        public int RecalcularDVH_DVV()
        {
            try
            {
                List<Configuracion> configuraciones = Retrieve(new Configuracion());
                TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
                int acumulador = 0;
                foreach (Configuracion configuracion in configuraciones)
                {
                    string cadena = ConcatenarDVH(configuracion);
                    Save(configuracion);
                    acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
                }
                return acumulador;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}
