using OSMLSGlobalLibrary.Modules;

namespace OSMLS.Types.Model
{
	public interface IInjectedModuleType : IInjectedType
	{
		IModule CreateInstance();

		double GetInitializationOrder();
	}
}