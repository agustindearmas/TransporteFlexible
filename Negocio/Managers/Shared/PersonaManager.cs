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
    public class PersonManager : CheckDigit<Persona>, IManagerCrud<Persona>
    {
        private readonly IRepository<Persona> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public PersonManager()
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarPersona", "Se produjo una excepción salvando una Persona. Exception: "
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
                    Nombre = CryptManager.EncryptAES(nombreSinEncriptar),
                    Apellido = CryptManager.EncryptAES(apellidoSinEncriptar),
                    DNI = CryptManager.EncryptAES(dniSinEncriptar),
                    NumeroCuil = CryptManager.EncryptAES(cuilSinEncriptar),
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
                string dniEncriptado = CryptManager.EncryptAES(dniSinEncriptar);
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

        public Persona ObtenerPersonaDesencriptada(int personaId)
        {
            try
            {
                List<Persona> personas = Retrieve(new Persona { Id = personaId });
                if (personas.Count == 1)
                {
                    Persona persona = personas.Single();
                    CryptManager _encriptacionMgr = new CryptManager();

                    persona.Nombre = CryptManager.DecryptAES(persona.Nombre);
                    persona.Apellido = CryptManager.DecryptAES(persona.Apellido);
                    persona.DNI = CryptManager.DecryptAES(persona.DNI);
                    persona.NumeroCuil = CryptManager.DecryptAES(persona.NumeroCuil);

                    if (persona.Emails != null)
                    {
                        foreach (var email in persona.Emails)
                        {
                            email.EmailAddress = CryptManager.DecryptAES(email.EmailAddress);
                        }
                    }

                    if (persona.Telefonos != null)
                    {
                        foreach (var telefono in persona.Telefonos)
                        {
                            telefono.NumeroTelefono = CryptManager.DecryptAES(telefono.NumeroTelefono);
                        }
                    }

                    return persona;
                }

                if (personas.Count == 0)
                {
                    return null;
                }

                throw new Exception("Hay mas de una persona con el mismo Id");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ObteniendoPersona", "Se produjo una excepción obteniendo una Persona. Exception: "
                        + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        #region Digito Verificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
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
            persona.DVH = CalculateRegistryIntegrity(persona);
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
