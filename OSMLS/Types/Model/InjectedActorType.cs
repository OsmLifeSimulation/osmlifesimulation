using System;
using System.Reflection;
using OSMLSGlobalLibrary.Map;

namespace OSMLS.Types.Model
{
	public class InjectedActorType : InjectedType, IInjectedActorType
	{
		public InjectedActorType(Type systemType) : base(systemType)
		{
		}

		private CustomStyleAttribute GetCustomStyleAttribute() =>
			(CustomStyleAttribute)SystemType.GetCustomAttribute(typeof(CustomStyleAttribute));

		public string GetCustomStyle() => GetCustomStyleAttribute().Style;

		public virtual bool IsVisible => GetCustomStyleAttribute() != null;
	}
}