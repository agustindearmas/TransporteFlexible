using System.ComponentModel;

namespace TransporteFlexible.Enums
{
    public enum PermisosEnum
    {
        [Description("Roles")]
        LeerRolesPerfiles = 1,

        [Description("Alta Roles")]
        AltaRolesPerfiles = 2,
        [Description("Baja Roles")]
        BajaRolesPerfiles = 3,
        [Description("Modifica Roles")]
        ModificacionRolesPerfiles = 4,

        [Description("Usuarios")]
        LeerUsuariosAdministrativos = 5,
        [Description("Alta Usuarios Administrativos")]
        AltaUsuariosAdministrativos = 6,
        [Description("Baja Usuarios Administrativos")]
        BajaUsuariosAdministrativos = 7,
        [Description("Modifica Usuarios Administrativos")]
        ModificacionUsuariosAdministrativos = 8,

        [Description("Permisos")]
        LeerPermisos = 9,
        [Description("Asigna Permisos")]
        AsignarPermisos = 10,
        [Description("Desasigna Permisos")]
        DesasignarPermisos = 11,

        [Description("Base de Datos")]
        LeerBasedeDatos = 42,
        [Description("Respaldo")]
        RespaldarBasedeDatos = 12,
        [Description("Restauración")]
        RestaurarBasedeDatos = 13,
        [Description("Corregir Dígitos")]
        CorregirDigitosVerificadores = 14,
        [Description("Calcular Dígitos")]
        CalcularDigitosVerificadores = 15,

        [Description("Bitácora")]
        LeerBitacora = 16,
        [Description("Eliminar Bitácora")]
        EliminarBitacora = 17,

        [Description("Cargas")]
        LeerCargas = 18,
        [Description("Alta Carga")]
        AltaCarga = 19,
        [Description("Baja Carga")]
        BajaCarga = 20,
        [Description("Modifica Carga")]
        ModificacionCarga = 21,

        [Description("Viajes")]
        LeerViajes = 22,
        [Description("Alta Viaje")]
        AltaViaje = 23,
        [Description("Baja Viaje")]
        BajaViaje = 24,
        [Description("Modifica Viaje")]
        ModificacionViaje = 25,

        [Description("Reputación")]
        LeerReputacion = 26,
        [Description("Alta Reputacion")]
        AltaReputacion = 27,
        [Description("Baja Reputación")]
        BajaReputacion = 28,
        [Description("Modifica Reputación")]
        ModificacionReputacion = 29,

        [Description("Conductores")]
        LeerConductores = 30,
        [Description("Alta Conductor")]
        AltaConductor = 31,
        [Description("Baja Conductor")]
        BajaConductor = 32,
        [Description("Modifica Conductor")]
        ModificacionConductor = 33,

        [Description("Vehiculos")]
        LeerVehiculos = 34,
        [Description("Alta Vehiculo")]
        AltaVehiculo = 35,
        [Description("BajaVehiculo")]
        BajaVehiculo = 36,
        [Description("Modifica Vehiculo")]
        ModificacionVehiculo = 37,

        [Description("Ofertas")]
        LeerOfertas = 38,
        [Description("Alta Oferta")]
        AltaOferta = 39,
        [Description("Baja Oferta")]
        BajaOferta = 40,
        [Description("Modifica Oferta")]
        ModificacionOferta = 41
    }
}