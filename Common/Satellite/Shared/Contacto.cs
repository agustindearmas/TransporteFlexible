using Common.Interfaces.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Satellite.Shared
{
    public class Contacto : IAuditoria
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int? UsuarioCreacion { get ; set ; }
        public int? UsuarioModificacion { get ; set ; }
        public DateTime FechaCreacion { get ; set ; }
        public DateTime FechaModificacion { get ; set ; }
        public int DVH { get ; set ; }
    }
}
