using Common.Attributes;
using Common.Enums.Mappings;
using Common.Interfaces.Seguridad;
using System;
using System.Collections.Generic;

namespace Common.Satellite.Shared
{
    [Table(ProcedureName = "Persona", Schema = Schema.Shared)]
    public class Persona : IAuditoria
    {
        [NameEntity(IdEntity = "IdPersona", NameEntity = "Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [NameEntity(IdEntity = "Nombre", NameEntity = "Nombre")]
        public string Nombre { get; set; }

        [NameEntity(IdEntity = "Apellido", NameEntity = "Apellido")]
        public string Apellido { get; set; }

        [NameEntity(IdEntity = "NumeroCuil", NameEntity = "NumeroCuil")]
        public string NumeroCuil { get; set; }

        [NameEntity(IdEntity = "EsCuit", NameEntity = "EsCuit")]
        public bool EsCuit { get; set; }

        [NameEntity(IdEntity = "DNI", NameEntity = "DNI")]
        public string DNI { get; set; }

        [NameEntity(IdEntity = "IdTelefono", NameEntity = "Telefono.Id")]
        public List<Telefono> Telefonos { get; set; }

        [NameEntity(IdEntity = "IdEmail", NameEntity = "Email.Id")]
        public List<Email> Emails { get; set; }

        [NameEntity(IdEntity = "Baja", NameEntity = "Baja")]
        public Boolean Baja { get; set; }

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

        public int ObtenerDVV()
        {
            throw new NotImplementedException();
        }
    }
}
