using Common.Attributes;
using DataAccess.Interfaces;
using System;
using System.Data.Common;
using System.Linq;

namespace DataAccess.Concrete
{
    public class EntityMapper : IMapper
    {
        private const string MensajeEntidadNoEncontrada = "No se encontro el atributo Table en la entidad '{0}'";
        private const string MensajePropiedadDuplicada = "La propiedad '{0}' de la entidad '{1}' esta presente en mas de un atributo.";
        private const string MensajeAtributoDuplicado = "El atributo '{0}' de la entidad '{1}' esta presente en mas de un atributo.";

        public string GetPropertyName(DbParameter parametro, Type TypeEntidad)
        {
            string campo = parametro.ParameterName.ToString().Replace("@", "");
            return GetPropertyName(campo, TypeEntidad);
        }

        public string GetPropertyName(string campo, Type TypeEntidad)
        {
            var split = campo.Split('.');

            if (split.Length == 1)
            {
                var Props = TypeEntidad.GetProperties().SelectMany(propertyInfo => Attribute.GetCustomAttributes(propertyInfo, typeof(NameEntityAttribute)).Select(attribute => (NameEntityAttribute)attribute))
                .Where(Att =>
                {
                    return (Att != null && Att.IdEntity == campo);
                });

                if (!Props.Any()) return string.Empty;

                if (Props.Count() > 1)
                    throw new Exception(string.Format(MensajeAtributoDuplicado, campo, TypeEntidad.Name));

                return Props.ElementAt(0).NameEntity.Trim();
            }
            else
            {
                var typePropiedad = TypeEntidad.GetProperty(split[0]).PropertyType;
                string prop = split.Skip(1).Aggregate((x, y) => x + "." + y);
                return split[0] + "." + GetPropertyName(prop, typePropiedad);
            }

        }

        public string GetAttributeDBName(string propiedad, Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => Attribute.GetCustomAttributes(x, typeof(NameEntityAttribute)).Select(a => (NameEntityAttribute)a))
                .Where(Att =>
                {
                    return (Att != null && Att.NameEntity == propiedad);
                });

            if (!Props.Any()) return string.Empty;

            if (Props.Count() > 1)
                throw new Exception(string.Format(MensajePropiedadDuplicada, propiedad, TypeEntidad.Name));

            return Props.ElementAt(0).IdEntity.Trim();
        }

        public string[] GetPropertyDBKey(Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => Attribute.GetCustomAttributes(x, typeof(NameEntityAttribute)).Select(a => (NameEntityAttribute)a))
                .Where(Att =>
                {
                    return (Att != null && Convert.ToBoolean(Att.IsPrimaryKey));
                });


            var resultado = new string[Props.Count()];

            for (int i = 0; i < Props.Count(); i++)
            {
                resultado[i] = Props.ElementAt(i).NameEntity.Trim();
            }

            return resultado;
        }

        //public string GetEntityTable(Type TypeEntidad)
        //{
        //    var A = (TableAttribute)Attribute.GetCustomAttribute(TypeEntidad, typeof(TableAttribute));

        //    if (A == null)
        //        throw new Exception(string.Format(MensajeEntidadNoEncontrada, TypeEntidad.Name));

        //    return A.TableName.Trim();
        //}

        public string[] GetPropertyDBKeyAndKeyChild(Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => Attribute.GetCustomAttributes(x, typeof(NameEntityAttribute)).Select(a => (NameEntityAttribute)a))
               .Where(Att =>
               {
                   return (Att != null && Convert.ToBoolean(Att.IsForeingKey));
               });

            var resultado = new string[Props.Count()];

            for (int i = 0; i < Props.Count(); i++)
            {
                resultado[i] = Props.ElementAt(i).NameEntity.Trim();
            }

            return resultado;
        }
    }
}
