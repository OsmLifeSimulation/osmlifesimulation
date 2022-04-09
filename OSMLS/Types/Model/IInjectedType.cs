using System;
using System.Collections.Generic;
using System.Reflection;

namespace OSMLS.Types.Model
{
	public interface IInjectedType
	{
		Type SystemType { get; }

		string FullName { get; }

		IEnumerable<PropertyInfo> GetProperties();
	}
}