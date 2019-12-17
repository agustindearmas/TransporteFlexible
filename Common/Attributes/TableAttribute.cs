using Common.Enums.Mappings;
using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string ProcedureName { get; set; }
        public DAOMapperType Mapping { get; set; }
        public Schema Schema { get; set; }
        public SQLType Motor { get; set; }
    }
}
