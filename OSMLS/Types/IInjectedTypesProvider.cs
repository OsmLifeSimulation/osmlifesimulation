using System;
using System.Collections.Generic;
using OSMLS.Types.Model;

namespace OSMLS.Types
{
	public interface IInjectedTypesProvider
	{
		IEnumerable<IInjectedType> GetTypes();

		class TypeAddedEventArgs : EventArgs
		{
			public IInjectedType Type { get; }

			public TypeAddedEventArgs(IInjectedType type)
			{
				Type = type;
			}
		}

		public delegate void TypeAddedEventHandler(object sender, TypeAddedEventArgs eventArgs);

		event TypeAddedEventHandler TypeAdded;
	}
}