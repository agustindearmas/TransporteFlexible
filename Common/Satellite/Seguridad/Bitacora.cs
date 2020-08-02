using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;
using System;

namespace Common.Satellite.Seguridad
{
    [Table(ProcedureName = "Bitacora", Schema = Schema.Seguridad)]
    public class Bitacora : IAuditoria
    {
        [NameEntity(IdEntity = "IdBitacora", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "NivelCriticidad", NameEntity = "NivelCriticidad.Id")]
        public NivelCriticidad NivelCriticidad { get; set; }

        [NameEntity(IdEntity = "Evento", NameEntity = "Evento")]
        public string Evento { get; set; }

        [NameEntity(IdEntity = "Suceso", NameEntity = "Suceso")]
        public string Suceso { get; set; }

        [NameEntity(IdEntity = "FechaDesde", NameEntity = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [NameEntity(IdEntity = "FechaHasta", NameEntity = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [NameEntity(IdEntity = "Baja", NameEntity = "Baja")]
        public bool? Baja { get; set; }

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
    }
}
