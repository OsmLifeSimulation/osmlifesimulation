using System;

namespace OSMLSGlobalLibrary
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomStyleAttribute : Attribute
    {
        public string Style { get; set; }

        public CustomStyleAttribute(string style)
        {
            Style = style;
        }
    }
}