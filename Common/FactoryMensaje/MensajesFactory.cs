using Common.Enums.Seguridad;
using Common.Extensions;
using Common.Satellite.Shared;
using System;

namespace Common.FactoryMensaje
{
    public static class MessageFactory
    {
        /// <summary>
        /// Devuelve un mensaje que al ser procesado se espera que se rediriga a la pagina de error y 
        /// muestre el mensaje indicado mas el mensaje de la excepcion
        /// </summary>
        /// <param name="codigoMensaje">El codigo de la leyenda que se desea mostrar en la pagina de error</param>
        /// <param name="resultado"> La excepción encontrada en el flujo</param>
        /// <returns> Devuelve un mensaje</returns>
        public static Message GettErrorMessage(string codigoMensaje, Exception resultado)
        {
            return new Message
            {
                CodigoMensaje = codigoMensaje,
                EsError = true,
                Resultado = resultado,
                RutaRedireccion = ViewsEnum.Error.GD()
            };
        }

        /// <summary>
        /// Devuelve un mensaje que al ser procesado espera que se muestre un mensaje en pantalla, no debe ser utilizado
        /// en flujos de error ya que solo mostrara el mensaje que se le indique
        /// </summary>
        /// <param name="codigoMensaje">TIENE QUE SER DISTINTO DE OK</param>
        /// <returns></returns>
        public static Message GetMessage(string codigoMensaje)
        {
            return new Message
            {
                CodigoMensaje = codigoMensaje,
                EsError = false,
                MuestraMensaje = true,
                Resultado = null,
                RutaRedireccion = null
            };
        }

        /// <summary>
        /// Devuelve un mensaje que solo sirve para flujos de error, y al ser procesado se rediriga a la pagina de error con el mensaje pasado por parametro
        /// </summary>
        /// <param name="codigoMensaje">Es el mensaje que sea desea mostrar en la pagina de error</param>
        /// <returns></returns>
        public static Message CrearMensajeErrorFuncional(string codigoMensaje)
        {
            return new Message
            {
                CodigoMensaje = codigoMensaje,
                EsError = true,
                MuestraMensaje = true,
                Resultado = null,
                RutaRedireccion = ViewsEnum.Error.GD()
            };
        }
        /// <summary>
        /// Devuelve un mensaje que al ser procesado muestra la leyenda indicada por parametro una vez mostrada 
        /// redirigue a la pagina pasada por parametro
        /// </summary>
        /// <param name="codigoMensaje">La leyenda que se desea mostrar</param>
        /// <param name="rutaRedireccion">La pagina a la que se desea rediriguir una vez mostrado el mensaje</param>
        /// <returns></returns>
        public static Message GetMessage(string codigoMensaje, string rutaRedireccion)
        {
            return new Message
            {
                CodigoMensaje = codigoMensaje,
                EsError = false,
                MuestraMensaje = true,
                Resultado = null,
                RutaRedireccion = rutaRedireccion
            };
        }

        /// <summary>
        /// Devuelve un Message con información necesaria para la capa de presentación
        /// </summary>
        /// <param name="resultado">Es el objeto que necesita la capa de presentación</param>
        /// <returns></returns>
        public static Message GetOKMessage(object resultado)
        {
            return new Message
            {
                CodigoMensaje = "OK",
                EsError = false,
                MuestraMensaje = false,
                Resultado = resultado,
                RutaRedireccion = null
            };
        }

        /// <summary>
        /// Devuelve un Message que solo informara que lo slicitado al servidor ha sido resuelto con exito
        /// </summary>
        /// <returns></returns>
        public static Message GetOKMessage()
        {
            return new Message
            {
                CodigoMensaje = "OK",
                EsError = false,
                MuestraMensaje = false,
                Resultado = null,
                RutaRedireccion = null
            };
        }

        /// <summary>
        /// Devuelve un mensaje que permite concatenar algo al mensaje generico
        /// </summary>
        /// <returns></returns>
        public static Message CrearMensajeErrorFuncional(string codigoMensaje, string aConcatenar)
        {
            return new Message
            {
                CodigoMensaje = codigoMensaje,
                EsError = false,
                MuestraMensaje = true,
                Resultado = null,
                RutaRedireccion = null,
                Concatena = aConcatenar
            };
        }
    }
}
