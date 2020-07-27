using Common.Attributes;
using Common.Enums.Mappings;
using System.Collections.Generic;

namespace Common.Satellite.Shared
{
    [Table(ProcedureName = "Provincia", Schema = Schema.Shared)]
    public class Province
    {
        [NameEntity(IdEntity = "IdProvincia", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Descripcion", NameEntity = "Description")]
        public string Description { get; set; }

        [NameEntity(IdEntity = "IdLocalidad", NameEntity = "Localidad.Id")]
        public List<Location> Locations { get; set; }
    }
}
