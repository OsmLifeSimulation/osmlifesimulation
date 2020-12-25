using System;

namespace OSMLSGlobalLibrary.Modules
{
	/// <summary>
	/// Custom initialization order attribute to use with modules classes. Defines the initialization order of the module.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CustomInitializationOrderAttribute : Attribute
	{
		/// <summary>
		/// Module initialization order.
		/// For example, a module with an initialization order of 100 will be initialized earlier than a module with an initialization order of 1000.
		/// Negative values are allowed.
		/// </summary>
		public double InitializationOrder { get; }

		public CustomInitializationOrderAttribute(double initializationOrder = 0)
		{
			InitializationOrder = initializationOrder;
		}
	}
}