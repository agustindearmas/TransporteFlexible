using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class IngresoManager
    {
        private BitacoraManager _bitacoraMgr;

        public IngresoManager()
        {
            _bitacoraMgr = new BitacoraManager();
        }
        public Mensaje Ingresar(string nombreUsuario, string contraseña)
        {
            //encripto campos punto 9 del caso de uso
            string contraseñaEncriptada = CryptManager.EncriptMD5(contraseña);
            string usuarioEncriptado = CryptManager.EncryptAES(nombreUsuario);

            UsuarioManager _usuarioMgr = new UsuarioManager();
            SessionManager _sessionMgr = new SessionManager();
            PermisoManager _permisoMgr = new PermisoManager();



            try
            {
                Mensaje msj = new Mensaje();

                // Obtengo de la base de datos el usuario que se corresponda con el nombre de usuario ingresado punto 10 del caso de uso
                Usuario usuario = _usuarioMgr.Retrieve(new Usuario { NombreUsuario = usuarioEncriptado }).FirstOrDefault();

                if (ComprobarConsistenciaBD())
                {
                    Sesion ses = AdminLogin(usuarioEncriptado, contraseñaEncriptada, nombreUsuario);

                    return ses != null ? MessageFactory.GetOkMessage(ses) :
                        throw new Exception("Falla Admin login");
                }

                if (usuario == null) // Valida existencia del usuario
                {
                    // No existe el usuario
                    // creo bitacora punto 18 del caso de uso
                    Sesion ses = AdminLogin(usuarioEncriptado, contraseñaEncriptada, nombreUsuario);
                    if (ses != null)
                    {
                        return MessageFactory.GetOkMessage(ses);
                    }
                    else
                    {
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Intento de ingreso de un usuario no registrado", 1);
                        return MessageFactory.CrearMensaje("MS09");
                    }

                }

                if (usuario.Contraseña != contraseñaEncriptada) // Valida que las contraseñas sean iguales punto 11
                {
                    // Contraseña Incorrecta
                    usuario.Intentos++;
                    if (usuario.Intentos >= 3)
                    {
                        usuario.Habilitado = false;
                        _usuarioMgr.Save(usuario);
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Usuario bloqueado por reiterado intentos, IdUsuario: " + usuario.Id.ToString(), 0);
                        return MessageFactory.CrearMensaje("MS06", ViewsEnum.Default.GD());
                    }
                    else
                    {
                        // creo bitacora punto 18 del caso de uso
                        _usuarioMgr.Save(usuario);
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Intento de ingreso por un usuario registrado, no concidio su contraseña, IdUsuario: " + usuario.Id.ToString(), 1);
                        return MessageFactory.CrearMensaje("MS05");
                    }
                }

                if (!usuario.Habilitado)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario inhabilitado, IdUsuario: " + usuario.Id.ToString(), 1);
                    // Usuario Inhabilitado
                    return MessageFactory.CrearMensaje("MS06");
                }

                if (usuario.Baja)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario dado de baja, IdUsuario: " + usuario.Id.ToString(), 1);
                    //Usuario Dado de Baja
                    return MessageFactory.CrearMensaje("MS07");
                }

                if (!usuario.Activo) // valida que se encuentre activo punto 14
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario inactivo, IdUsuario: " + usuario.Id.ToString(), 1);
                    // Usuario inactivo
                    //Usuario Dado de Baja
                    return MessageFactory.CrearMensaje("MS08");
                }

                // punto 15 del caso de uso
                usuario.Intentos = 0;
                usuario.Habilitado = true;
                _usuarioMgr.Save(usuario);
                // punto 15 del caso de uso

                // creo el objeto session punto 16 del caso de uso
                Sesion session = _sessionMgr.CrearSession(usuario.Id, _permisoMgr.ObtenerSoloIdsDePermisos(usuario.Permisos), nombreUsuario);
                // punto 16

                // punto 17 es ejecutado en la capa de presentacion 

                // creo bitacora punto 18 del caso de uso
                _bitacoraMgr.Create(CriticidadBitacora.Media, "Login", "Se logueo el usuario: " + usuario.Id.ToString() + " en el sistema", 1);
                return MessageFactory.GetOkMessage(session);

                //Punto 19 es ejecutado en la capa de presentacion
            }
            catch (Exception e)
            {
                Sesion ses = AdminLogin(usuarioEncriptado, contraseñaEncriptada, nombreUsuario);
                if (ses != null)
                {
                    return MessageFactory.GetOkMessage(ses);
                }
                else
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Se produjo una excepción en el login", 1);
                    return MessageFactory.CrearMensajeError("ER01", e);
                }

            }
        }

        private Sesion AdminLogin(string usuarioEncriptado, string contraseñaEncriptada, string nombreUsuario)
        {
            if (usuarioEncriptado == ConfigurationManager.AppSettings["userName"] && contraseñaEncriptada == ConfigurationManager.AppSettings["pass"])
            {
                SessionManager _sessionMgr = new SessionManager();
                List<int> permisos = new List<int>
                    {
                        12,
                        13,
                        14,
                        15,
                        16,
                        17,
                        42
                    };
                Sesion session = _sessionMgr.CrearSession(1, permisos, nombreUsuario);
                return session;
            }
            return null;
        }

        private bool ComprobarConsistenciaBD()
        {
            BDManager _bdMgr = new BDManager();
            return _bdMgr.ValidarIntegridadBD();
        }
    }
}
