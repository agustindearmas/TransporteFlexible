using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;
using System;

namespace Common.Satellite.Shared
{
    [Table(ProcedureName = "Configuracion", Schema = Schema.Seguridad)]
    public class Configuracion : IAuditoria
    {
        [NameEntity(IdEntity = "IdConfiguracion", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Nombre", NameEntity = "Nombre")]
        public string Nombre { get; set; }

        [NameEntity(IdEntity = "Valor", NameEntity = "Valor")]
        public string Valor { get; set; }

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
