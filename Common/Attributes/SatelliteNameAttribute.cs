using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SatelliteNameAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
