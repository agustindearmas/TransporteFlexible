
using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class TablaDVVManager : CheckDigit<TablaDVV>, IManagerCrud<TablaDVV>
    {
        private readonly IRepository<TablaDVV> _Repository;
        public TablaDVVManager()
        {
            _Repository = new Repository<TablaDVV>();
        }

        public int Save(TablaDVV entity)
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
                    BitacoraManager _bitacoraMgr = new BitacoraManager();
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarDVV", "Se produjo una excepción salvando DVV. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch{}               
                throw e;
            }
        }

        public Mensaje RecalcularDigitosVerificadores()
        {
            try
            {
                List<TablaDVV> tablas = Retrieve(null);
                foreach (TablaDVV tbl in tablas)
                {
                    RecalcularIntegridad(tbl.Descripcion);
                }
                BDManager _bdMgr = new BDManager();
                _bdMgr.DesbloquearBase();
                return MessageFactory.CrearMensaje("MS24");
            }
            catch (Exception e)
            {
                try
                {
                    BitacoraManager _bitacoraMgr = new BitacoraManager();
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarDVV", "Se produjo una excepción salvando DVV. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        public List<TablaDVV> Retrieve(TablaDVV filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public void ActualizarDVV(string tabla, int dvh, int id, string campo)
        {
            try
            {
                string queryDVV = string.Concat("Select CASE WHEN SUM(DVH) IS NULL THEN 0 ELSE SUM(DVH) END FROM ", tabla, " WHERE ", campo, " <> ", id.ToString());
                int dvvTabla = _Repository.ExecuteScalarScript(queryDVV, "TransporteFlexible");
                int sumaDVV = dvh + dvvTabla;
                string queryUpdateDVV = string.Concat("UPDATE Seguridad.TablaDVV SET DVV = ", sumaDVV.ToString(), " WHERE Descripcion = '", tabla, "'");
                _Repository.ExecuteQuery(queryUpdateDVV, "TransporteFlexible");
            }
            catch (Exception e)
            {
                throw e;

            }
        }

        #region PRIVADA
        private void RecalcularIntegridad(string nombreTabla)
        {
            try
            {
                string nombreEntidad = nombreTabla.Split('.')[1];
                switch (nombreEntidad)
                {
                    case "Email":
                        EmailManager _emailMgr = new EmailManager();
                        _emailMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Usuario":
                        UsuarioManager _usuarioMgr = new UsuarioManager();
                        _usuarioMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Permiso":
                        PermisoManager _permisoMgr = new PermisoManager();
                        _permisoMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Rol":
                        RolManager _rolMgr = new RolManager();
                        _rolMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Persona":
                        PersonManager _personaMgr = new PersonManager();
                        _personaMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Bitacora":
                        BitacoraManager _bitacoraMgr = new BitacoraManager();
                        _bitacoraMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Configuracion":
                        ConfigManager _configuracionMgr = new ConfigManager();
                        _configuracionMgr.RecalcularIntegridadRegistros();
                        break;
                    case "Telefono":
                        PhoneManager _telefonoMgr = new PhoneManager();
                        _telefonoMgr.RecalcularIntegridadRegistros();
                        break;
                    case "TablaDVV":
                        RecalcularIntegridadRegistros();
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }
        #endregion


        #region DigitoVerificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(TablaDVV entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Descripcion,
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

        protected override void AplicarIntegridadRegistro(TablaDVV entity)
        {
            TablaDVV tablaDVV = Retrieve(entity).First();
            tablaDVV.DVH = CalculateRegistryIntegrity(tablaDVV);
            _Repository.Save(tablaDVV);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<TablaDVV> dvvs = Retrieve(null);
                foreach (TablaDVV dvv in dvvs)
                {
                    Save(dvv);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Satisfaciendo Interfaz
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(TablaDVV entity)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
