using Common.Attributes;
using Common.Enums.Mappings;

namespace Common.Satellite.Seguridad
{
    [Table(ProcedureName = "NivelCriticidad", Schema = Schema.Seguridad)]
    public class NivelCriticidad
    {
        [NameEntity(IdEntity = "IdCriticidadBitacora", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Descripcion", NameEntity = "Descripcion")]
        public string Descripcion { get; set; }
    }
}
