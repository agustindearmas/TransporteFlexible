using Common.Attributes;
using Common.Enums.Mappings;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetPrefix(this Schema value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            SatelliteNameAttribute attribute = Attribute.GetCustomAttribute(field, typeof(SatelliteNameAttribute)) as SatelliteNameAttribute;

            return attribute == null ? value.ToString() : "";//attribute.Prefix;
        }
        public static string GetName(this Schema value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            SatelliteNameAttribute attribute = Attribute.GetCustomAttribute(field, typeof(SatelliteNameAttribute)) as SatelliteNameAttribute;

            return attribute == null ? value.ToString() : "";//attribute.Prefix;
        }
    }
}
