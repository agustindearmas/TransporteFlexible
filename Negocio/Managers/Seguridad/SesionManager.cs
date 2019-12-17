using Common.Satellite.Seguridad;
using System.Collections.Generic;

namespace Negocio.Managers.Seguridad
{
    public class SessionManager
    {
        public Sesion CrearSession(int idUsuario, List<int> permisos, string nombreUsuario)
        {
            return new Sesion { IdUsuario = idUsuario, Permisos = permisos, NombreUsuario = nombreUsuario };
        }
    }
}
