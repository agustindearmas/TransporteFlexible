﻿using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.Managers.Shared;
using System;
using System.Configuration;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class BDManager
    {

        private readonly IRepository<Respaldo> _Repository;
        private readonly LogManager _bitacoraMgr;


        public BDManager()
        {
            _Repository = new Repository<Respaldo>();
            _bitacoraMgr = new LogManager();
        }

        public Message GenerarBKP(string nombreBKP, int idUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreBKP))
                {
                    return MessageFactory.GetMessage("MS44");
                }
                else
                {
                    string path = ConfigurationManager.AppSettings["pathBKP"].ToString();
                    Respaldo resp = new Respaldo
                    {
                        NombreRespaldo = nombreBKP + "_" + DateTime.Now.ToString("ddMMyyyy") + ".bak"
                    };
                    resp.Path = string.Concat(path, resp.NombreRespaldo);

                    _Repository.Execute(resp, "GenerarRespaldo");
                    _bitacoraMgr.Create(LogCriticality.Alta, "RespaldoBD", "Se genero un respaldo de la base de datos", idUsuario);
                    return MessageFactory.GetMessage("MS22");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "RespaldoBD", "Se produjo una Excepcion generarndo un respaldo de la BD. Exception: " + e.Message, idUsuario);
                }
                catch { }

                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message MontarBKP(string fileName, int idUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return MessageFactory.GetMessage("MS38");
                }
                else
                {
                    string fullPath = string.Concat(ConfigurationManager.AppSettings["pathBKP"].ToString(), fileName);

                    string sqlQuery = "USE master ALTER DATABASE TransporteFlexible SET SINGLE_USER WITH ROLLBACK " +
                        "IMMEDIATE RESTORE DATABASE TransporteFlexible FROM DISK = '" + fullPath + "' WITH REPLACE ALTER DATABASE " +
                        "TransporteFlexible SET Multi_User";

                    _Repository.ExecuteQuery(sqlQuery, "master");
                    _bitacoraMgr.Create(LogCriticality.Alta, "RespaldoBD", "Se genero un respaldo de la base de datos", idUsuario);
                    return MessageFactory.GetMessage("MS23");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "RespaldoBD", "Se produjo una Excepcion montando un respaldo de la BD. Exception: " + e.Message, idUsuario);
                }
                catch { }

                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public void BloquearBase()
        {
            ConfigManager _confMgr = new ConfigManager();
            Configuracion config = new Configuracion
            {
                Id = 1,
                Nombre = "Bloqueo",
                Valor = "1",
                FechaModificacion = DateTime.Now,
                UsuarioModificacion = 1
            };
            _confMgr.Save(config);
        }

        public void DesbloquearBase()
        {
            ConfigManager _confMgr = new ConfigManager();
            Configuracion config = new Configuracion
            {
                Id = 1,
                Nombre = "Bloqueo",
                Valor = "0",
                FechaModificacion = DateTime.Now,
                UsuarioModificacion = 1
            };
            _confMgr.Save(config);
        }

        public bool ValidarIntegridadBD()
        {
            try
            {
                ConfigManager _configuracionMgr = new ConfigManager();
                Configuracion config = _configuracionMgr.Retrieve(new Configuracion { Id = 1 }).First();
                if (config.Valor == "1")
                {
                    return true;
                }

                EmailManager _emailMgr = new EmailManager();
                UserManager _usuarioMgr = new UserManager();
                PermisoManager _permisoMgr = new PermisoManager();
                RolManager _rolMgr = new RolManager();
                PersonManager _personaMgr = new PersonManager();
                LogManager _bitacoraMgr = new LogManager();
                PhoneManager _telefonoMgr = new PhoneManager();
                TablaDVVManager _tablaDvvMgr = new TablaDVVManager();
                AddressManager _addressMgr = new AddressManager();

                _emailMgr.ValidarIntegridadRegistros();
                _usuarioMgr.ValidarIntegridadRegistros();
                _permisoMgr.ValidarIntegridadRegistros();
                _rolMgr.ValidarIntegridadRegistros();
                _personaMgr.ValidarIntegridadRegistros();
                _bitacoraMgr.ValidarIntegridadRegistros();
                _configuracionMgr.ValidarIntegridadRegistros();
                _telefonoMgr.ValidarIntegridadRegistros();
                _addressMgr.ValidarIntegridadRegistros();
                _tablaDvvMgr.ValidarIntegridadRegistros();

                config = _configuracionMgr.Retrieve(new Configuracion { Id = 1 }).First();

                return (config.Valor == "1");
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}
