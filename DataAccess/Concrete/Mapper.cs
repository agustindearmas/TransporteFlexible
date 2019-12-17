using Common.Attributes;
using Common.Enums.Mappings;
using Common.Extensions;
using DataAccess.Interfaces;
using System;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DataAccess.Concrete
{
    public class Mapper : IMapper
    {
        private const string MensajeEntidadNoEncontrada = "No se encontro la entidad '{0}' en el mapping";
        private const string MensajePropiedadDuplicada = "La propiedad '{0}' de la entidad '{1}' tiene mas de una coincidencia en el mapping.";
        private const string MensajeAtributoDuplicado = "El atributo '{0}' de la entidad '{1}' tiene mas de una coincidencia en el mapping.";

        private string GetXMLConfigurationPath(Type entityName)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string archivoMapping;

            var TableAtt = (TableAttribute)Attribute.GetCustomAttribute(entityName, typeof(TableAttribute));

            var name = TableAtt.Schema != Schema.None ? TableAtt.Schema.GetName() : TableAtt.Schema.GetName();

            archivoMapping = string.Format(ConfigurationManager.AppSettings["ArchivoMapping"], name);

            return Path.Combine(path, archivoMapping);
        }

        public string GetPropertyName(DbParameter parametro, Type nombreEntidad)
        {
            string campo = parametro.ParameterName.ToString().Replace("@", "");
            return GetPropertyName(campo, nombreEntidad);
        }

        public string GetPropertyName(string campo, Type nombreEntidad)
        {
            var props = ObtenerNodoEntidadPor(nombreEntidad).Descendants("Propiedades").Descendants("Propiedad")
                        .Where(p => p.Attribute("atributoDB").Value == campo);

            if (!props.Any()) return string.Empty;

            if (props.Count() > 1)
                throw new Exception(string.Format(MensajeAtributoDuplicado, campo, nombreEntidad.Name));

            return props.ElementAt(0).Attribute("nombre").Value.Trim();
        }

        public string GetAttributeDBName(string propiedad, Type nombreEntidad)
        {
            var props = ObtenerNodoEntidadPor(nombreEntidad).Descendants("Propiedades").Descendants("Propiedad")
                        .Where(p => p.Attribute("nombre").Value == propiedad);

            if (!props.Any()) return string.Empty;

            if (props.Count() > 1)
                throw new Exception(string.Format(MensajePropiedadDuplicada, propiedad, nombreEntidad.Name));

            return props.ElementAt(0).Attribute("atributoDB").Value.Trim();
        }

        public string[] GetPropertyDBKey(Type entidadNombre)
        {
            var props = ObtenerNodoEntidadPor(entidadNombre).Descendants("Propiedades").Descendants("Propiedad")
                         .Where(p => bool.Parse(p.Attribute("esClave").Value));

            var resultado = new string[props.Count()];

            for (int i = 0; i < props.Count(); i++)
            {
                resultado[i] = props.ElementAt(i).Attribute("nombre").Value.Trim();
            }

            return resultado;
        }

        //public string GetEntityTable(Type entidadNombre)
        //{
        //    return ObtenerNodoEntidadPor(entidadNombre).Attribute("tabla").Value.Trim();
        //}

        public string[] GetPropertyDBKeyAndKeyChild(Type nombreEntidad)
        {
            var nodoEntidad = ObtenerNodoEntidadPor(nombreEntidad);

            var props = nodoEntidad.Descendants("Propiedades").Descendants("Propiedad")
                        .Where(p => p.Attribute("esClaveHija") != null
                                    && bool.Parse(p.Attribute("esClaveHija").Value)).ToList();

            var resultado = new string[props.Count];

            for (int i = 0; i < props.Count; i++)
            {
                resultado[i] = props.ElementAt(i).Attribute("nombre").Value;
            }

            return resultado;
        }

        public XElement ObtenerNodoEntidadPor(Type nombreEntidad)
        {
            var nodoEntidad = GetMappingXmlFromContext(nombreEntidad).Descendants("Entidades").Descendants("Entidad")
                             .SingleOrDefault(e => e.Attribute("nombre").Value == nombreEntidad.Name);

            if (nodoEntidad == null)
                throw new Exception(string.Format(MensajeEntidadNoEncontrada, nombreEntidad.Name));

            return nodoEntidad;
        }

        private XDocument GetMappingXmlFromContext(Type entityName)
        {
            if (!Common.ExistsObjectInContext("mappingXML"))
            {
                Common.AddObjectInContext("mappingXML", XDocument.Load(GetXMLConfigurationPath(entityName)));
            }

            return (XDocument)Common.GetObjectInContext("mappingXML");
        }
    }
}
