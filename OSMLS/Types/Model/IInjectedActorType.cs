namespace OSMLS.Types.Model
{
	public interface IInjectedActorType : IInjectedType
	{
		public string GetCustomStyle();

		bool IsVisible { get; }
	}
}