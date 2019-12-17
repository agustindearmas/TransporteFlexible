using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class BitacoraManager : IManagerCrud<Bitacora>
    {
        private readonly IRepository<Bitacora> _Repository;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Seguridad.Bitacora";
        public BitacoraManager()
        {
            _Repository = new Repository<Bitacora>();
            _digitoVerificadorMgr = new TablaDVVManager();
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Bitacora entity)
        {
            _Repository.Delete(entity);
        }

        public List<Bitacora> Retrieve(Bitacora filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public List<Bitacora> Retrieve(Bitacora filter, string executionName)
        {
            if (filter == null)
            {
                return _Repository.GetAll();
            }
            else
            {
                List<Bitacora> bitacoras = _Repository.Find(filter, executionName);
                NivelCriticidadManager _nivelCriticidadMgr = new NivelCriticidadManager();
                foreach (var bit in bitacoras)
                    bit.NivelCriticidad = _nivelCriticidadMgr.Retrieve(bit.NivelCriticidad).First();

                return bitacoras;
            }
        }

        public int Save(Bitacora entity)
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
                    Create(CriticidadBitacora.Alta, "GuardarBitacora", "Se produjo una excepción salvando Bitacora. Exception: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        public int Create(CriticidadBitacora nivelCriticidad, string evento, string suceso, int idUsuario)
        {
            Bitacora bitacora =
                new Bitacora
                {
                    NivelCriticidad = new NivelCriticidad { Id = (int)nivelCriticidad },
                    Evento = EncriptacionManager.EncriptarAES(evento),
                    Suceso = EncriptacionManager.EncriptarAES(suceso),
                    UsuarioCreacion = idUsuario,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioModificacion = idUsuario,
                    FechaModificacion = DateTime.UtcNow,
                    DVH = 0
                };
            return Create(bitacora);
        }

        public int Create(Bitacora entity)
        {
            return Save(entity);
        }

        public Mensaje ObtenerBitacorasDesencriptadas(string fechaDesde, string fechaHasta, int nivel, string evento, string usuario)
        {
            try
            {
                DateTime? fechaDesdeConvertida;
                DateTime? fechaHastaConvertida;

                if (string.IsNullOrWhiteSpace(fechaDesde))
                {
                    fechaDesdeConvertida = null;
                    fechaHastaConvertida = null;
                }
                else
                {
                    fechaDesdeConvertida = Convert.ToDateTime(fechaDesde);
                    fechaHastaConvertida = Convert.ToDateTime(fechaHasta);
                    if (fechaDesdeConvertida > fechaHastaConvertida)
                    {
                        return Mensaje.CrearMensaje("MS37", false, true, null, RedireccionesEnum.Bitacora.GetDescription());
                    }
                }

                UsuarioManager _usuarioManager = new UsuarioManager();
                string usuarioEncriptado = EncriptacionManager.EncriptarAES(usuario);
                Usuario userDB = _usuarioManager.Retrieve(new Usuario { NombreUsuario = usuarioEncriptado, Id = 0 }).FirstOrDefault();

                int? usuarioAux;
                if (userDB != null && userDB.Id != 0)
                {
                    usuarioAux = userDB.Id;
                }
                else
                {
                    usuarioAux = null;
                }

                Bitacora bitacoraFilter = new Bitacora
                {
                    Evento = string.IsNullOrWhiteSpace(evento) ? null : EncriptacionManager.EncriptarAES(evento),
                    NivelCriticidad = new NivelCriticidad { Id = nivel },
                    UsuarioCreacion = usuarioAux,
                    FechaDesde = fechaDesdeConvertida,
                    FechaHasta = fechaHastaConvertida
                };

                List<Bitacora> bitacoras = Retrieve(bitacoraFilter, "Filtrada");
                bitacoras = DesencriptadorBitacora(bitacoras);
                return Mensaje.CrearMensaje("OK", false, false, bitacoras, null);
            }
            catch (Exception e)
            {
                try
                {
                    Create(CriticidadBitacora.Alta, "Filtro Bitacora", "Se produjo una excepción en el método ObtenerBitacoras()" +
                       " de la clase BitacoraManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                return Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
            }
        }

        public int RecalcularDVH_DVV()
        {
            try
            {
                List<Bitacora> bitacoras = Retrieve(new Bitacora());
                TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
                int acumulador = 0;
                foreach (Bitacora bitacora in bitacoras)
                {
                    string cadena = ConcatenarDVH(bitacora);
                    Save(bitacora);
                    acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
                }
                return acumulador;
            }
            catch (Exception e)
            {
                try
                {
                    Create(CriticidadBitacora.Alta, "RecalcularDVH_DVV", "Se produjo una excepción en el método RecalcularDVH_DVV()" +
                    " de la clase BitacoraManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }

        }

        private List<Bitacora> DesencriptadorBitacora(List<Bitacora> encriptadas)
        {
            foreach (var encrip in encriptadas)
            {
                encrip.Evento = EncriptacionManager.DesencriptarAES(encrip.Evento);
                encrip.Suceso = EncriptacionManager.DesencriptarAES(encrip.Suceso);
            }
            return encriptadas;
        }

        private void GenerarEImpactarDVH(Bitacora entity)
        {
            try
            {
                entity = Retrieve(entity).First();
                entity.DVH = _digitoVerificadorMgr.CalcularImpactarDVH_DVV(ConcatenarDVH(entity), _table);
                _Repository.Save(entity);
            }
            catch (Exception e)
            {
                try
                {
                    Create(CriticidadBitacora.Alta, "Generando DVH", "Se produjo una excepción en el método GenerarEImpactarDVH()" +
                    " de la clase BitacoraManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        private string ConcatenarDVH(Bitacora entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.NivelCriticidad.ToString(),
                entity.Evento,
                entity.Suceso,
                entity.UsuarioCreacion.ToString(),
                entity.FechaCreacion.ToString(),
                entity.UsuarioModificacion.ToString(),
                entity.FechaModificacion.ToString());
            }
            catch (Exception e)
            {
                try
                {
                    Create(CriticidadBitacora.Alta, "Generando DVH", "Se produjo una excepción en el método ConcatenarDVH()" +
                                      " de la clase BitacoraManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

    }
}
