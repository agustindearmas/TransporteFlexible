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
    public class AddressManager : CheckDigit<Address>, IManagerCrud<Address>
    {
        private readonly IRepository<Address> _Repository;
        private readonly LogManager _bitacoraMgr;
        public AddressManager()
        {
            _Repository = new Repository<Address>();
            _bitacoraMgr = new LogManager();
        }

        public List<Address> Retrieve(Address filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Address entity)
        {
            _Repository.Delete(entity);
        }

        public int Create(int idProv, int idLoc, string street, int number, string floor, string unit, int loggedUserId)
        {
            try
            {
                Address address = new Address
                {
                    Province = new Province { Id = idProv },
                    Location = new Location { Id = idLoc },
                    Street = street,
                    Number = number,
                    Floor = floor,
                    Unit = unit,
                    UsuarioCreacion = loggedUserId,
                    UsuarioModificacion = loggedUserId,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    DVH = 0
                };
                CryptFields(address);
                return Save(address);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public int Create(Address address, int loggedUserId)
        {
            try
            {
                address.UsuarioCreacion = loggedUserId;
                address.UsuarioModificacion = loggedUserId;
                address.FechaCreacion = DateTime.UtcNow;
                address.FechaModificacion = DateTime.UtcNow;
                address.DVH = 0;
                CryptFields(address);
                return Save(address);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void CryptFields(Address address)
        {
            address.Street = CryptManager.EncryptAES(address.Street);
            address.Floor = !string.IsNullOrWhiteSpace(address.Floor) ? CryptManager.EncryptAES(address.Floor) : address.Floor;
            address.Unit =  !string.IsNullOrWhiteSpace(address.Unit) ? CryptManager.EncryptAES(address.Unit) : address.Unit;
        }

        private void DecryptFields(Address address)
        {
            address.Street = CryptManager.DecryptAES(address.Street);
            address.Floor = !string.IsNullOrWhiteSpace(address.Floor) ? CryptManager.DecryptAES(address.Floor) : address.Floor;
            address.Unit = !string.IsNullOrWhiteSpace(address.Unit) ? CryptManager.DecryptAES(address.Unit) : address.Unit;
        }

        public void DecryptFields(List<Address> address)
        {
            foreach (var add in address)
            {
                DecryptFields(add);
            }
        }

        public int Save(Address entity)
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "Guardar Direccion", "Se produjo una excepción salvando una Direccion. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override void AplicarIntegridadRegistro(Address entity)
        {
            Address address = Retrieve(entity).First();
            address.DVH = CalculateRegistryIntegrity(address);
            _Repository.Save(address);
        }

        protected override string ConcatenarPropiedadesDelObjeto(Address entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Street,
                entity.Number.ToString(),
                entity.Floor,
                entity.Unit,
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

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Address> addresses = Retrieve(null);
                foreach (Address address in addresses)
                {
                    Save(address);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Actualiza una direccion en la base de datos
        /// </summary>
        /// <param name="address">la direccion a ser actualizada</param>
        /// <param name="loggedUserId">El id del usuario que dispara la actualizacion</param>
        /// <returns> Devuelve un mensaje indicando el resultado de la operación</returns>
        public Message SaveAddress(Address address, int loggedUserId)
        {
            try
            {
                Address addressBD = new Address { Id = address.Id };
                addressBD = Retrieve(addressBD).FirstOrDefault();

                if (addressBD != null && addressBD.Id == address.Id)
                {
                    addressBD.Location = address.Location;
                    addressBD.Number = address.Number;
                    addressBD.Province = address.Province;
                    addressBD.Street = address.Street;
                    addressBD.Unit = address.Unit;
                    addressBD.Floor = address.Floor;
                    addressBD.UsuarioModificacion = loggedUserId;
                    addressBD.FechaModificacion = DateTime.UtcNow;
                    CryptFields(addressBD);

                    int saveFlag = Save(addressBD);
                    if (saveFlag == addressBD.Id)
                    {
                        DecryptFields(addressBD);
                        return MessageFactory.GetOKMessage(addressBD);
                    }
                    else
                    {
                        throw new Exception("Error al ejecutar metodo save en Address Manager");
                    }
                }
                else
                {
                    return MessageFactory.GetMessage("MS74");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "SaveAddress", "Se produjo una excepción guardando una direccion a una persona. Exception: "
                        + e.Message, loggedUserId); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }
    }
}
