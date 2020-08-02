using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;
using Common.Satellite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Satellite.Seguridad
{
    [Table(ProcedureName = "Usuario", Schema = Schema.Seguridad)]
    public class User : IAuditoria
    {
        public User()
        {
            Persona = new Persona();
            Permisos = new List<Permiso>();
            Roles = new List<Rol>();
        }
        public bool Habilitado
        {
            get
            {
                if (Persona != null)
                {
                    if (Persona.Emails != null)
                    {
                        return Persona.Emails.Where(x => x.Habilitado).ToList().Count > 0;
                    }
                }
                return false;
            }
        }

        [NameEntity(IdEntity = "IdUsuario", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "IdPersona", NameEntity = "Persona.Id")]
        public Persona Persona { get; set; }

        [NameEntity(IdEntity = "NombreUsuario", NameEntity = "NombreUsuario")]
        public string NombreUsuario { get; set; }

        [NameEntity(IdEntity = "Contraseña", NameEntity = "Contraseña")]
        public string Contraseña { get; set; }

        [NameEntity(IdEntity = "Baja", NameEntity = "Baja")]
        public bool? Baja { get; set; }

        [NameEntity(IdEntity = "Activo", NameEntity = "Activo")]
        public bool Activo { get; set; }

        [NameEntity(IdEntity = "Intentos", NameEntity = "Intentos")]
        public int Intentos { get; set; }

        [NameEntity(IdEntity = "IdRol", NameEntity = "Rol.Id")]
        public List<Rol> Roles { get; set; }

        [NameEntity(IdEntity = "IdPermiso", NameEntity = "Permiso.Id")]
        public List<Permiso> Permisos { get; set; }

        [NameEntity(IdEntity = "UsuarioCreacion", NameEntity = "UsuarioCreacion")]
        public int UsuarioCreacion { get; set; }

        [NameEntity(IdEntity = "UsuarioModificacion", NameEntity = "UsuarioModificacion")]
        public int UsuarioModificacion { get; set; }

        [NameEntity(IdEntity = "FechaCreacion", NameEntity = "FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [NameEntity(IdEntity = "FechaModificacion", NameEntity = "FechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [NameEntity(IdEntity = "DVH", NameEntity = "DVH")]
        public int DVH { get; set; }

        public List<Permiso> GetUserPermissions()
        {
            try
            {
                List<Permiso> permissions = this.Permisos;
                foreach (var item in this.Roles)
                {
                    permissions.AddRange(item.Permisos);
                }
                return permissions.GroupBy(p => p.Id).Select(g => g.First()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
