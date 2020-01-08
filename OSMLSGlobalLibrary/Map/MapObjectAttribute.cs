using System;

namespace OSMLSGlobalLibrary.Map
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapObjectAttribute : Attribute
    {
        public string Style { get; set; }

        public MapObjectAttribute(string style)
        {
            Style = style;
        }
    }
}