using System;

namespace OSMLSGlobalLibrary.Map
{
	/// <summary>
	/// Custom style attribute to use with geometry classes. Defines the style of the geometry.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomStyleAttribute : Attribute
	{
		/// <summary>
		/// Geometry style by default.
		/// </summary>
		private const string DefaultStyle =
			@"new style.Style({
                image: new style.Circle({
                    opacity: 1.0,
                    scale: 1.0,
                    radius: 3,
                    fill: new style.Fill({
                      color: 'rgba(255, 255, 255, 0.4)'
                    }),
                    stroke: new style.Stroke({
                      color: 'rgba(0, 0, 0, 0.4)',
                      width: 1
                    }),
                }),
                fill: new style.Fill({
                    color: 'rgba(255, 255, 255, 0.4)'
                }),
                stroke: new style.Stroke({
                    color: 'rgba(0, 0, 0, 0.4)',
                    width: 1
                })
            });
        ";

		/// <summary>
		/// Geometry style in javascript language to use from OpenLayers library.
		/// </summary>
		public string Style { get; set; }

		public CustomStyleAttribute(string style = DefaultStyle)
		{
			Style = style;
		}
	}
}