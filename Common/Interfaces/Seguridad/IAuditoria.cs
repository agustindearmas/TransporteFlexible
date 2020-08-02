using System;

namespace Common.Interfaces.Seguridad
{
    public interface IAuditoria
    {
        int UsuarioCreacion { get; set; }
        int UsuarioModificacion { get; set; }
        DateTime FechaCreacion { get; set; }
        DateTime FechaModificacion { get; set; }
        int DVH { get; set; }
    }
}
