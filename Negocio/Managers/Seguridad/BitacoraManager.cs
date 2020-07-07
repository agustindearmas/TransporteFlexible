using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.DigitoVerificador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class BitacoraManager : CheckDigit<Bitacora>, IManagerCrud<Bitacora>
    {
        private readonly IRepository<Bitacora> _Repository;

        public BitacoraManager()
        {
            _Repository = new Repository<Bitacora>();
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
                AplicarIntegridadRegistro(entity);
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
                    Evento = CryptManager.EncryptAES(evento),
                    Suceso = CryptManager.EncryptAES(suceso),
                    UsuarioCreacion = idUsuario,
                    FechaCreacion = DateTime.Now,
                    UsuarioModificacion = idUsuario,
                    FechaModificacion = DateTime.Now,
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
                        return MessageFactory.CrearMensaje("MS37", ViewsEnum.Bitacora.GD());
                    }
                }

                UsuarioManager _usuarioManager = new UsuarioManager();
                string usuarioEncriptado = CryptManager.EncryptAES(usuario);
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
                    Evento = string.IsNullOrWhiteSpace(evento) ? null : CryptManager.EncryptAES(evento),
                    NivelCriticidad = new NivelCriticidad { Id = nivel },
                    UsuarioCreacion = usuarioAux,
                    FechaDesde = fechaDesdeConvertida,
                    FechaHasta = fechaHastaConvertida
                };

                List<Bitacora> bitacoras = Retrieve(bitacoraFilter, "Filtrada");
                bitacoras = DesencriptadorBitacora(bitacoras);
                return MessageFactory.GetOkMessage(bitacoras);
            }
            catch (Exception e)
            {
                try
                {
                    Create(CriticidadBitacora.Alta, "Filtro Bitacora", "Se produjo una excepción en el método ObtenerBitacoras()" +
                       " de la clase BitacoraManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        private List<Bitacora> DesencriptadorBitacora(List<Bitacora> encriptadas)
        {
            foreach (var encrip in encriptadas)
            {
                encrip.Evento = CryptManager.DecryptAES(encrip.Evento);
                encrip.Suceso = CryptManager.DecryptAES(encrip.Suceso);
            }
            return encriptadas;
        }

        #region DigitoVerificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Bitacora entity)
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

        protected override void AplicarIntegridadRegistro(Bitacora entity)
        {
            Bitacora bitacora = Retrieve(entity).First();
            bitacora.DVH = CalculateRegistryIntegrity(bitacora);
            _Repository.Save(bitacora);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Bitacora> bitacoras = Retrieve(null);
                foreach (Bitacora bitacora in bitacoras)
                {
                    Save(bitacora);
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
