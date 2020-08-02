using Common.Enums.Seguridad;
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
    public class PhoneManager : CheckDigit<Telefono>, IManagerCrud<Telefono>
    {
        private readonly IRepository<Telefono> _Repository;
        private readonly LogManager _bitacoraMgr;

        public PhoneManager()
        {
            _Repository = new Repository<Telefono>();
            _bitacoraMgr = new LogManager();
        }
        public int Create(string telefono, int usuarioCreacion)
        {
            try
            {
                Telefono tel = new Telefono
                {
                    NumeroTelefono = CryptManager.EncryptAES(telefono),
                    UsuarioCreacion = usuarioCreacion,
                    UsuarioModificacion = usuarioCreacion,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    DVH = 0
                };
                return Save(tel);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public int Save(Telefono entity)
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarTelefono", "Se produjo una excepción salvando un Telefono. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch {}
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

        public Message SavePhone(int phoneId, string phoneNumber, int loggedUserId)
        {
            try
            {
                Telefono phone = new Telefono { Id = phoneId };
                phone = Retrieve(phone).FirstOrDefault();
                if (phone != null && phone.Id == phoneId)
                {
                    phone.NumeroTelefono = CryptManager.EncryptAES(phoneNumber);
                    phone.UsuarioModificacion = loggedUserId;
                    phone.FechaModificacion = DateTime.UtcNow;
                    int saveFlag = Save(phone);
                    if (saveFlag == phoneId)
                    {
                        phone.NumeroTelefono = phoneNumber;
                        return MessageFactory.GetOKMessage(phone);
                    }
                    else
                    {
                        throw new Exception("Error al ejecutar metodo save en Telefono Manager");
                    }
                }
                else
                {
                    return MessageFactory.GetMessage("MS72");
                }

            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Actualizar Telefono", "Se produjo una excepción actualizando un Telefono. Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        #region Digito Verificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Telefono entity)
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

        protected override void AplicarIntegridadRegistro(Telefono entity)
        {
            Telefono tel = Retrieve(entity).First();
            tel.DVH = CalculateRegistryIntegrity(tel);
            _Repository.Save(tel);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Telefono> telefonos = Retrieve(null);
                foreach (Telefono telefono in telefonos)
                {
                    Save(telefono);
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
