using System;

namespace OSMLSGlobalLibrary.Observable.Property
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ObservablePropertyAttribute : Attribute
	{
		public string Title { get; }

		public bool Editable { get; }

		public ObservablePropertyAttribute(string title, bool editable)
		{
			Title = title;
			Editable = editable;
		}
	}
}