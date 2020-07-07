using Common.Enums.Seguridad;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Shared
{
    public class EmailManager : CheckDigit<Email>, IManagerCrud<Email>
    {
        private readonly IRepository<Email> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public EmailManager()
        {
            _Repository = new Repository<Email>();
            _bitacoraMgr = new BitacoraManager();
        }
        public int Save(Email entity)
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarEmail", "Se produjo una excepción salvando un Email. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Email entity)
        {
            _Repository.Delete(entity);
        }

        public List<Email> Retrieve(Email filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public int Create(string emailSinEcnriptar, int usuarioCreacion)
        {
            try
            {
                Email email = new Email
                {
                    EmailAddress = CryptManager.EncryptAES(emailSinEcnriptar),
                    UsuarioCreacion = usuarioCreacion,
                    UsuarioModificacion = usuarioCreacion,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    DVH = 0
                };
                return Save(email);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ExistEmail(string emailSinEncriptar)
        {
            try
            {
                string emailEncriptado = CryptManager.EncryptAES(emailSinEncriptar);
                Email emailBD = Retrieve(new Email { EmailAddress = emailEncriptado }).FirstOrDefault();
                return (emailBD != null && emailBD.Id > 0); //DEVUELVE TRUE SI ENCUENTRA ALGO EN LA BASE
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Mensaje ExistEmail(string emailSinEncriptar, int emailId)
        {
            try
            {
                string emailEncriptado = CryptManager.EncryptAES(emailSinEncriptar);
                Email emailBD = Retrieve(new Email { EmailAddress = emailEncriptado }).FirstOrDefault();

                return (emailBD != null && emailBD.Id > 0 && emailBD.Id != emailId) ?
                    MessageFactory.CrearMensaje("MS02")
                    : MessageFactory.CrearMensajeOk();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Mensaje UpdateEmail(int emailId, string plainEmail, int loggedUser = 1) // Si no se pasa el loggedUser se guarda user 1 
        {
            try
            {
                string encriptedEmail = CryptManager.EncryptAES(plainEmail);
                Email emailBD = Retrieve(new Email { Id = emailId }).Single();
                emailBD.EmailAddress = encriptedEmail;
                emailBD.Habilitado = false;
                int saveValue = Save(emailBD);
                if (saveValue == emailId)
                {
                    emailBD.EmailAddress = CryptManager.DecryptAES(emailBD.EmailAddress);
                    SendEmailManager _sendEmailMgr = new SendEmailManager();
                    _sendEmailMgr.SendValidationEmail(emailBD.EmailAddress, CryptManager.EncryptAES(emailBD.Id.ToString()));
                    return MessageFactory.GetOkMessage(emailBD);
                }
                else
                {
                    throw new Exception("Error al ejecutar metodo save en Email Manager");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Actualizar Email", "Se produjo una excepción actualizando un Email. Exception: " + e.Message, loggedUser);
                }
                catch {}
                throw;
            }
        }

        #region DigitoVerificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Email entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.EmailAddress,
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

        protected override void AplicarIntegridadRegistro(Email entity)
        {
            Email email = Retrieve(entity).First();
            email.DVH = CalculateRegistryIntegrity(email);
            _Repository.Save(email);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Email> emails = Retrieve(null);
                foreach (Email email in emails)
                {
                    Save(email);
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
