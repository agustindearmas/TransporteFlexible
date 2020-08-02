using Common.Enums.Seguridad;
using Common.FactoryMensaje;
using Common.Interfaces.Shared;
using Common.Repositories.Interfaces;
using Common.Satellite.Seguridad;
using Common.Satellite.Shared;
using DataAccess.Concrete;
using Negocio.CheckDigit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio.Managers.Seguridad
{
    public class PermisoManager : CheckDigit<Permiso>, IManagerCrud<Permiso>
    {
        private readonly IRepository<Permiso> _Repository;
        private readonly LogManager _bitacoraMgr;
        private readonly UserManager _usuarioMgr;

        public PermisoManager()
        {
            _Repository = new Repository<Permiso>();
            _bitacoraMgr = new LogManager();
            _usuarioMgr = new UserManager();
        }

        public List<Permiso> Retrieve(Permiso filter)
        {
            return filter == null ? _Repository.GetAll() : _Repository.Find(filter);
        }

        /// <summary>
        /// Este metodo obtiene todos los permisos asignados que posee un usuario
        /// </summary>
        public Message ObtenerPermisosPorUsuarioId(int userId)
        {
            try
            {
                List<Permiso> permisos =
                    _usuarioMgr.ObtenerPermisosDeUnUsuario(userId);

                if (permisos != null)
                {
                    permisos.OrderBy(x => x.Id);
                }

                return MessageFactory.GetOKMessage(permisos);
            }
            catch (Exception e)
            {
                // NO ENCUENTRA EL USUARIO
                throw e;
            }
        }

     

        /// <summary>
        /// Este metodo obtiene todos los permisos desasignados que posee un usuario
        /// </summary>
        public Message ObtenerPermisosDesasignados(int userId)
        {
            try
            {
                List<Permiso> todos = Retrieve(null);
                List<Permiso> asignados = _usuarioMgr.ObtenerPermisosDeUnUsuario(userId);
                List<Permiso> noAsignados = new List<Permiso>();
                if (asignados != null)
                {
                    noAsignados = todos.Where(x => !asignados.Any(y => y.Id == x.Id)).ToList();
                    noAsignados.OrderBy(x => x.Id);
                }
                else
                {
                    noAsignados = todos;
                    noAsignados.OrderBy(x => x.Id);
                }

                return MessageFactory.GetOKMessage(noAsignados);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Save(Permiso entity)
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
                    _bitacoraMgr.Create(LogCriticality.Alta, "GuardarUsuario", "Se produjo una excepción salvando un usuario. Exception: " + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Permiso entity)
        {
            throw new NotImplementedException();
        }

        public List<int> GetOnlyPermissionIds(List<Permiso> permisos)
        {
            try
            {
                List<int> idPermisos = new List<int>();
                foreach (var permiso in permisos)
                {
                    idPermisos.Add(permiso.Id);
                }
                return idPermisos;
            }
            catch (Exception e)
            {

                throw e;
            }

        }


        #region Digito Verificador

        public override void ValidarIntegridadRegistros()
        {
            ValidateIntegrity(Retrieve(null));
        }

        protected override string ConcatenarPropiedadesDelObjeto(Permiso entity)
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

        protected override void AplicarIntegridadRegistro(Permiso entity)
        {
            Permiso permiso = Retrieve(entity).First();
            permiso.DVH = CalculateRegistryIntegrity(permiso);
            _Repository.Save(permiso);
        }

        public override void RecalcularIntegridadRegistros()
        {
            try
            {
                List<Permiso> permisos = Retrieve(null);
                foreach (Permiso permiso in permisos)
                {
                    Save(permiso);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Message ComprobarPermisoAsignadoAOtroUsuario(Permiso permiso, int userId)
        {
            try
            {
                Message msj = null;
                UserManager _usuarioMgr = new UserManager();
                msj = _usuarioMgr.ObtenerUsuariosNegocioDesencriptados(false, null, null);
                if (msj.CodigoMensaje == "OK")
                {
                    foreach (User usuario in msj.Resultado as List<User>)
                    {
                        if (usuario.Id != userId && usuario.GetUserPermissions().Where(x => x.Id == permiso.Id).Any())
                        {
                            return MessageFactory.GetOKMessage();
                        }
                    }
                    return MessageFactory.CreateMessageAndConcat("MS67", permiso.Descripcion);
                }
                else
                {
                    return msj;
                }
                    
            }
            catch (Exception e)
            {

                try
                {
                    _bitacoraMgr.Create(LogCriticality.Alta, "ComprobandoPermiso", "Se Produjo una excepcion comprobando que un permiso este asignado a al menos un usuario" + e.Message, 1); // 1 User sistema
                }
                catch { }
                throw e;
            }
        }
        #endregion
    }
}
