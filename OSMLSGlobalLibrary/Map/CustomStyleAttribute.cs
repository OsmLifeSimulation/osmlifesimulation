using System;

namespace OSMLSGlobalLibrary.Map
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomStyleAttribute : Attribute
    {
        private const string DefaultStyle =
            @"new ol.style.Style({
                image: new ol.style.Circle({
                    opacity: 1.0,
                    scale: 1.0,
                    radius: 3,
                    fill: new ol.style.Fill({
                      color: 'rgba(255, 255, 255, 0.4)'
                    }),
                    stroke: new ol.style.Stroke({
                      color: 'rgba(0, 0, 0, 0.4)',
                      width: 1
                    }),
                }),
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.4)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                })
            });
        ";

        public string Style { get; set; }

        public CustomStyleAttribute(string style = DefaultStyle)
        {
            Style = style;
        }
    }
}