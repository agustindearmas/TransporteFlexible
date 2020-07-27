using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class LogInManager
    {
        private readonly LogManager _bitacoraMgr;

        public LogInManager()
        {
            _bitacoraMgr = new LogManager();
        }
        public Message LogIn(string userName, string password)
        {
            //encripto campos punto 9 del caso de uso
            string encryptedPassword = CryptManager.EncriptMD5(password);
            string encryptedUser = CryptManager.EncryptAES(userName);

            UserManager _usuarioMgr = new UserManager();
            SessionManager _sessionMgr = new SessionManager();
            PermisoManager _permisoMgr = new PermisoManager();



            try
            {
                Message msj = new Message();

                // Obtengo de la base de datos el user que se corresponda con el nombre de user ingresado punto 10 del caso de uso
                User user = _usuarioMgr.Retrieve(new User { NombreUsuario = encryptedUser }).FirstOrDefault();

                if (CheckBdConsistency())
                {
                    Sesion ses = AdminLogin(encryptedUser, encryptedPassword, userName);

                    return ses != null ? MessageFactory.GetOKMessage(ses) :
                        throw new Exception("Falla Admin login");
                }

                if (user == null) // Valida existencia del user
                {
                    // No existe el user
                    // creo bitacora punto 18 del caso de uso
                    Sesion ses = AdminLogin(encryptedUser, encryptedPassword, userName);
                    if (ses != null)
                    {
                        return MessageFactory.GetOKMessage(ses);
                    }
                    else
                    {
                        _bitacoraMgr.Create(LogCriticality.Alta, "Login", "Intento de ingreso de un user no registrado", 1);
                        return MessageFactory.GetMessage("MS09");
                    }

                }

                if (user.Contraseña != encryptedPassword) // Valida que las contraseñas sean iguales punto 11
                {
                    // Contraseña Incorrecta
                    user.Intentos++;
                    if (user.Intentos >= 3)
                    {
                        user.Activo = false;
                        _usuarioMgr.Save(user);
                        _bitacoraMgr.Create(LogCriticality.Alta, "Login", "User bloqueado por reiterado intentos, IdUsuario: " + user.Id.ToString(), 0);
                        return MessageFactory.GetMessage("MS06", ViewsEnum.Default.GD());
                    }
                    else
                    {
                        // creo bitacora punto 18 del caso de uso
                        _usuarioMgr.Save(user);
                        _bitacoraMgr.Create(LogCriticality.Alta, "Login", "Intento de ingreso por un user registrado, no concidio su password, IdUsuario: " + user.Id.ToString(), 1);
                        return MessageFactory.GetMessage("MS05");
                    }
                }

                PersonManager _personMgr = new PersonManager();
                user.Persona = _personMgr.Retrieve(user.Persona).Single();
                if (!user.Habilitado)
                {
                    _bitacoraMgr.Create(LogCriticality.Baja, "Login", "Intento de ingreso por un user inhabilitado, IdUsuario: " + user.Id.ToString(), 1);
                    // User Inhabilitado
                    return MessageFactory.GetMessage("MS08");
                }

                if (user.Baja)
                {
                    _bitacoraMgr.Create(LogCriticality.Baja, "Login", "Intento de ingreso por un user dado de baja, IdUsuario: " + user.Id.ToString(), 1);
                    //User Dado de Baja
                    return MessageFactory.GetMessage("MS07");
                }

                if (!user.Activo) // valida que se encuentre activo punto 14
                {
                    _bitacoraMgr.Create(LogCriticality.Baja, "Login", "Intento de ingreso por un user inactivo, IdUsuario: " + user.Id.ToString(), 1);
                    // User inactivo
                    //User Dado de Baja
                    return MessageFactory.GetMessage("MS08");
                }

                // punto 15 del caso de uso
                user.Intentos = 0;
                user.Activo = true;
                _usuarioMgr.Save(user);
                // punto 15 del caso de uso

                // creo el objeto session punto 16 del caso de uso
                Sesion session = _sessionMgr.CreateSession(user.Id, _permisoMgr.GetOnlyPermissionIds(user.Permisos), userName);
                // punto 16

                // punto 17 es ejecutado en la capa de presentacion 

                // creo bitacora punto 18 del caso de uso
                _bitacoraMgr.Create(LogCriticality.Medium, "Login", "Se logueo el user: " + user.Id.ToString() + " en el sistema", 1);
                return MessageFactory.GetOKMessage(session);

                //Punto 19 es ejecutado en la capa de presentacion
            }
            catch (Exception e)
            {
                Sesion ses = AdminLogin(encryptedUser, encryptedPassword, userName);
                if (ses != null)
                {
                    return MessageFactory.GetOKMessage(ses);
                }
                else
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Login", "Se produjo una excepción en el login", 1);
                    return MessageFactory.GettErrorMessage("ER01", e);
                }

            }
        }

        private Sesion AdminLogin(string encryptedUser, string encryptedPassword, string userName)
        {
            if (encryptedUser == ConfigurationManager.AppSettings["userName"] && encryptedPassword == ConfigurationManager.AppSettings["pass"])
            {
                SessionManager _sessionMgr = new SessionManager();
                List<int> permissions = new List<int>
                    {
                        12,
                        13,
                        14,
                        15,
                        16,
                        17,
                        42
                    };
                Sesion session = _sessionMgr.CreateSession(1, permissions, userName);
                return session;
            }
            return null;
        }

        private bool CheckBdConsistency()
        {
            BDManager _bdMgr = new BDManager();
            return _bdMgr.ValidarIntegridadBD();
        }
    }
}
