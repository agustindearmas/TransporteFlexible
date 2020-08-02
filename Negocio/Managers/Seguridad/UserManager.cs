using Common.DTO.Shared;
using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;
using Negocio.Managers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class UserManager : CheckDigit<User>, IManagerCrud<User>
    {
        private readonly IRepository<User> _Repository;
        private readonly LogManager _bitacoraMgr;

        public UserManager()
        {
            _Repository = new Repository<User>();
            _bitacoraMgr = new LogManager();
        }

        public Message ValidateAccount(string emailId, string userId)
        {
            try
            {
                userId = CryptManager.DecryptAES(userId);
                if (int.TryParse(userId, out int id))
                {
                    EmailManager _emailMgr = new EmailManager();
                    _emailMgr.ValidateEmailAccount(emailId);
                    User user = new User { Id = id };
                    if (user != null)
                    {
                        UnblockUser(user);
                        //"Usuario Activado"
                        return MessageFactory.GetMessage("MS63");
                    }
                }
                //"Imposible activar el usuario"
                return MessageFactory.GetMessage("MS64");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Registro", "Se produjo una excepción en el método RegistrarUsuario()" +
                    " de la clase UsuarioManager. Excepción: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message GetUserById(int userId)
        {
            try
            {
                User user = new User { Id = userId, Baja = null };
                user = GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
                if (user != null)
                {
                    return MessageFactory.GetOKMessage(user);
                }
                return MessageFactory.GetMessage("MS75");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "GetUserById", "Se produjo una excepción obteniendo un usuario con Id: " +
                    userId.ToString() + ". Excepción: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public int Save(User entity)
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarUsuario", "Se produjo una excepción salvando un usuario. Exception: "
                        + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        public void Create(User user, int usuarioCreacion)
        {
            user.NombreUsuario = CryptManager.EncryptAES(user.NombreUsuario);
            user.Contraseña = CryptManager.EncriptMD5(user.Contraseña);
            user.Activo = true;
            user.Intentos = 0;
            user.Baja = false;
            user.UsuarioCreacion = usuarioCreacion;
            user.FechaCreacion = DateTime.UtcNow;
            user.UsuarioModificacion = usuarioCreacion;
            user.FechaModificacion = DateTime.UtcNow;
            user.DVH = 0;
            user.Id = Save(user);
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(User entity)
        {
            _Repository.Delete(entity);
        }

        public List<User> Retrieve(User filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        internal void UnblockUser(User user)
        {
            try
            {
                user = GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
                user.Activo = true;
                user.Intentos = 0;
                user.FechaModificacion = DateTime.UtcNow;
                user.NombreUsuario = CryptManager.EncryptAES(user.NombreUsuario);
                int idUser = Save(user);
                if (user.Id != idUser)
                {
                    throw new Exception("Problemas salvando un usuario");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal void BlockUser(User user)
        {
            try
            {
                user = GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
                user.Activo = false;
                user.Intentos = 3;
                user.FechaModificacion = DateTime.UtcNow;
                Random r = new Random();
                int first = r.Next(999, 9999);
                int second = r.Next(999, 9999);
                string pass = first.ToString() + second.ToString();
                user.Contraseña = CryptManager.EncriptMD5(pass);
                user.NombreUsuario = CryptManager.EncryptAES(user.NombreUsuario);
                int idUser = Save(user);

                if (idUser == user.Id)
                {
                    SendEmail(user.Persona.Emails.First().EmailAddress, user.Persona.Emails.First().Id, user.Id, pass);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Obtiene un usuario que coincida con el User Name pasado por parametro
        /// El usuario que devuelve no esta completo ni desencriptado
        /// </summary>
        /// <param name="userName">el nombre de usuario que corresponde con el usuario que se busca debe estar encriptado</param>
        /// <returns>Un usuario no completo ni desencriptado si no encuentra nada devuelve NULL</returns>
        public User GetUserByUserName(string userName)
        {
            try
            {
                User user = new User
                {
                    NombreUsuario = userName
                };
                return GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Realiza todas las validaciones de existencia necesarias para permitir un nuevo registro de usuario en el sistema
        /// Tiene un parametro Out el cual si queda NULL el registro es valido, si esta populado el registro no es valido.
        /// </summary>
        /// <param name="userName">El nombre de Usuario a validar</param>
        /// <param name="personDNI">El dni de la persona a validar</param>
        /// <param name="email">El email a ser validado</param>
        /// <param name="loggedUserId">El id del usuario que impulso el flujo</param>
        /// <param name="message">El mensaje de salida, si es es distinto de null el registro no paso la validación. Si esta vacio si.</param>
        private void ValidateRegister(string userName, string personDNI, string email, int loggedUserId, out Message message)
        {
            message = null;

            //comprobando existencia del usuario               
            if (ExistUser(userName, null))
            {
                _bitacoraMgr.Create(LogCriticality.Baja, "Registro", "El nombre de usuario ya existe", loggedUserId);
                message = MessageFactory.GetMessage("MS01");
                return; // EL username ya esta en uso MENSAJE MS01
            }
            // fin comprobando existencia del usuario

            //comprobando existencia de la persona
            PersonManager _personaMgr = new PersonManager();
            if (_personaMgr.ExistPerson(personDNI, null))
            {
                _bitacoraMgr.Create(LogCriticality.Baja, "Registro", "La persona del usuario ya existe", loggedUserId);
                message = MessageFactory.GetMessage("MS35");// La persona ya existe MENSAJE MS02
                return;
            }
            // fin comprobando existencia de la persona

            //comprobando existencia del Email
            EmailManager _emailMgr = new EmailManager();
            if (_emailMgr.ExistEmail(email))
            {
                _bitacoraMgr.Create(LogCriticality.Baja, "Registro", "El email del usuario ya existe", loggedUserId);
                message = MessageFactory.GetMessage("MS02");// El Email ya existe Message MS02
                return;
            }
            // fin comprobando existencia del Email
        }

        internal bool GetFlagAssignedRoleToUser(int roleId)
        {
            try
            {
                User user = new User
                {
                    Baja = false
                };
                List<User> users = Retrieve(user);

                foreach (User u in users)
                {
                    Rol role = u.Roles.Where(r => r.Id == roleId).FirstOrDefault();
                    if (role != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ValidateAutomaticRegister(string rol, string password, string password2, int loggedUserId, out Message message)
        {
            message = null;
            // Compruebo fraude
            if (rol != "2" && rol != "3" && rol != "4")
            {
                _bitacoraMgr.Create(LogCriticality.Alta, "Registro", "Se intento crear un usuario con un perfil que no corresponde al proceso de registro automático", loggedUserId);
                message = MessageFactory.GetMessage("MS36"); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                return;
            }
            // Compruebo fraude 

            // Comparando contraseñas
            if (password != password2)
            {
                _bitacoraMgr.Create(LogCriticality.Baja, "Registro", "Las contraseñas no coinciden", loggedUserId);
                message = MessageFactory.GetMessage("MS62"); // LAS CONTRASEÑAS NO COINCIDEN mensaje MS62
                return;
            }
            // FIN Comparando contraseñas
        }

        public Message RegisterNewUser(RegistroDto register, int loggedUserId = 1)// si no se suministra el parametro tiene como valor por defecto 1
        {
            try
            {
                User user = new User();
                PersonManager _personMgr = new PersonManager();
                Message message;


                if (register.AutomaticRegister)
                {
                    // Validaciones excusivas del registro automatico
                    ValidateAutomaticRegister(register.Rol, register.Contraseña, register.RepetirContraseña, loggedUserId, out message);
                    if (message != null)
                        return message;
                }
                else
                {
                    //GENERO AUTOMATICAMENTE LA CONTRASEÑA
                    register.Contraseña = CryptManager.EncryptAES(register.NombreUsuario);
                    // LUEGO SE LA ENVIO POR MAIL.
                }

                ValidateRegister(register.NombreUsuario, register.DNI, register.Email, loggedUserId, out message);
                if (message != null)
                    return message;

                user.Persona = _personMgr.CreatePersonForUserRegister(register.Nombre, register.Apellido, register.CUIL,
                register.DNI, register.EsCuit, register.Email, register.Telefono, loggedUserId);

                ResolveUserRolForRegister(user, register.Rol);

                user.NombreUsuario = register.NombreUsuario;
                user.Contraseña = register.Contraseña;

                CreateUserForRegister(user, loggedUserId);

                if (user.Id > 0)
                {
                    int emailId = user.Persona.Emails.Single().Id;
                    if (register.AutomaticRegister)
                    {
                        SendEmail(register.Email, emailId, user.Id, null);
                        message = MessageFactory.GetMessage("MS04", ViewsEnum.Default.GD());
                    }
                    else
                    {
                        SendEmail(register.Email, emailId, user.Id, register.Contraseña);
                        message = MessageFactory.GetMessage("MS04", ViewsEnum.Usuario.GD());
                    }

                    _bitacoraMgr.Create(LogCriticality.Baja, "Registro", "El usuario: " + user.Id + " fue creado con éxito", loggedUserId); // 1 User sistema
                }
                else
                {
                    throw new Exception("Error funcional, problemas al crear User");
                }

                return message;
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Registro", "Se produjo una excepción en el método RegistrarUsuario()" +
                    " de la clase UsuarioManager. Excepción: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        private void SendEmail(string email, int emailId, int userId, string password)
        {
            SendEmailManager _envioEmailMgr = new SendEmailManager();
            _envioEmailMgr.SendRegisterEmail(email, emailId, userId, password);
        }

        private void CreateUserForRegister(User user, int loggedUserId)
        {
            try
            {
                Create(user, loggedUserId);
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Registro", "Se produjo una excepción en el método RegistrarUsuario()" +
                    " de la clase UsuarioManager. Excepción: " + e.Message, loggedUserId); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        private void ResolveUserRolForRegister(User user, string registerRol)
        {
            try
            {
                RolManager _rolMgr = new RolManager();
                int rolId = Convert.ToInt32(registerRol);

                List<Rol> roles = _rolMgr.Retrieve(new Rol { Id = rolId });
                List<Permiso> permisos = _rolMgr.GetPermissionsByRoleId(rolId);

                user.Roles = roles;
                user.Permisos = permisos;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message DownOrUpUser(int userId, int loggedUserId)
        {
            try
            {
                User user = new User { Id = userId };
                user = GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
                if (user != null)
                {
                    if (!user.Baja.Value)
                    {
                        if (CheckIfUserCanBeBlocked(user, out string permitDescription))
                        {
                            user.Baja = true;
                            user.Activo = false;
                        }
                        else
                        {
                            return MessageFactory.CreateMessageAndConcat("MS67", permitDescription);
                        }
                    }
                    else
                    {
                        if (user.Roles.Count > 0)
                        {
                            user.Baja = false;
                            user.Activo = true;
                        }
                        else
                        {
                            //"No se puede dar de alta un usuario sin un Rol asignado"
                            return MessageFactory.GetMessage("MS84");
                        }

                    }
                    user.NombreUsuario = CryptManager.EncryptAES(user.NombreUsuario);
                    int saveUser = Save(user);
                    if (saveUser == user.Id)
                    {
                        return MessageFactory.GetMessage("MS13");
                    }
                    else
                    {
                        throw new Exception("Problemas guardando información de User");
                    }
                }
                return MessageFactory.GetMessage("MS75");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "DeleteUser", "Se produjo una excepción en dando de baja el usuario:" + userId.ToString() + " Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message UpdateUser(int id, string userName, int roleId, bool blocked, int loggedUserId)
        {
            try
            {
                if (ExistUser(userName, id))
                {
                    //Ya existe una persona con ese DNI
                    return MessageFactory.GetMessage("MS77");
                }

                User user = new User { Id = id };
                user = Retrieve(user).FirstOrDefault();
                if (user != null)
                {
                    if (!blocked)
                    {
                        if (CheckIfUserCanBeBlocked(user, out string permitDescription))
                        {
                            user.Activo = blocked;
                        }
                        else
                        {
                            return MessageFactory.CreateMessageAndConcat("MS67", permitDescription);
                        }
                    }

                    user.NombreUsuario = CryptManager.EncryptAES(userName);
                    user.UsuarioModificacion = loggedUserId;
                    user.FechaModificacion = DateTime.UtcNow;
                    user.Roles = new List<Rol>
                    {
                        new Rol { Id = roleId }
                    };
                    int saveUser = Save(user);
                    if (saveUser == user.Id)
                    {
                        return MessageFactory.GetMessage("MS13");
                    }
                    else
                    {
                        throw new Exception("Problemas guardando información de User");
                    }
                }
                return MessageFactory.GetMessage("MS75");

            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "UpdateUser", "Se produjo una excepción actualizando un usuario Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        private bool CheckIfUserCanBeBlocked(User user, out string permitDescription)
        {
            try
            {
                // OBTENGO TODOS LOS USUARIOS
                List<User> allUsers = GetAllUsersDecriptedAndComplete(null);
                List<Permiso> allAsignedPermits = new List<Permiso>();
                permitDescription = "";
                foreach (User userBD in allUsers)
                {
                    if (userBD.Id != user.Id && userBD.Activo && userBD.Habilitado && !userBD.Baja.Value)
                    {
                        // creo una lista con todos los permisos de los usuarios habilitados, activos y distintos del usuario que intento blockear
                        List<Permiso> psBD = userBD.GetUserPermissions();
                        allAsignedPermits.AddRange(psBD);
                    }
                }
                List<Permiso> ps = user.GetUserPermissions();
                foreach (Permiso userPermit in ps)
                {
                    if (allAsignedPermits.Where(p => p.Id == userPermit.Id).Any())
                    {
                        continue;
                    }
                    else
                    {
                        permitDescription = userPermit.Descripcion;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message ActivateOrDeactivateUser(int userId)
        {
            try
            {
                User user = new User { Id = userId };
                user = GetAllUsersDecriptedAndComplete(user).FirstOrDefault();
                if (user != null)
                {
                    if (user.Activo)
                    {
                        // DESACTIVAR
                        if (CheckIfUserCanBeBlocked(user, out string permitDescription))
                            user.Activo = false;
                        else
                            return MessageFactory.CreateMessageAndConcat("MS67", permitDescription);
                    }
                    else
                    {
                        //Activar
                        user.Intentos = 0;
                        user.Activo = true;
                    }
                    user.NombreUsuario = CryptManager.EncryptAES(user.NombreUsuario);
                    int saveUser = Save(user);
                    if (saveUser == user.Id)
                    {
                        return MessageFactory.GetMessage("MS13");
                    }
                    else
                    {
                        throw new Exception("Problemas guardando información de User");
                    }
                }
                return MessageFactory.GetMessage("MS75");
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "ModificandoUsuario", "Se produjo una excepción habilitando deshabilitando un usuario Exception: " + e.Message, 1);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message AsignPermits(int userId, List<Permiso> permisos, int usuarioSession)
        {
            try
            {
                User usuario = new User { Id = userId };
                usuario = Retrieve(usuario).SingleOrDefault();
                if (usuario != null)
                {
                    usuario.Permisos = permisos;
                    int saveReturn = Save(usuario);
                    if (saveReturn != 0)
                    {
                        _bitacoraMgr.Create(LogCriticality.Alta, "Modificacion Usuarios", "Se asignaron permisos al usuario " + userId.ToString(), usuarioSession);
                        return MessageFactory.GetOKMessage();
                    }
                    else
                    {
                        throw new Exception("Error sin controlar");
                    }

                }
                return new Message();
                //return msj;
            }
            catch (Exception ex)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Modificacion Usuarios", "Se produjo una excepción asignando permisos al usuario" + userId.ToString(), usuarioSession);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER01", ex);
            }
        }

        /// <summary>
        /// Comprueba la existencia de un nombre de usuario en la BD
        /// recibe como parametro un UserId si interesa descartar algun usuario de la existencia
        /// </summary>
        /// <param name="nombreUsuarioSinEncriptar"> el nombre de usuario a buscar en la bd sin encriptar</param>
        /// <param name="userId">El userId a descartar, puede ser null si no interesa descartar ningun user</param>
        /// <returns>un booleano indicando true si existe false si no existe</returns>
        private bool ExistUser(string nombreUsuarioSinEncriptar, int? userId)
        {
            try
            {
                string nuEncriptado = CryptManager.EncryptAES(nombreUsuarioSinEncriptar);
                User usuarioBD = Retrieve(new User { NombreUsuario = nuEncriptado }).FirstOrDefault();
                if (userId != null)
                {
                    return usuarioBD != null && usuarioBD.Id != userId;

                }
                return (usuarioBD != null && usuarioBD.Id > 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message ObtenerUsuariosNegocioDesencriptados(bool baja, string nombreUsuario, string dni)
        {
            try
            {
                string nombreUsuariEncrip = "";
                User usuario = new User();
                PersonManager _prsMgr = new PersonManager();


                usuario.Baja = baja;

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
                    if (prs != null && prs.Id != 0)
                    {
                        usuario.Persona = prs;
                    }
                    else
                    {
                        // Devuelvo una lista de usuarios ya que si el dni fue populado y no se encontro ninguna persona con ese dni 
                        // NO existe ningun usuario que no tenga asociada una persona
                        return MessageFactory.GetOKMessage(new List<User>());
                    }
                }

                List<User> usuarios = GetAllUsersDecriptedAndComplete(usuario);

                return MessageFactory.GetOKMessage(usuarios);
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "ObteniendoUsuarios", "Se produjo una excepción en el metodo obteniendo usuarios Exception: " + e.Message, 1);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public List<Permiso> ObtenerPermisosDeUnUsuario(int userId)
        {
            try
            {

                User user = new User {Id = userId };
                return GetAllUsersDecriptedAndComplete(user).Single().GetUserPermissions();
                
            }
            catch (Exception e)
            {
                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "Obteniendo Permisos De Un User", "Se produjo una excepción en el metodo ObteniendoPermisosDeUnUsuario Exception: " + e.Message, 1);
                }
                catch { }
                throw e;
            }

        }

        /// <summary>
        /// Obtiene todos los usuarios de la bd Completos y con sus campos desencriptados
        /// </summary>
        /// <param name="filterUser"> El usuario que sirve de filtro para la busqueda de la BD, puede ser null si no interesa agregar filtro en la busqueda</param>
        /// <returns>Una lista de usuarios, o una lsita de usuarios VACIA si la busqueda no devuelve nada</returns>
        private List<User> GetAllUsersDecriptedAndComplete(User filterUser)
        {
            try
            {
                List<User> usuarios = Retrieve(filterUser);

                if (usuarios.Count > 0)
                {
                    foreach (var usr in usuarios)
                    {
                        usr.NombreUsuario = CryptManager.DecryptAES(usr.NombreUsuario);


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
                            usr.Persona = _personaMgr.GetPersonFull(usr.Persona.Id);
                        }


                        usr.Permisos = permisos;
                        usr.Roles = roles;
                    }
                    return usuarios;
                }
                return new List<User>();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region DIGITO VERIFICADOR
        /// <summary> 
        /// Concatena todas las propiedades del objeto necesarias para el caculo de DVH
        /// </summary>
        protected override string ConcatenarPropiedadesDelObjeto(User entity)
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
        /// <param name="entity">Es el registro a aplicarle integridad en este caso User</param>
        protected override void AplicarIntegridadRegistro(User entity)
        {
            User user = Retrieve(entity).First();
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
                List<User> usuarios = Retrieve(null);
                foreach (User usuario in usuarios)
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
