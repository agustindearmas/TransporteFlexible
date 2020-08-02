using System.ComponentModel;

namespace TransporteFlexible.Enums
{
    public enum MensajesEnum
    {
        [Description("El sistema se encuentra fuera de servicio. Estamos trabajando para volver todo a la normalidad. Disculpe las molestias.")]
        ER01,

        [Description("El sistema no puede procesar el formulario tal cual fue completado.")]
        ER02,

        [Description("El sistema no puede completar la solicitud en este momento, intente de nuevo más tarde.")]
        ER03,

        [Description("El nombre de usuario ingresado se corresponde con el de un usuario registrado en el sistema, por favor cámbielo y pruebe nuevamente.")]
        MS01,

        [Description("La dirección de correo electrónica ingresada es utilizada por otro usuario registrado en el sistema, por favor cámbiela y pruebe nuevamente.")]
        MS02,

        [Description("User y/o contraseña invalidos.")]
        MS03,

        [Description("El usuario ha sido creado con éxito, se ha enviado un email para validar su correo electrónico, siga los pasos detallados para activar su usuario.")]
        MS04,

        [Description("El usuario o la contraseña son incorrectos.")]
        MS05,

        [Description("Su usuario ha sido bloqueado por reiterados intentos fallidos, reestablezca su contraseña para seguir operando en el sistema.")]
        MS06,

        [Description("Su usuario se encuentra dado de baja, contacte un administrador si desea revertir la situación.")]
        MS07,

        [Description("Su usuario se encuentra inactivo, revise su email en busca del mail con el enlace de activación.")]
        MS08,

        [Description("Ningún usuario coincide con los datos ingresados, reingréselos e intente nuevamente.")]
        MS09,

        [Description("El usuario ha sido creado con éxito.")]
        MS10,

        [Description("El usuario no puede ser eliminado ya que es el único que posee el/los permisos: “permisos”")]
        MS11,

        [Description("El usuario ha sido dado de baja correctamente.")]
        MS12,

        [Description("El usuario ha sido modificado correctamente.")]
        MS13,

        [Description("El rol ha sido creado con éxito")]
        MS14,

        [Description("El nombre del rol ya está en uso por otro rol dentro del sistema.")]
        MS15,

        [Description("Los permisos asociados al rol coinciden con los asociados a otro rol del sistema. Cambie las asociaciones para crear un nuevo rol.")]
        MS16,

        [Description("El rol no puede ser eliminado ya que es el único que posee el/los permisos: “permisos”")]
        MS17,

        [Description("El rol ha sido dado de baja correctamente")]
        MS18,

        [Description("El rol no puede ser eliminado ya que está asociado a un usuario registrado en el sistema.")]
        MS19,

        [Description("El rol ha sido modificado correctamente")]
        MS20,

        [Description("No se puede deshacer la relación rol permiso, ya que este es el único rol que posee la patente esencial.")]
        MS21,

        [Description("El respaldo ha sido generado correctamente.")]
        MS22,

        [Description("El respaldo ha sido restaurado correctamente.")]
        MS23,

        [Description("La integridad ha sido reestablecida.")]
        MS24,

        [Description("El vehículo ha sido dado de alta correctamente.")]
        MS25,

        [Description("El tractor ingresado en el sistema ya existe.")]
        MS26,

        [Description("El remolque ingresado en el sistema ya existe.")]
        MS27,

        [Description("El tractor ha sido eliminado correctamente.")]
        MS28,

        [Description("El remolque ha sido eliminado correctamente.")]
        MS29,

        [Description("No se puede eliminar el tractor en este momento ya que es parte de un viaje")]
        MS30,

        [Description("No se puede eliminar el remolque en este momento ya que es parte de un viaje")]
        MS31,

        [Description("El vehículo ha sido modificado correctamente.")]
        MS32,

        [Description("No se puede modificar un vehículo que es parte de una oferta, cancele la oferta para proseguir con la modificación.")]
        MS33,

        [Description("La carga ya tiene una oferta asociada, si desea volver a ofertar elimine la oferta vigente.")]
        MS34,

        [Description("La persona por dar de alta ya tiene un usuario en el sistema.")]
        MS35,

        [Description("Intento de fraude detectado.")]
        MS36,

        [Description("La fecha desde no puede ser mayor a la fecha hasta.")]
        MS37,

        [Description("Debe seleccionar un respaldo para ser montado.")]
        MS38,

        [Description("El usuario no posee los permisos necesarios.")]
        MS39,

        [Description("Se necesita un nombre para el Respaldo.")]
        MS44,

        [Description("El vehículo seleccionado no es el propicio para la carga a transportar.")]
        MS45,

        [Description("Todos los conductores se encuentran ocupados en esas fechas y horarios")]
        MS46,

        [Description("Se ha creado una oferta nueva.")]
        MS47,

        [Description("Se ha cancelado una oferta.")]
        MS48,

        [Description("La oferta no puede ser eliminada ya que el viaje está en proceso")]
        MS49,

        [Description("No se puede eliminar ofertas Terminadas.")]
        MS50,

        [Description("No se pueden eliminar ofertas Canceladas.")]
        MS51,

        [Description("Se ha creado una carga.")]
        MS52,

        [Description("Ya existe una carga con las mismas características.")]
        MS53,

        [Description("Se ha eliminado la carga.")]
        MS54,

        [Description("No se puede cancelar la carga ya que se encuentra en viaje.")]
        MS55,

        [Description("Se ha creado un viaje, revise su email para más información.")]
        MS56,

        [Description("Su perfil no puede ser eliminado hasta que finalice o cancele todas sus transacciones ")]
        MS57,

        [Description("Su contraseña ha sido cambiada.")]
        MS58,

        [Description("Error en los datos ingresados reintente. ")]
        MS59,

        [Description("Calificación Enviada")]
        MS60,

        [Description("Resulta imposible localizar el dispositivo móvil en viaje, intente más tarde.")]
        MS61,

        [Description("Las contraseñas no coinciden")]
        MS62,

        [Description("Usuario Activado")]
        MS63,

        [Description("Imposible activar el usuario")]
        MS64,

        [Description("Debe seleccionar un permiso para poder asignarlo")]
        MS65,

        [Description("Debe seleccionar un permiso para poder desasignarlo")]
        MS66,

        [Description("Es el único User que posee el Permiso: {0}, no se puede completar la operación")]
        MS67,

        [Description("El usuario no puede ser deshabilitado ya que es el único que posee el permiso {0}")]
        MS68,

        [Description("No existe el email, contacte una Administrador del Sistema")]
        MS69,

        [Description("No se puede borrar el Email, es el único mail o el único habilitado para el usuario")]
        MS70,

        [Description("No existe ninguna persona con ese Id")]
        MS71,

        [Description("No existe un telefono con ese Id")]
        MS72,

        [Description("No se puede borrar el Teléfono, es el único asignado para el usuario")]
        MS73,

        [Description("No existe una dirección con ese Id")]
        MS74,

        [Description("No existe un usuario con ese Id")]
        MS75,

        [Description("La persona ha sido modificada correctamente.")]
        MS76,

        [Description("Ya existe una persona con ese DNI")]
        MS77,

        [Description("La integridad de los datos ha sido afectada")]
        MS78,

        [Description("Xml Exportado con éxito")]
        MS79,

        [Description("No existe un rol con ese Id")]
        MS80,

        [Description("El rol seleccionado es esencial, no puede ser eliminado ni modificado")]
        MS81,

        [Description("Suceso dado de baja correctamente")]
        MS82,

        [Description("No existe un suceso con ese Id")]
        MS83,

        [Description("No se puede dar de alta un usuario sin un Rol asignado")]
        MS84
    }
}