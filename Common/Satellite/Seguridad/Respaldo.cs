using Common.Attributes;
using Common.Enums.Mappings;

namespace Common.Satellite.Seguridad
{
    [Table(ProcedureName = "Respaldo", Schema = Schema.Seguridad)]
    public class Respaldo
    {
        public string NombreRespaldo { get; set; }

        [NameEntity(IdEntity = "Path", NameEntity = "Path")]
        public string Path { get; set; }
    }
}
