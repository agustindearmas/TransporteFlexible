namespace Common.DTO.Shared
{
    public class RegistroDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string CUIL { get; set; }
        public bool EsCuit { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public string RepetirContraseña { get; set; }
        public string Rol { get; set; }
        public bool AutomaticRegister { get; set; }
    }
}
