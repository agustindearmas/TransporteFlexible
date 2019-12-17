﻿using Common.DTO.Shared;
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
    public class UsuarioManager : IManagerCrud<Usuario>
    {
        private readonly IRepository<Usuario> _Repository;
        private readonly BitacoraManager _bitacoraMgr;
        private readonly TablaDVVManager _digitoVerificadorMgr;
        private readonly string _table = "Seguridad.Usuario";
        public UsuarioManager()
        {
            _Repository = new Repository<Usuario>();
            _bitacoraMgr = new BitacoraManager();
            _digitoVerificadorMgr = new TablaDVVManager();
        }

        public int Save(Usuario entity)
        {
            try
            {
                entity.Id = _Repository.Save(entity);
                GenerarEImpactarDVH(entity);
                return entity.Id;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarUsuario", "Se produjo una excepción salvando un usuario. Exception: " + e.Message, 1); // 1 Usuario sistema
                return 0;
            }
        }

        public int Create(string nombreUsuarioSinEncriptar, string passwordSinEncriptar, int idPersona, List<Rol> roles, List<Permiso> permisos, int usuarioCreacion)
        {
            Usuario user = new Usuario
            {
                Persona = new Persona { Id = idPersona },
                NombreUsuario = EncriptacionManager.EncriptarAES(nombreUsuarioSinEncriptar),
                Contraseña = EncriptacionManager.EncriptarMD5(passwordSinEncriptar),
                Activo = false,
                Intentos = 0,
                Habilitado = true,
                Baja = false,
                Roles = roles,
                Permisos = permisos,
                UsuarioCreacion = usuarioCreacion,
                FechaCreacion = DateTime.UtcNow,
                UsuarioModificacion = usuarioCreacion,
                FechaModificacion = DateTime.UtcNow,
                DVH = 0
            };

            return Save(user);
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Usuario entity)
        {
            _Repository.Delete(entity);
        }

        public List<Usuario> Retrieve(Usuario filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public Mensaje RegistrarUsuario(RegistroDto registro)
        {
            try
            {
                Mensaje _msjReturn;
                TelefonoManager _telefonoMgr = new TelefonoManager();

                // Compruebo fraude
                if (registro.Rol != "2" && registro.Rol != "3" && registro.Rol != "4")
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Registro", "Se intento crear un usuario con un perfil que no corresponde al proceso de registro automático", 1); // 1 Usuario sistema
                    return Mensaje.CrearMensaje("MS36", false, true, null, null); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                }
                // Compruebo fraude

                // Comparando contraseñas
                if (registro.Contraseña != registro.RepetirContraseña)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "Las contraseñas no coinciden", 1); // 1 Usuario sistema
                    return Mensaje.CrearMensaje("MS62", false, true, null, null); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                }
                // FIN Comparando contraseñas

                //comprobando existencia del usuario               
                if (ComprobarExistenciaUsuario(registro.NombreUsuario))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El nombre de usuario ya existe", 1); // 1 Usuario sistema
                    return Mensaje.CrearMensaje("MS01", false, true, null, null); // EL username ya esta en uso MENSAJE MS01
                }
                // fin comprobando existencia del usuario

                PersonaManager _personaMgr = new PersonaManager();
                //comprobando existencia de la persona
                if (_personaMgr.ComprobarExistenciaPersona(registro.DNI))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "La persona del usuario ya existe", 1); // 1 Usuario sistema
                    return Mensaje.CrearMensaje("MS35", false, true, null, null);// La persona ya existe MENSAJE MS02
                }
                // fin comprobando existencia de la persona

                EmailManager _emailMgr = new EmailManager();
                //comprobando existencia del Email
                if (_emailMgr.ComprobarExistenciaEmail(registro.Email))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El email del usuario ya existe", 1); // 1 Usuario sistema
                    return Mensaje.CrearMensaje("MS02", false, true, null, null);// El Email ya existe Mensaje MS02  
                }
                // fin comprobando existencia del Email

                // CONTACTOS
                Telefono tel = new Telefono
                {
                    Id = _telefonoMgr.Create(registro.Telefono, 1),
                    NumeroTelefono = EncriptacionManager.EncriptarAES(registro.Telefono)
                };
                List<Telefono> telefonos = new List<Telefono> { tel };

                Email email = new Email
                {
                    Id = _emailMgr.Create(registro.Email, 1),
                    EmailAddress = EncriptacionManager.EncriptarAES(registro.Email)
                };
                List<Email> emails = new List<Email> { email };
                // FIN CONTACTOS

                // PERSONA
                int idPersona = _personaMgr.Create(registro.Nombre, registro.Apellido, registro.CUIL,
                    registro.DNI, registro.EsCuit, emails, telefonos, 1);
                // FIN PERSONA

                RolManager _rolMgr = new RolManager();
                PermisoManager _permisoMgr = new PermisoManager();
                //Permisos
                List<Rol> roles = _rolMgr.Retrieve(new Rol { Id = Convert.ToInt32(registro.Rol) });
                List<Permiso> permisos = _permisoMgr.ObtenerPermisosPorRolId(Convert.ToInt32(registro.Rol));
                //Fin Permisos

                // USUARIO
                int idUsuario = Create(registro.NombreUsuario, registro.RepetirContraseña, idPersona, roles, permisos, 1);
                //USUARIO

                if (idUsuario > 0)
                {
                    EnvioEmailManager _envioEmailMgr = new EnvioEmailManager();
                    string idEncriptado = EncriptacionManager.EncriptarAES(idUsuario.ToString());
                    _envioEmailMgr.EnviarEmailRegistro(registro.Email, idEncriptado);
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El usuario: " + idUsuario + " fue creado con éxito", 1); // 1 Usuario sistema
                    _msjReturn = Mensaje.CrearMensaje("MS04", false, true, null, RedireccionesEnum.Default.GetDescription());
                }
                else
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "Algo salio mal registrando un nuevo usuario", 1); // 1 Usuario sistema
                    _msjReturn = Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
                }

                return _msjReturn;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "Registro", "Se produjo una excepción en el método RegistrarUsuario()" +
                    " de la clase UsuarioManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                return Mensaje.CrearMensaje("ER03", true, true, null, RedireccionesEnum.Error.GetDescription());
            }
        }

        public int RecalcularDVH_DVV()
        {
            List<Usuario> usuarios = Retrieve(new Usuario());
            TablaDVVManager _dVerificadorMgr = new TablaDVVManager();
            int acumulador = 0;
            foreach (Usuario usuario in usuarios)
            {
                string cadena = ConcatenarDVH(usuario);
                Save(usuario);
                acumulador += _dVerificadorMgr.ObtenerDVH(cadena);
            }
            return acumulador;
        }

        private bool ComprobarExistenciaUsuario(string nombreUsuarioSinEncriptar)
        {
            string nuEncriptado = EncriptacionManager.EncriptarAES(nombreUsuarioSinEncriptar);
            Usuario usuarioBD = Retrieve(new Usuario { NombreUsuario = nuEncriptado }).FirstOrDefault();
            return (usuarioBD != null && usuarioBD.Id > 0);
        }

        public Mensaje ValidarUsuario(string idUsuarioEncriptado)
        {
            try
            {
                Mensaje _returnMensaje;
                int idUser;
                if (string.IsNullOrEmpty(idUsuarioEncriptado))
                {
                    // ALGO ANDA MAL
                    _returnMensaje = Mensaje.CrearMensaje("MS64", true, true, null, RedireccionesEnum.Error.GetDescription());
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Error al intentar activar un usuario", 1);
                }
                else
                {
                    idUser = Convert.ToInt32(EncriptacionManager.DesencriptarAES(idUsuarioEncriptado.Replace(" ", "+")));
                    Usuario usuarioActivar = Retrieve(new Usuario { Id = idUser }).FirstOrDefault();
                    if (usuarioActivar != null)
                    {
                        usuarioActivar.Activo = true;
                        Save(usuarioActivar);
                        _returnMensaje = Mensaje.CrearMensaje("MS63", false, true, null, RedireccionesEnum.Ingreso.GetDescription());
                        _bitacoraMgr.Create(CriticidadBitacora.Media, "ValidarEmail", "Se activo el Usuario: " + idUser.ToString() + "en el Sistema", 1);
                    }
                    else
                    {
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Se intentó activar un usuario con un Id null o inexistente", 1);
                        _returnMensaje = Mensaje.CrearMensaje("MS64", true, true, null, RedireccionesEnum.Error.GetDescription());
                    }
                }
                return _returnMensaje;
            }
            catch (Exception e)
            {
                _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Se produjo una excepción activando un usuario", 1);
                return Mensaje.CrearMensaje("MS64", true, true, e, RedireccionesEnum.Error.GetDescription());
            }
        }

        #region Metodos privados
        private void GenerarEImpactarDVH(Usuario entity)
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

        private string ConcatenarDVH(Usuario entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Persona.Id.ToString(),
                entity.NombreUsuario,
                entity.Contraseña,
                entity.Activo.ToString(),
                entity.Intentos.ToString(),
                entity.Habilitado.ToString(),
                entity.Baja.ToString(),
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