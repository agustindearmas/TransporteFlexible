using Common.Enums.Seguridad;
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
    public class PhoneManager : CheckDigit<Telefono>, IManagerCrud<Telefono>
    {
        private readonly IRepository<Telefono> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public PhoneManager()
        {
            _Repository = new Repository<Telefono>();
            _bitacoraMgr = new BitacoraManager();
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarTelefono", "Se produjo una excepción salvando un Telefono. Exception: " + e.Message, 1); // 1 Usuario sistema
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
