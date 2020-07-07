using System.ComponentModel;

namespace Common.Enums.Seguridad
{
    public enum SV
    {
        [Description("NombreUsuario")]
        NombreUsuario,
        [Description("Permisos")]
        Permisos,
        [Description("UsuarioLogueado")]
        UsuarioLogueado,
        [Description("UsuarioModificado")]
        UsuarioModificado,
        [Description("PermisosAsignados")]
        PermisosAsignados,
        [Description("PermisosDesasignados")]
        PermisosDesasignados,
        [Description("Emails")]
        Emails
    }
}
