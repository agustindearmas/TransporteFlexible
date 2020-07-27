using System;
using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;

namespace Common.Satellite.Shared
{
    [Table(ProcedureName = "Direccion", Schema = Schema.Shared)]
    public class Address : IAuditoria
    {
        [NameEntity(IdEntity = "IdDireccion", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "IdProvincia", NameEntity = "Province.Id")]
        public Province Province { get; set; }

        [NameEntity(IdEntity = "IdLocalidad", NameEntity = "Location.Id")]
        public Location Location { get; set; }

        [NameEntity(IdEntity = "Calle", NameEntity = "Street")]
        public string Street { get; set; }

        [NameEntity(IdEntity = "Numero", NameEntity = "Number")]
        public int Number { get; set; }

        [NameEntity(IdEntity = "Piso", NameEntity = "Floor")]
        public string Floor { get; set; }

        [NameEntity(IdEntity = "Departamento", NameEntity = "Unit")]
        public string Unit { get; set; }

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
