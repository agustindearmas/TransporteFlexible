using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;
using System;
using System.Collections.Generic;

namespace Common.Satellite.Seguridad
{

    [Table(ProcedureName = "Rol", Schema = Schema.Seguridad)]
    public class Rol: IAuditoria
    {
        [NameEntity(IdEntity = "IdRol", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Descripcion", NameEntity = "Descripcion")]
        public string Descripcion { get; set; }        
        public List<Permiso> Permisos { get; set; }

        [NameEntity(IdEntity = "UsuarioCreacion", NameEntity = "UsuarioCreacion")]
        public int? UsuarioCreacion { get; set; }

        [NameEntity(IdEntity = "UsuarioModificacion", NameEntity = "UsuarioModificacion")]
        public int? UsuarioModificacion { get; set; }

        [NameEntity(IdEntity = "FechaCreacion", NameEntity = "FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [NameEntity(IdEntity = "FechaModificacion", NameEntity = "FechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [NameEntity(IdEntity = "DVH", NameEntity = "DVH")]
        public int DVH { get; set; }
    }
}
