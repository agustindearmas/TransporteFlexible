using Common.Attributes;
using Common.Enums.Mappings;
using System.Collections.Generic;

namespace Common.Satellite.Shared
{
    [Table(ProcedureName = "Localidad", Schema = Schema.Shared)]
    public class Location
    {
        [NameEntity(IdEntity = "IdLocalidad", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Descripcion", NameEntity = "Description")]
        public string Description { get; set; }

        [NameEntity(IdEntity = "CodigoPostal", NameEntity = "PostalCode")]
        public int PostalCode { get; set; }
    }
}
