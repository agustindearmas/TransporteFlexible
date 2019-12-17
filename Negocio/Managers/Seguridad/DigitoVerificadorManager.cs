
using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class TablaDVVManager : IManagerCrud<TablaDVV>
    {
        private readonly IRepository<TablaDVV> _Repository;
        private readonly string _table = "Seguridad.TablaDVV";
        public TablaDVVManager()
        {
            _Repository = new Repository<TablaDVV>();
        }

        public int Save(TablaDVV entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                GenerarEImpactarDVH(entity);
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

        public int CalcularImpactarDVH_DVV(string cadena, string tabla)
        {
            try
            {
                int suma = SumarCaracteres(cadena);
                int dvh = CalcularDVH(suma);
                ActualizarDVV(tabla, dvh);
                return dvh;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int ObtenerDVH(string cadena)
        {
            try
            {
                int suma = SumarCaracteres(cadena);
                int dvh = CalcularDVH(suma);
                return dvh;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int SumarCaracteres(string cadena)
        {
            try
            {
                int acumuladorPares = 0;
                int acumuladorImpares = 0;
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (EsPar(i))
                    {
                        acumuladorPares += cadena[i];
                    }
                    else
                    {
                        acumuladorImpares += cadena[i];
                    }
                }
                int resultadoMultiplicacion = acumuladorPares * 3;
                int suma = resultadoMultiplicacion + acumuladorImpares;
                return suma;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Mensaje RecalcularDigitosVerificadores()
        {
            try
            {
                List<TablaDVV> tablas = Retrieve(new TablaDVV());
                foreach (TablaDVV tbl in tablas)
                {
                    int dvvTabla = ObtenerDVV(tbl.Descripcion);
                    tbl.DVV = dvvTabla;
                    Save(tbl);
                }
                return Mensaje.CrearMensaje("MS24", false, true, null, null);
            }
            catch (Exception e)
            {
                try
                {
                    BitacoraManager _bitacoraMgr = new BitacoraManager();
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarDVV", "Se produjo una excepción salvando DVV. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                return Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
            }
        }

        public List<TablaDVV> Retrieve(TablaDVV filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        #region PRIVADA
        private void ActualizarDVV(string tabla, int dvh)
        {
            try
            {
                string queryDVV = string.Concat("Select SUM(DVH) FROM ", tabla);
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

        private int CalcularDVH(int valor)
        {
            try
            {
                int contador = 0;
                int suma = valor;
                string contadorAuxiliar;
                do
                {
                    contadorAuxiliar = contador.ToString();
                    suma += contador;
                    contador++;

                } while (EsMultiploDe10(suma));

                string dvh = string.Concat(valor.ToString(), contadorAuxiliar);
                return Convert.ToInt32(dvh);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private bool EsPar(int numero)
        {
            return (numero % 2) == 0;
        }

        private bool EsMultiploDe10(int numero)
        {
            return (numero % 10) == 0;
        }

        private int ObtenerDVV(string nombreTabla)
        {
            try
            {
                string nombreEntidad = nombreTabla.Split('.')[1];
                switch (nombreEntidad)
                {
                    case "Email":
                        EmailManager _emailMgr = new EmailManager();
                        return _emailMgr.RecalcularDVH_DVV();
                    case "Usuario":
                        UsuarioManager _usuarioMgr = new UsuarioManager();
                        return _usuarioMgr.RecalcularDVH_DVV();
                    case "Permiso":
                        PermisoManager _permisoMgr = new PermisoManager();
                        return _permisoMgr.RecalcularDVH_DVV();
                    case "Rol":
                        RolManager _rolMgr = new RolManager();
                        return _rolMgr.RecalcularDVH_DVV();
                    case "Persona":
                        PersonaManager _personaMgr = new PersonaManager();
                        return _personaMgr.RecalcularDVH_DVV();
                    case "Bitacora":
                        BitacoraManager _bitacoraMgr = new BitacoraManager();
                        return _bitacoraMgr.RecalcularDVH_DVV();
                    case "Configuracion":
                        ConfiguracionManager _configuracionMgr = new ConfiguracionManager();
                        return _configuracionMgr.RecalcularDVH_DVV();
                    case "Telefono":
                        TelefonoManager _telefonoMgr = new TelefonoManager();
                        return _telefonoMgr.RecalcularDVH_DVV();
                    case "TablaDVV":
                        TablaDVVManager _tablaDVVMgr = new TablaDVVManager();
                        return _tablaDVVMgr.RecalcularDVH_DVV();
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private int RecalcularDVH_DVV()
        {
            try
            {
                List<TablaDVV> tdvvs = Retrieve(new TablaDVV());
                TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
                int acumulador = 0;
                foreach (TablaDVV tddv in tdvvs)
                {
                    string cadena = ConcatenarDVH(tddv);
                    Save(tddv);
                    acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
                }
                return acumulador;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private string ConcatenarDVH(TablaDVV entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Descripcion,
                entity.DVV.ToString(),
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
        private void GenerarEImpactarDVH(TablaDVV entity)
        {
            try
            {
                entity = Retrieve(entity).First();
                TablaDVVManager _digitoVerificadorMgr = new TablaDVVManager();
                entity.DVH = _digitoVerificadorMgr.CalcularImpactarDVH_DVV(ConcatenarDVH(entity), _table);
                _Repository.Save(entity);
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
