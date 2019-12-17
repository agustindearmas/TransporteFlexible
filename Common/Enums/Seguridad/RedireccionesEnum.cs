using System.ComponentModel;

namespace Common.Enums.Seguridad
{
    public enum RedireccionesEnum
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
        Bitacora
    }
}
