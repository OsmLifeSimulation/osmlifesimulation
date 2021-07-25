using System;
using System.Reflection;

namespace OSMLS.Model
{
	public interface INotifyAssemblyAdded
	{
		class AssemblyAddedEventArgs : EventArgs
		{
			public Assembly Assembly { get; }

			public AssemblyAddedEventArgs(Assembly assembly)
			{
				Assembly = assembly;
			}
		}

		public delegate void AssemblyAddedEventHandler(object sender, AssemblyAddedEventArgs eventArgs);

		event AssemblyAddedEventHandler AssemblyAdded;
	}
}