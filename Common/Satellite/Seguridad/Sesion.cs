using System.Collections.Generic;

namespace Common.Satellite.Seguridad
{
    public class Sesion
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public List<int> Permisos { get; set; }
    }
}
