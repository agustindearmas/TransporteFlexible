﻿using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Shared
{
    public class EmailManager : CheckDigit<Email>, IManagerCrud<Email>
    {
        private readonly IRepository<Email> _Repository;
        private readonly LogManager _bitacoraMgr;

        public EmailManager()
        {
            _Repository = new Repository<Email>();
            _bitacoraMgr = new LogManager();
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarEmail", "Se produjo una excepción salvando un Email. Exception: " + e.Message, 1); // 1 User sistema
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

        /// <summary>
        /// Devuelve un booleano indicando TRUE si existe el Email en la BD y FALSE si lo contrario
        /// </summary>
        /// <param name="emailSinEncriptar">El email a buscar en la base de datos</param>
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

        public Message ExistEmail(string emailSinEncriptar, int emailId)
        {
            try
            {
                string emailEncriptado = CryptManager.EncryptAES(emailSinEncriptar);
                Email emailBD = Retrieve(new Email { EmailAddress = emailEncriptado }).FirstOrDefault();

                return (emailBD != null && emailBD.Id > 0 && emailBD.Id != emailId) ?
                    MessageFactory.GetMessage("MS02")
                    : MessageFactory.GetOKMessage();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message SaveEmail(int emailId, string plainEmail, int loggedUser = 1) // Si no se pasa el loggedUser se guarda user 1 
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
                    return MessageFactory.GetOKMessage(emailBD);
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "Actualizar Email", "Se produjo una excepción actualizando un Email. Exception: " + e.Message, loggedUser);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message ValidateEmailAccount(string encryptedEmailId)
        {
            try
            {
                bool parseFlag = int.TryParse(CryptManager.DecryptAES(encryptedEmailId), out int emailId);
                
                if (parseFlag && emailId != 0)
                {
                    Email email = new Email { Id = emailId };
                    email = Retrieve(email).FirstOrDefault();
                    if (email != null)
                    {
                        email.Habilitado = true;
                        int saveFlag = Save(email);
                        if (saveFlag == emailId)
                        {
                            _bitacoraMgr.Create(LogCriticality.Medium, "ValidarEmail", "Se activo el User con EmailId: " + emailId.ToString() + "en el Sistema", 1);
                            return MessageFactory.GetMessage("MS63", ViewsEnum.Ingreso.GD());
                            
                        }
                        else
                        {
                            throw new Exception("Error al ejecutar metodo save en Email Manager");
                        }
                        
                    }
                }
                _bitacoraMgr.Create(LogCriticality.Alta, "ValidarEmail", "Se intentó activar un usuario con un Id null o inexistente", 1);
                return MessageFactory.GetMessage("MS69");

            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "ValidarEmail", "Se produjo una excepción actualizando un Email. Exception: " + e.Message, 1);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
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
                entity.Habilitado.ToString(),
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
