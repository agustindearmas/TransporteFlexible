namespace Common.Satellite.Shared
{
    public class Mensaje
    {
        public string CodigoMensaje { get; set; }
        public bool EsError { get; set; }
        public bool MuestraMensaje { get; set; }
        public string RutaRedireccion { get; set; }
        public object Resultado { get; set; }

        public static Mensaje CrearMensaje(string codigoMensaje, bool esError, bool muestraMensaje, object resultado, string rutaRedireccion)
        {
            return new Mensaje
            {
                CodigoMensaje = codigoMensaje,
                EsError = esError,
                MuestraMensaje = muestraMensaje,
                Resultado = resultado,
                RutaRedireccion = rutaRedireccion
            };
        }
    }
}
