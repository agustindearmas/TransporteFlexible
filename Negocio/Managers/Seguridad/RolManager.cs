using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums.Seguridad;
using Common.Extensions;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;

namespace Negocio.Managers.Seguridad
{
    public class RolManager : CheckDigit<Rol>, IManagerCrud<Rol>
    {
        private readonly IRepository<Rol> _Repository;
        private readonly LogManager _logMgr;

        public RolManager()
        {
            _Repository = new Repository<Rol>();
            _logMgr = new LogManager();
        }

        public int Save(Rol entity)
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
                    _logMgr.Create(LogCriticality.Alta, "GuardarRol", "Se produjo una excepción salvando un Rol. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        public void Delete(int id)
        {
            _Repository.Delete(id);
        }

        public void Delete(Rol entity)
        {
            _Repository.Delete(entity);
        }

        public List<Rol> Retrieve(Rol filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        public Message GetRoles(int loggedUserId)
        {
            try
            {
                List<Rol> roles = new List<Rol>();
                roles = Retrieve(null);
                return MessageFactory.GetOKMessage(roles);
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "GetRolesByName", "Se produjo una excepción obteniendo roles. Exception: "
                        + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message GetRole(int roleId, int loggedUserId)
        {
            try
            {
                Rol role = new Rol { Id = roleId };
                role = Retrieve(role).FirstOrDefault();
                if (role != null)
                {
                    return MessageFactory.GetOKMessage(role);
                }
                return MessageFactory.GetMessage("MS80");
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "GetRole", "Se produjo una excepción obteniendo un el Rol con RolId: " + roleId + " Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        /// <summary>
        /// Comprueba la existencia de un role en la BD, con el roleName
        /// recibe como parametro un RoleId si interesa descartar algun rol de la existencia
        /// </summary>
        /// <param name="roleName">El rolname del rol a comprobar la existencia</param>
        /// <param name="roleId">El Id del rol a descartar, puede ser null si no interesa descartar a ningun rol</param>
        /// <returns>Un booleano indicando true si existe o false si no existe</returns>
        private bool ExistRole(string roleName, int? roleId)
        {
            try
            {
                Rol rolBD = Retrieve(new Rol { Descripcion = roleName }).FirstOrDefault();
                if (rolBD != null)
                    return rolBD != null && rolBD.Id != roleId;
                return (rolBD != null && rolBD.Id > 0);// DEVUELVO TRUE SI EXISTE EL ROL EN LA BASE DE DATOS.
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message DeleteRol(int roleId, int loggedUserId)
        {
            try
            {
                List<int> essentialRoles = new List<int> { 1, 2, 3, 4, 5 };
                if (essentialRoles.Contains(roleId))
                {
                    //El rol seleccionado es esencial, no puede ser eliminado ni modificado
                    return MessageFactory.GetMessage("MS81");
                }

                Rol role = new Rol { Id = roleId };
                role = Retrieve(role).FirstOrDefault();
                if (role != null)
                {
                    UserManager _userMgr = new UserManager();
                    bool isRoleAssignedToActiveUser = _userMgr.GetFlagAssignedRoleToUser(roleId);
                    if (isRoleAssignedToActiveUser)
                    {
                        //El rol no puede ser eliminado ya que está asociado a un 
                        //usuario registrado en el sistema
                        return MessageFactory.GetMessage("MS19");
                    }

                    Delete(roleId);
                    //El rol ha sido dado de baja correctamente
                    return MessageFactory.GetMessage("MS18");
                }
                else
                {
                    // NO EXISTE NINGUN ROL CON ESE ID
                    return MessageFactory.GetMessage("MS80");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "DeleteRol", "Se produjo una excepción eliminando un rol Exception: "
                        + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message Insert(string roleName, List<Permiso> permissions, int loggedUserId)
        {
            try
            {
                if (ExistRole(roleName, null))
                {
                    //El nombre del rol ya está en uso por otro rol dentro del sistema.
                    return MessageFactory.GetMessage("MS15");
                }

                if (CheckAssignedPermissions(permissions, 0))
                {
                    //Los permisos asociados al rol coinciden con los asociados a otro rol del sistema. 
                    //Cambie las asociaciones para crear un nuevo rol.
                    return MessageFactory.GetMessage("MS16");
                }

                int RoleId = Create(roleName, permissions, loggedUserId);
                if (RoleId > 0)
                {
                    //El rol ha sido creado con éxito
                    return MessageFactory.GetMessage("MS14", ViewsEnum.Rol.GD());
                }
                else
                {
                    throw new Exception("Existe un problema al guardar un rol");
                }

            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "Insert", "Se produjo una excepción salvando un rol Exception: "
                        + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public Message Update(int roleId, string roleName, List<Permiso> permissions, int loggedUserId)
        {
            try
            {
                List<int> essentialRoles = new List<int> { 1, 2, 3, 4, 5 };
                if (essentialRoles.Contains(roleId))
                {
                    //El rol seleccionado es esencial, no puede ser eliminado ni modificado
                    return MessageFactory.GetMessage("MS81");
                }

                if (ExistRole(roleName, roleId))
                {
                    //El nombre del rol ya está en uso por otro rol dentro del sistema.
                    return MessageFactory.GetMessage("MS15");
                }

                if (CheckAssignedPermissions(permissions, roleId))
                {
                    //Los permisos asociados al rol coinciden con los asociados a otro rol del sistema. 
                    //Cambie las asociaciones para crear un nuevo rol.
                    return MessageFactory.GetMessage("MS16");
                }

                Rol role = new Rol
                {
                    Id = roleId
                };

                role = Retrieve(role).FirstOrDefault();

                if (role != null)
                {
                    role.Descripcion = roleName;
                    role.Permisos = permissions;
                    role.UsuarioModificacion = loggedUserId;
                    role.FechaModificacion = DateTime.UtcNow;

                    int roleSave = Save(role);
                    if (roleSave == roleId)
                    {
                        //El rol ha sido modificado correctamente
                        return MessageFactory.GetMessage("MS20", ViewsEnum.Rol.GD());
                    }
                    else
                    {
                        throw new Exception("Existe un problema al guardar un rol");
                    }
                }
                else
                {
                    // NO EXISTE NINGUN ROL CON ESE ID
                    return MessageFactory.GetMessage("MS80");
                }
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "Update", "Se produjo una excepción salvando un rol Exception: "
                        + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        private int Create(string roleName, List<Permiso> permissions, int loggedUserId)
        {
            try
            {
                Rol role = new Rol
                {
                    Id = 0,
                    Permisos = permissions,
                    Descripcion = roleName,
                    UsuarioCreacion = loggedUserId,
                    UsuarioModificacion = loggedUserId,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    DVH = 0
                };
                return Save(role);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool CheckAssignedPermissions(List<Permiso> permissions, int RoleId)
        {
            try
            {
                PermisoManager _perMgr = new PermisoManager();
                List<Rol> rolesBD = Retrieve(null); //Obtengo todos los roles de la BD
                List<int> perIds = _perMgr.GetOnlyPermissionIds(permissions); // Obtengo solo los id de los permisos a crear
                bool equalityFlag = true;
                if (rolesBD != null)
                {
                    rolesBD = rolesBD.Where(rbd => rbd.Id != RoleId).ToList(); //Filtro el rol con el id de rol
                    foreach (Rol role in rolesBD)
                    {
                        if (role.Permisos != null)
                        {
                            List<int> perIdsBD = _perMgr.GetOnlyPermissionIds(role.Permisos);
                            if (perIds.All(perIdsBD.Contains) && perIds.Count == perIdsBD.Count)
                            {
                                equalityFlag = true;
                                break;
                            }
                            else
                            {
                                equalityFlag = false;
                            }
                        }
                    }
                }
                return equalityFlag;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Obtiene todos los permisos desasginados del RoleId que se pase.
        /// Si el RoleId es 0 traera todos los permisson de la base.
        /// </summary>
        /// <param name="roleId">El RoleId del rol que interesa omitir los permisos asignados</param>
        /// <returns>Un mensaje conteniendo los permisos</returns>
        public Message GetDeallocatedPermissions(int roleId, int loggedUserId)
        {
            try
            {
                PermisoManager _permisoMgr = new PermisoManager();
                List<Permiso> permissions = _permisoMgr.Retrieve(null);
                if (roleId != 0)
                {
                    List<Permiso> assignedPermissions = GetPermissionsByRoleId(roleId);
                    permissions = permissions.Where(p => !assignedPermissions.Any(ap => ap.Id == p.Id)).ToList();
                }
                return MessageFactory.GetOKMessage(permissions);
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "GetDeallocatedPermissions", "Se produjo una excepción obteniendo permisos Exception: " + e.Message, loggedUserId);
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }

        public List<Permiso> GetPermissionsByRoleId(int RoleId)
        {
            try
            {
                RolManager _rolMgr = new RolManager();
                Rol rol = new Rol { Id = RoleId };
                rol = _rolMgr.Retrieve(rol).FirstOrDefault();
                return rol.Permisos;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region DigitoVerificador
        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Rol entity)
        {
            try
            {
                return string.Concat(
                entity.Id.ToString(),
                entity.Descripcion,
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

        protected override void AplicarIntegridadRegistro(Rol entity)
        {
            Rol rol = Retrieve(entity).First();
            rol.DVH = CalculateRegistryIntegrity(rol);
            _Repository.Save(rol);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Rol> roles = Retrieve(null);
                foreach (Rol rol in roles)
                {
                    Save(rol);
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
