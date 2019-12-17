using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.Managers.Seguridad;

namespace Negocio.Managers.Shared
{
    public class PersonaManager : IManagerCrud<Persona>
    {
        private readonly IRepository<Persona> _Repository;
        private readonly BitacoraManager _bitacoraMgr;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Shared.Persona";

        public PersonaManager()
        {
            _Repository = new Repository<Persona>();
            _digitoVerificadorMgr = new TablaDVVManager();
            _bitacoraMgr = new BitacoraManager();
        }

        public int Save(Persona entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                GenerarEImpactarDVH(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarEmail", "Se produjo una excepción salvando un Email. Exception: " + e.Message, 1); // 1 Usuario sistema
                return 0;
            }
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Persona entity)
        {
            _Repository.Delete(entity);
        }

        public int Create(string nombreSinEncriptar, string apellidoSinEncriptar, string cuilSinEncriptar, 
            string dniSinEncriptar, bool esCuit, List<Email> emails, List<Telefono> telefonos, int usuarioCreacion)
        {
            Persona persona = new Persona
            {
                Nombre = EncriptacionManager.EncriptarAES(nombreSinEncriptar),
                Apellido = EncriptacionManager.EncriptarAES(apellidoSinEncriptar),
                DNI = EncriptacionManager.EncriptarAES(dniSinEncriptar),
                NumeroCuil = EncriptacionManager.EncriptarAES(cuilSinEncriptar),
                EsCuit = esCuit,
                Baja = false,
                Emails = emails,
                Telefonos = telefonos,
                UsuarioCreacion = usuarioCreacion,
                UsuarioModificacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow,
                DVH = 0
            };
            return Save(persona);
        }

        public bool ComprobarExistenciaPersona(string dniSinEncriptar)
        {
            string dniEncriptado = EncriptacionManager.EncriptarAES(dniSinEncriptar);
            Persona personaBD = Retrieve(new Persona { DNI = dniEncriptado }).FirstOrDefault();
            return (personaBD != null && personaBD.Id > 0);// DEVUELVO TRUE SI EXISTE LA PERSONA EN LA BASE DE DATOS.
        }

        public List<Persona> Retrieve(Persona filter = null)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        #region Metodos Privados
        private void GenerarEImpactarDVH(Persona entity)
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

        private string ConcatenarDVH(Persona entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Nombre,
                entity.Apellido,
                entity.EsCuit.ToString(),
                entity.NumeroCuil,
                entity.DNI,
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
            List<Persona> personas = Retrieve(new Persona());
            TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
            int acumulador = 0;
            foreach (Persona persona in personas)
            {
                string cadena = ConcatenarDVH(persona);                
                Save(persona);
                acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
            }
            return acumulador;
        }
        #endregion
    }
}
