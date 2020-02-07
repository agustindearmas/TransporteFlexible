using Common.Enums.Seguridad;
using Common.Extensions;
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
            string contraseñaEncriptada = EncriptacionManager.EncriptarMD5(contraseña);
            string usuarioEncriptado = EncriptacionManager.EncriptarAES(nombreUsuario);
            UsuarioManager _usuarioMgr = new UsuarioManager();
            SessionManager _sessionMgr = new SessionManager();
            PermisoManager _permisoMgr = new PermisoManager();

            try
            {
                //ComprobarConsistenciaBD();

                Mensaje msj = new Mensaje();

                // Obtengo de la base de datos el usuario que se corresponda con el nombre de usuario ingresado punto 10 del caso de uso
                Usuario usuario = _usuarioMgr.Retrieve(new Usuario { NombreUsuario = usuarioEncriptado }).FirstOrDefault();

                if (usuario == null) // Valida existencia del usuario
                {
                    // No existe el usuario
                    // creo bitacora punto 18 del caso de uso
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Intento de ingreso de un usuario no registrado", 1);
                    return Mensaje.CrearMensaje("MS09", false, true, null, null);
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
                        return Mensaje.CrearMensaje("MS06", false, true, null, RedireccionesEnum.Default.GetDescription());
                    }
                    else
                    {
                        // creo bitacora punto 18 del caso de uso
                        _usuarioMgr.Save(usuario);
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Intento de ingreso por un usuario registrado, no concidio su contraseña, IdUsuario: " + usuario.Id.ToString(), 1);
                        return Mensaje.CrearMensaje("MS05", false, true, null, null);
                    }

                }

                if (!usuario.Habilitado)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario inhabilitado, IdUsuario: " + usuario.Id.ToString(), 1);
                    // Usuario Inhabilitado
                    return Mensaje.CrearMensaje("MS06", false, true, null, null);
                }

                if (usuario.Baja)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario dado de baja, IdUsuario: " + usuario.Id.ToString(), 1);
                    //Usuario Dado de Baja
                    return Mensaje.CrearMensaje("MS07", false, true, null, null);
                }

                if (!usuario.Activo) // valida que se encuentre activo punto 14
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Login", "Intento de ingreso por un usuario inactivo, IdUsuario: " + usuario.Id.ToString(), 1);
                    // Usuario inactivo
                    //Usuario Dado de Baja
                    return Mensaje.CrearMensaje("MS08", false, true, null, null);
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
                return Mensaje.CrearMensaje("OK", false, false, session, null);

                //Punto 19 es ejecutado en la capa de presentacion
            }
            catch (Exception e)
            {
                if (usuarioEncriptado == ConfigurationManager.AppSettings["userName"] && contraseñaEncriptada == ConfigurationManager.AppSettings["pass"])
                {
                    List<int> permisos = new List<int>
                    {
                        12,
                        13,
                        14,
                        15,
                        42
                    };
                    Sesion session = _sessionMgr.CrearSession(1, permisos, nombreUsuario);
                    return Mensaje.CrearMensaje("OK", false, false, session, null);
                }
                else
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Login", "Se produjo una excepción en el login", 1);
                    return Mensaje.CrearMensaje("ER01", true, true, e, RedireccionesEnum.Error.GetDescription());
                }
            }
        }

        private bool ComprobarConsistenciaBD()
        {
            TablaDVVManager _tablaDVVMgr = new TablaDVVManager();
           return _tablaDVVMgr.ComprobarConsistencia();
        }
    }
}
