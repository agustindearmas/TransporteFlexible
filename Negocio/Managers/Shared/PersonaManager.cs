using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;
using Negocio.Managers.Seguridad;

namespace Negocio.Managers.Shared
{
    public class PersonManager : CheckDigit<Persona>, IManagerCrud<Persona>
    {
        private readonly IRepository<Persona> _Repository;
        private readonly LogManager _bitacoraMgr;

        public PersonManager()
        {
            _Repository = new Repository<Persona>();
            _bitacoraMgr = new LogManager();
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarPersona", "Se produjo una excepción salvando una Persona. Exception: "
                        + e.Message, 1); // 1 User sistema
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
                    Nombre = nombreSinEncriptar,
                    Apellido = apellidoSinEncriptar,
                    DNI = dniSinEncriptar,
                    NumeroCuil = cuilSinEncriptar,
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
                CryptFields(persona);
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

        public Message DeleteAddress(int addressIdInt, int sessionUserId, int sessionPeopleId)
        {
            throw new NotImplementedException();
        }

        public List<Persona> Retrieve(Persona filter = null)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        /// <summary>
        /// Obtiene una persona con todos sus campos desencriptados, y los campos de tipos complejos llenos y desencriptados de ser necesario.
        /// </summary>
        /// <param name="personId">Es el la identificacion unica de una persona</param>
        /// <returns>Una persona completa y desencriptada si no encuentra nada con el ID que se pasa devuelve NULL</returns>
        public Persona GetPersonFull(int personId)
        {
            try
            {
                if (personId != 0)
                {
                    List<Persona> personas = Retrieve(new Persona { Id = personId });
                    if (personas.Count == 1)
                    {
                        Persona persona = personas.Single();
                        DecryptFields(persona);

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

                        if (persona.Addresses != null)
                        {
                            AddressManager _addressMgr = new AddressManager();
                            _addressMgr.DecryptFields(persona.Addresses);
                        }

                        return persona;
                    }
                    else if (personas.Count != 0)
                    {
                        throw new Exception("Hay mas de una persona con el mismo Id");
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "ObteniendoPersona", "Se produjo una excepción obteniendo una Persona. Exception: "
                        + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        /// <summary>
        /// Agrega una nueva direccion a una persona
        /// </summary>
        /// <param name="address"> La direccion a salvar</param>
        /// <param name="loggedUserId"> El id del usuario loggeado</param>
        /// <param name="editingPeopleId"> el id de la persona a agregarse la nueva direccion</param>
        /// <returns></returns>
        public Message AddAddress(Address address, int loggedUserId, int editingPeopleId)
        {
            try
            {
                Persona person = GetPersonFull(editingPeopleId);
                if (person != null)
                {
                    AddressManager _addressMgr = new AddressManager();
                    int addressId = _addressMgr.Create(address, loggedUserId);

                    if (person.Addresses != null)
                    {
                        person.Addresses.Add(new Address { Id = addressId });
                    }
                    else
                    {
                        person.Addresses = new List<Address>
                        {
                            new Address { Id = addressId }
                        };
                    }
                    
                    
                    CryptFields(person);
                    int peopleSave = Save(person);
                    if (peopleSave == editingPeopleId)
                    {
                        return MessageFactory.GetOKMessage();
                    }
                    else
                    {
                        throw new Exception("Error al ejecutar metodo save en Person Manager");
                    }

                }
                return MessageFactory.GetMessage("MS71");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "AddAddress", "Se produjo una excepción agregando una direccion a una persona. Exception: "
                        + e.Message, loggedUserId); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        /// <summary>
        /// Agrega un nuevo telefono a la persona
        /// </summary>
        /// <param name="phoneNumber"> El telefono a guardar</param>
        /// <param name="loggedUserId">El id del usuario que ejecuta la accion</param>
        /// <param name="editingPeopleId">El id de la persona que se le asignara el telfono</param>
        /// <returns>Un mensaje indicando el resultado de la operacion</returns>
        public Message AddPhone(string phoneNumber, int loggedUserId, int editingPeopleId)
        {
            try
            {
                Persona person = GetPersonFull(editingPeopleId);
                if (person != null)
                {
                    PhoneManager _phoneMgr = new PhoneManager();
                   
                    int phoneId = _phoneMgr.Create(phoneNumber, loggedUserId);

                    person.Telefonos.Add(new Telefono { Id = phoneId });
                    CryptFields(person);
                    int peopleSave = Save(person);
                    if (peopleSave == editingPeopleId)
                    {
                        return MessageFactory.GetOKMessage();
                    }
                    else
                    {
                        throw new Exception("Error al ejecutar metodo save en Person Manager");
                    }
                  
                }
                return MessageFactory.GetMessage("MS71");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "AddPhone", "Se produjo una excepción agregando un telefono a una persona. Exception: "
                        + e.Message, loggedUserId); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        /// <summary>
        /// Elimina un telefono de una persona, siempre y cuando este no sea el único que le quede
        /// </summary>
        /// <param name="phoneId">Es el id del telefono a eliminar</param>
        /// <param name="loggedUserId">Es el id del usuario loggueado ejecutando la accion</param>
        /// <param name="personId">Es el Id de la persona que se eliminar el telefono</param>
        /// <returns>Un mensaje indicando el resultado de la operacion</returns>
        public Message DeletePhone(int phoneId, int loggedUserId, int personId)
        {
            try
            {
                Persona person = GetPersonFull(personId);
                if (person != null)
                {
                    person.Telefonos.RemoveAll(x => x.Id == phoneId);
                    if (person.Telefonos.Count > 0)
                    {
                        PhoneManager _phoneMgr = new PhoneManager();
                        _phoneMgr.Delete(phoneId);
                        _bitacoraMgr.Create(LogCriticality.Baja, "Eliminar Telefono Persona", "Se elimino un email con Id: " + phoneId.ToString() + "que era de la persona con Id: " + personId.ToString(), loggedUserId);
                        return MessageFactory.GetOKMessage();
                    }
                    else
                    {
                        _bitacoraMgr.Create(LogCriticality.Baja, "Eliminar Telefono Persona", "Se intento eliminar un Telefono pero era el único asignado al usuario", loggedUserId);
                        return MessageFactory.GetMessage("MS73");
                    }
                }
                return MessageFactory.GetMessage("MS71");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Eliminar Telefono Persona", "Se produjo una excepción eliminando un Telefono. Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        /// <summary>
        /// Elimina el email de la persona validando q este no sea su unico email o que le quede algun email habilitado
        /// </summary>
        /// <param name="emailId">El id del email a borrar</param>
        /// <param name="loggedUserId">el Id del ususario loggeado en el sistema</param>
        /// <param name="personId">El id de la persona a la cual se le queire eliminar el email</param>
        /// <returns></returns>
        public Message DeleteEmail(int emailId, int loggedUserId, int personId)
        {
            try
            {
                Persona person = GetPersonFull(personId);
                if (person != null)
                {
                    person.Emails.RemoveAll(x => x.Id == emailId);
                    if (person.Emails.Select(e => e.Habilitado).ToList().Count > 0)
                    {
                        EmailManager _emailMgr = new EmailManager();
                        _emailMgr.Delete(emailId);
                        _bitacoraMgr.Create(LogCriticality.Baja, "Eliminar Email Persona", "Se elimino un email con Id: " + emailId.ToString() + "que era de la persona con Id: " + personId.ToString(), loggedUserId);
                        return MessageFactory.GetOKMessage();
                    }
                    else
                    {
                        _bitacoraMgr.Create(LogCriticality.Baja, "Eliminar Email Persona", "Se intento eliminar un email pero era el único asignado al usuario o el único habilitado", loggedUserId);
                        return MessageFactory.GetMessage("MS70");
                    }
                }
                return MessageFactory.GetMessage("MS71");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Eliminar Email Persona", "Se produjo una excepción eliminando un Email. Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        /// <summary>
        /// Agrega un Email nuevo a una persona
        /// </summary>
        /// <param name="loggedUserId">Es el UserId del Usuario loggeado</param>
        /// <param name="personId">Es el id de la persona a agregar el nuevo email</param>
        /// <returns>Un mensaje indicado el resultado de la operación</returns>
        public Message AddEmail(string email, int loggedUserId, int personId)
        {
            try
            {
                Persona person = GetPersonFull(personId);
                if (person != null)
                {
                    EmailManager _emailMgr = new EmailManager();
                    if (!_emailMgr.ExistEmail(email))
                    {
                        int emailId = _emailMgr.Create(email, loggedUserId);

                        person.Emails.Add(new Email { Id = emailId });
                        CryptFields(person);
                        int peopleSave = Save(person);
                        if (peopleSave == personId)
                        {
                            SendEmailManager _sendEmailMgr = new SendEmailManager();
                            _sendEmailMgr.SendValidationEmail(email, CryptManager.EncryptAES(emailId.ToString()));
                            return MessageFactory.GetOKMessage();
                        }
                        else
                        {
                            throw new Exception("Error al ejecutar metodo save en Person Manager");
                        }
                    }
                    else
                    {
                        return MessageFactory.GetMessage("MS02");
                    }
                }
                return MessageFactory.GetMessage("MS71");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "AddEmail", "Se produjo una excepción agregando un email a un usuario. Exception: "
                        + e.Message, loggedUserId); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        private void CryptFields(Persona person)
        {
            person.Nombre = CryptManager.EncryptAES(person.Nombre);
            person.Apellido = CryptManager.EncryptAES(person.Apellido);
            person.DNI = CryptManager.EncryptAES(person.DNI);
            person.NumeroCuil = CryptManager.EncryptAES(person.NumeroCuil);
        }

        private void DecryptFields(Persona person)
        {
            person.Nombre = CryptManager.DecryptAES(person.Nombre);
            person.Apellido = CryptManager.DecryptAES(person.Apellido);
            person.DNI = CryptManager.DecryptAES(person.DNI);
            person.NumeroCuil = CryptManager.DecryptAES(person.NumeroCuil);
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
