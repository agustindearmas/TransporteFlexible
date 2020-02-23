using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using Negocio.Managers.Seguridad;

namespace Negocio.Managers.Shared
{
    public class PersonaManager : DigitoVerificador<Persona>, IManagerCrud<Persona>
    {
        private readonly IRepository<Persona> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public PersonaManager()
        {
            _Repository = new Repository<Persona>();
            _bitacoraMgr = new BitacoraManager();
        }

        public int Save(Persona entity)
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarEmail", "Se produjo una excepción salvando un Email. Exception: "
                        + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
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
            try
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
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    DVH = 0
                };
                return Save(persona);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ComprobarExistenciaPersona(string dniSinEncriptar)
        {
            try
            {
                string dniEncriptado = EncriptacionManager.EncriptarAES(dniSinEncriptar);
                Persona personaBD = Retrieve(new Persona { DNI = dniEncriptado }).FirstOrDefault();
                return (personaBD != null && personaBD.Id > 0);// DEVUELVO TRUE SI EXISTE LA PERSONA EN LA BASE DE DATOS.
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public List<Persona> Retrieve(Persona filter = null)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        #region Digito Verificador
        public override void ValidarIntegridadRegistros()
        {
            ValidarIntegridad(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Persona entity)
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

        protected override void AplicarIntegridadRegistro(Persona entity)
        {
            Persona persona = Retrieve(entity).First();
            persona.DVH = CalcularIntegridadRegistro(persona);
            _Repository.Save(persona);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Persona> personas = Retrieve(null);
                foreach (Persona persona in personas)
                {
                    Save(persona);
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
