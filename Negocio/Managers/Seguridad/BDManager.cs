using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using System;
using System.Configuration;

namespace Negocio.Managers.Seguridad
{
    public class BDManager
    {

        private readonly IRepository<Respaldo> _Repository;
        private readonly BitacoraManager _bitacoraMgr;


        public BDManager()
        {
            _Repository = new Repository<Respaldo>();
            _bitacoraMgr = new BitacoraManager();
        }

        public Mensaje GenerarBKP(string nombreBKP, int idUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreBKP))
                {
                    return Mensaje.CrearMensaje("MS44", false, true, null, null);
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "RespaldoBD", "Se genero un respaldo de la base de datos", idUsuario);
                    return Mensaje.CrearMensaje("MS22", false, true, null, null);
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "RespaldoBD", "Se produjo una Excepcion generarndo un respaldo de la BD. Exception: " + e.Message, idUsuario);
                }
                catch { }

                return Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
            }
        }

        public Mensaje MontarBKP(string fileName, int idUsuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return Mensaje.CrearMensaje("MS38", false, true, null, null);
                }
                else
                {
                    string fullPath = string.Concat(ConfigurationManager.AppSettings["pathBKP"].ToString(), fileName);

                    string sqlQuery = "USE master ALTER DATABASE TransporteFlexible SET SINGLE_USER WITH ROLLBACK " +
                        "IMMEDIATE RESTORE DATABASE TransporteFlexible FROM DISK = '" + fullPath + "' WITH REPLACE ALTER DATABASE " +
                        "TransporteFlexible SET Multi_User";

                    _Repository.ExecuteQuery(sqlQuery, "master");
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "RespaldoBD", "Se genero un respaldo de la base de datos", idUsuario);
                    return Mensaje.CrearMensaje("MS23", false, true, null, null);
                }
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "RespaldoBD", "Se produjo una Excepcion montando un respaldo de la BD. Exception: " + e.Message, idUsuario);
                }
                catch { }

                return Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
            }
        }
    }
}
