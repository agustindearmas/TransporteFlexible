using Common.Enums.Seguridad;
using Common.Extensions;

namespace Common.Satellite.Shared
{
    /// <summary>
    /// Esta clase obra como enlace de respuesta entre las capas de negocio y presentación 
    /// </summary>
    public class Mensaje
    {
        /// <summary>
        /// Es el codigo de mensaje que se desea mostrar
        /// </summary>
        public string CodigoMensaje { get; set; }

        /// <summary>
        /// Indica si el flujo que se intento ejecutar concluyo con un error
        /// </summary>
        public bool EsError { get; set; }

        /// <summary>
        /// Indica si se debe mostrar un mensaje o no
        /// </summary>
        public bool MuestraMensaje { get; set; }

        /// <summary>
        /// Contiene la ruta a la que se deberia rediriguir segun la accion ejeecutada/deseada
        /// </summary>
        public string RutaRedireccion { get; set; }

        /// <summary>
        /// Contiene un resultado que necesita ser mostrado y/o manipulado por la capa de presentacion
        /// </summary>
        public object Resultado { get; set; }

        /// <summary>
        /// Contiene un mensaje que debe ser concatenado al mensaje generico indicado por el campo CodigoMensaje
        /// </summary>
        public string Concatena { get; set; }
    }
}

