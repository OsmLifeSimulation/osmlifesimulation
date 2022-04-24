using System;
using System.Collections.Generic;
using System.Reflection;

namespace OSMLS.Types.Model
{
	public abstract class InjectedType : IInjectedType
	{
		public InjectedType(Type systemType)
		{
			SystemType = systemType;
		}

		public Type SystemType { get; }

		public virtual string FullName => SystemType.FullName;

		public override bool Equals(object other) =>
			other != null && SystemType == (other as InjectedType)?.SystemType;

		public override int GetHashCode() => SystemType.GetHashCode();

		public virtual IEnumerable<PropertyInfo> GetProperties() => SystemType.GetProperties();
	}
}