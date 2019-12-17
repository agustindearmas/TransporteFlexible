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
    public class EmailManager : IManagerCrud<Email>
    {
        private readonly IRepository<Email> _Repository;
        private readonly BitacoraManager _bitacoraMgr;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Shared.Email";

        public EmailManager()
        {
            _Repository = new Repository<Email>();
            _digitoVerificadorMgr = new TablaDVVManager();
            _bitacoraMgr = new BitacoraManager();
        }
        public int Save(Email entity)
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
            Email email = new Email
            {
                EmailAddress = EncriptacionManager.EncriptarAES(emailSinEcnriptar),
                UsuarioCreacion = usuarioCreacion,
                UsuarioModificacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow,
                DVH = 0
            };
            return Save(email);
        }

        public bool ComprobarExistenciaEmail(string emailSinEncriptar)
        {
            string emailEncriptado = EncriptacionManager.EncriptarAES(emailSinEncriptar);
            Email emailBD = Retrieve(new Email { EmailAddress = emailEncriptado }).FirstOrDefault();
            return (emailBD != null && emailBD.Id > 0); //DEVUELVE TRUE SI ENCUENTRA ALGO EN LA BASE
        }

        public int RecalcularDVH_DVV()
        {
            List<Email> emails = Retrieve(new Email());
            TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
            int acumulador = 0;
            foreach (Email email in emails)
            {
                string cadena = ConcatenarDVH(email);
                Save(email);
                acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
            }
            return acumulador;
        }

        #region Metodos Privados
        private void GenerarEImpactarDVH(Email entity)
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

        private string ConcatenarDVH(Email entity)
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
        #endregion
    }
}
