using System;
using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;

namespace Common.Satellite.Seguridad
{
    [Table(ProcedureName = "TablaDVV", Schema = Schema.Seguridad)]
    public class TablaDVV : IAuditoria
    {
        [NameEntity(IdEntity = "IdTablaDVV", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Descripcion", NameEntity = "Descripcion")]
        public string Descripcion { get; set; }

        [NameEntity(IdEntity = "DVV", NameEntity = "DVV")]
        public int DVV { get; set; }

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
