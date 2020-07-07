using Common.DTO.Shared;
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
    public class UsuarioManager : CheckDigit<Usuario>, IManagerCrud<Usuario>
    {
        private readonly IRepository<Usuario> _Repository;
        private readonly BitacoraManager _bitacoraMgr;

        public UsuarioManager()
        {
            _Repository = new Repository<Usuario>();
            _bitacoraMgr = new BitacoraManager();
        }

        public int Save(Usuario entity)
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "GuardarUsuario", "Se produjo una excepción salvando un usuario. Exception: "
                        + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                throw e;
            }
        }

        public int Create(string nombreUsuarioSinEncriptar, string passwordSinEncriptar, int idPersona, List<Rol> roles, List<Permiso> permisos, int usuarioCreacion)
        {
            Usuario user = new Usuario
            {
                Persona = new Persona { Id = idPersona },
                NombreUsuario = CryptManager.EncryptAES(nombreUsuarioSinEncriptar),
                Contraseña = CryptManager.EncriptMD5(passwordSinEncriptar),
                Activo = false,
                Intentos = 0,
                Habilitado = true,
                Baja = false,
                Roles = roles,
                Permisos = permisos,
                UsuarioCreacion = usuarioCreacion,
                FechaCreacion = DateTime.Now,
                UsuarioModificacion = usuarioCreacion,
                FechaModificacion = DateTime.Now,
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
                PhoneManager _telefonoMgr = new PhoneManager();

                // Compruebo fraude
                if (registro.Rol != "2" && registro.Rol != "3" && registro.Rol != "4")
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Registro", "Se intento crear un usuario con un perfil que no corresponde al proceso de registro automático", 1); // 1 Usuario sistema
                    return MessageFactory.CrearMensaje("MS36"); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                }
                // Compruebo fraude

                // Comparando contraseñas
                if (registro.Contraseña != registro.RepetirContraseña)
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "Las contraseñas no coinciden", 1); // 1 Usuario sistema
                    return MessageFactory.CrearMensaje("MS62"); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                }
                // FIN Comparando contraseñas

                //comprobando existencia del usuario               
                if (ComprobarExistenciaUsuario(registro.NombreUsuario))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El nombre de usuario ya existe", 1); // 1 Usuario sistema
                    return MessageFactory.CrearMensaje("MS01"); // EL username ya esta en uso MENSAJE MS01
                }
                // fin comprobando existencia del usuario

                PersonManager _personaMgr = new PersonManager();
                //comprobando existencia de la persona
                if (_personaMgr.ComprobarExistenciaPersona(registro.DNI))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "La persona del usuario ya existe", 1); // 1 Usuario sistema
                    return MessageFactory.CrearMensaje("MS35");// La persona ya existe MENSAJE MS02
                }
                // fin comprobando existencia de la persona

                EmailManager _emailMgr = new EmailManager();
                //comprobando existencia del Email
                if (_emailMgr.ExistEmail(registro.Email))
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El email del usuario ya existe", 1); // 1 Usuario sistema
                    return MessageFactory.CrearMensaje("MS02");// El Email ya existe Mensaje MS02  
                }
                // fin comprobando existencia del Email

                // CONTACTOS
                Telefono tel = new Telefono
                {
                    Id = _telefonoMgr.Create(registro.Telefono, 1),
                    NumeroTelefono = CryptManager.EncryptAES(registro.Telefono)
                };
                List<Telefono> telefonos = new List<Telefono> { tel };

                Email email = new Email
                {
                    Id = _emailMgr.Create(registro.Email, 1),
                    EmailAddress = CryptManager.EncryptAES(registro.Email)
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
                    SendEmailManager _envioEmailMgr = new SendEmailManager();
                    string idEncriptado = CryptManager.EncryptAES(idUsuario.ToString());
                    _envioEmailMgr.SendRegisterEmail(registro.Email, idEncriptado);
                    _bitacoraMgr.Create(CriticidadBitacora.Baja, "Registro", "El usuario: " + idUsuario + " fue creado con éxito", 1); // 1 Usuario sistema
                    _msjReturn = MessageFactory.CrearMensaje("MS04", ViewsEnum.Default.GD());
                }
                else
                {
                    throw new Exception("Error funcional, problemas al crear Usuario");
                }

                return _msjReturn;
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Registro", "Se produjo una excepción en el método RegistrarUsuario()" +
                    " de la clase UsuarioManager. Excepción: " + e.Message, 1); // 1 Usuario sistema
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        public Mensaje HabilitrDeshabilitarUsuario(int id)
        {
            try
            {
                Usuario usuario = Retrieve(new Usuario { Id = id }).Single();

                if (usuario.Habilitado)
                {
                    // habilitado
                    PermisoManager _permisoMgr = new PermisoManager();
                    foreach (Permiso permiso in usuario.Permisos)
                    {
                       Mensaje msj = _permisoMgr.ComprobarPermisoAsignadoAOtroUsuario(permiso, usuario.Id);
                        if (msj.CodigoMensaje != "OK")
                        {
                            return msj;
                        }
                    }

                    usuario.Habilitado = !usuario.Habilitado;
                }
                else
                {
                    // deshabilitado
                    usuario.Intentos = 0;
                    usuario.Habilitado = !usuario.Habilitado;
                }
               

                int saveAnswer = Save(usuario);

                if (saveAnswer == id)
                {
                    return MessageFactory.CrearMensaje("MS13");
                }

                throw new Exception("Problemas guardando información de Usuario");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ModificandoUsuario", "Se produjo una excepción habilitando deshabilitando un usuario Exception: " + e.Message, 1);
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        public Mensaje AsignarPermisos(int userId, List<Permiso> permisos, int usuarioSession)
        {
            try
            {
                Usuario usuario = new Usuario { Id = userId };
                usuario = Retrieve(usuario).SingleOrDefault();
                if (usuario != null)
                {
                    usuario.Permisos = permisos;
                    int saveReturn = Save(usuario);
                    if (saveReturn != 0)
                    {
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificacion Usuarios", "Se asignaron permisos al usuario " + userId.ToString(), usuarioSession);
                        return MessageFactory.CrearMensajeOk();
                    }
                    else
                    {
                        throw new Exception("Error sin controlar");
                    }

                }
                return new Mensaje();
                //return msj;
            }
            catch (Exception ex)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Modificacion Usuarios", "Se produjo una excepción asignando permisos al usuario" + userId.ToString(), usuarioSession);
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER01", ex);
            }
        }

        private bool ComprobarExistenciaUsuario(string nombreUsuarioSinEncriptar)
        {
            try
            {
                string nuEncriptado = CryptManager.EncryptAES(nombreUsuarioSinEncriptar);
                Usuario usuarioBD = Retrieve(new Usuario { NombreUsuario = nuEncriptado }).FirstOrDefault();
                return (usuarioBD != null && usuarioBD.Id > 0);
            }
            catch (Exception e)
            {
                throw e;
            }
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
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Error al intentar activar un usuario", 1);
                    _returnMensaje = MessageFactory.CrearMensajeErrorFuncional("MS64");
                    
                }
                else
                {
                    idUser = Convert.ToInt32(CryptManager.DecryptAES(idUsuarioEncriptado.Replace(" ", "+")));
                    Usuario usuarioActivar = Retrieve(new Usuario { Id = idUser }).FirstOrDefault();
                    if (usuarioActivar != null)
                    {
                        usuarioActivar.Activo = true;
                        Save(usuarioActivar);
                        _returnMensaje = MessageFactory.CrearMensaje("MS63", ViewsEnum.Ingreso.GD());
                        _bitacoraMgr.Create(CriticidadBitacora.Media, "ValidarEmail", "Se activo el Usuario: " + idUser.ToString() + "en el Sistema", 1);
                    }
                    else
                    {
                        _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Se intentó activar un usuario con un Id null o inexistente", 1);
                        _returnMensaje = MessageFactory.CrearMensajeErrorFuncional("MS64");
                    }
                }
                return _returnMensaje;
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ValidarEmail", "Se produjo una excepción activando un usuario", 1);
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        public Mensaje ObtenerUsuariosNegocioDesencriptados(int id, string nombreUsuario, string dni)
        {
            try
            {
                string nombreUsuariEncrip = "";
                Usuario usuario = new Usuario();
                PersonManager _prsMgr = new PersonManager();


                if (id != 0)
                {
                    usuario.Id = id;

                }

                if (!String.IsNullOrWhiteSpace(nombreUsuario))
                {
                    nombreUsuariEncrip = CryptManager.EncryptAES(nombreUsuario);
                    usuario.NombreUsuario = nombreUsuariEncrip;

                }

                string dniEncriptado = "";
                if (!String.IsNullOrWhiteSpace(dni))
                {
                    dniEncriptado = CryptManager.EncryptAES(dni);
                    Persona prs = _prsMgr.Retrieve(new Persona { DNI = dniEncriptado }).FirstOrDefault();
                    if (prs.Id != 0)
                    {
                        usuario.Persona = prs;

                    }
                }

                List<Usuario> usuarios = Retrieve(usuario);

                if (usuarios.Count > 0)
                {
                    foreach (var usr in usuarios)
                    {
                        usr.NombreUsuario = CryptManager.DecryptAES(usr.NombreUsuario);
                        usr.Persona = _prsMgr.Retrieve(usr.Persona).FirstOrDefault();

                        PermisoManager _permisoMgr = new PermisoManager();
                        RolManager _rolesMgr = new RolManager();
                        PersonManager _personaMgr = new PersonManager();
                        List<Permiso> permisos = new List<Permiso>();
                        List<Rol> roles = new List<Rol>();

                        if (usr.Permisos != null)
                        {
                            foreach (var prm in usr.Permisos)
                            {
                                permisos.Add(_permisoMgr.Retrieve(prm).First());
                            }
                        }

                        if (usr.Roles != null)
                        {
                            foreach (var rl in usr.Roles)
                            {
                                roles.Add(_rolesMgr.Retrieve(rl).First());
                            }
                        }

                        if (usr.Persona != null)
                        {
                            usr.Persona = _personaMgr.ObtenerPersonaDesencriptada(usr.Persona.Id);
                        }
                       

                        usr.Permisos = permisos;
                        usr.Roles = roles;
                    }
                }

                return MessageFactory.GetOkMessage(usuarios);
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "ObteniendoUsuarios", "Se produjo una excepción en el metodo obteniendo usuarios Exception: " + e.Message, 1);
                }
                catch { }
                return MessageFactory.CrearMensajeError("ER03", e);
            }
        }

        public List<Permiso> ObtenerPermisosDeUnUsuario(int userId)
        {
            try
            {
                return Retrieve(new Usuario { Id = userId }).Single().Permisos;
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(CriticidadBitacora.Alta, "Obteniendo Permisos De Un Usuario", "Se produjo una excepción en el metodo ObteniendoPermisosDeUnUsuario Exception: " + e.Message, 1);
                }
                catch { }
                throw e;
            }

        }

        #region DIGITO VERIFICADOR
        /// <summary> 
        /// Concatena todas las propiedades del objeto necesarias para el caculo de DVH
        /// </summary>
        protected override string ConcatenarPropiedadesDelObjeto(Usuario entity)
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

        /// <summary>
        /// Valida que todos los registros de la tabla que corresponde tengan integridad y consistencia.
        /// </summary>
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        /// <summary>
        /// Aplica integridad en un registro especifico. Calcula el DVH
        /// </summary>
        /// <param name="entity">Es el registro a aplicarle integridad en este caso Usuario</param>
        protected override void AplicarIntegridadRegistro(Usuario entity)
        {
            Usuario user = Retrieve(entity).First();
            user.DVH = CalculateRegistryIntegrity(user);
            _Repository.Save(user);
        }

        /// <summary>
        /// Recalcula la integridad de todos los registros de la tabla que corresponde
        /// </summary>
        /// <returns>Retorna la suma de todos los DVH es decir el DVV</returns>
        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Usuario> usuarios = Retrieve(null);
                foreach (Usuario usuario in usuarios)
                {
                    Save(usuario);
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
