using System.ComponentModel;

namespace Common.Enums.Seguridad
{
    public enum ViewsEnum
    {
        [Description("/Views/Shared/Error.aspx")]
        Error,
        [Description("/Default.aspx")]
        Default,
        [Description("/Views/Shared/Bienvenida.aspx")]
        Bienvenida,
        [Description("/Views/Seguridad/Ingreso.aspx")]
        Ingreso,
        [Description("/Views/Seguridad/BitacoraView.aspx")]
        Bitacora,
        [Description("/Views/Seguridad/Usuarios/Permisos/PermisoView.aspx")]
        Permiso,
        [Description("/Views/Seguridad/BaseDeDatos.aspx")]
        BaseDeDatos,
        [Description("/Views/Seguridad/Usuarios/UsuarioView.aspx")]
        Usuario,
        /// <summary>
        /// Pantalla de Alta y Modificacion de User
        /// </summary>
        [Description("/Views/Seguridad/Usuarios/UsuarioAMView.aspx")]
        UsuarioAM
    }
}