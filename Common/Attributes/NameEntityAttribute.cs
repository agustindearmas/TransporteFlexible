using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class NameEntityAttribute : Attribute
    {
        /// <summary>
        /// Nombre de la Columna en Base de Datos
        /// </summary>
        public string IdEntity { get; set; }

        /// <summary>
        /// Nombre del campo en la entidad, usa . para indicar campos subrogados
        /// <para/>Ej: 
        /// <para/>"Fecha" -> mapea Fecha
        /// <para/>"Hijo.Fecha" -> mapea Fecha dentro de la entidad Hijo
        /// </summary>
        public string NameEntity { get; set; }

        /// <summary>
        /// Indica si el campo es la PK de la tabla.
        /// <para/>Distingue los procesos de Update y Create
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Si la tabla tiene una PK que es a la vez FK
        /// <para/>usar este atributo en la relacion y no colocar IdEntity.
        /// <para/>Ej:
        /// <para/>[NameEntity(IdEntity = "per_id", NameEntity="Id", IsPrimaryKey = true)]
        /// <para/>public int? Id { get; set; }
        /// 
        /// <para/>[NameEntity( NameEntity = "Persona.Id", IsForeingKey = true )]
        /// <para/>public Persona Persona { get; set; }
        /// <para/>
        /// <para/>Para inserts, rellenar el campo Persona.Id solamente
        /// <para/>Para updates, rellerar el campo Id
        /// </summary>
        public bool IsForeingKey { get; set; }
    }
}
