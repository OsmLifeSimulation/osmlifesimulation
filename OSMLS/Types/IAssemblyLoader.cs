using System.IO;

namespace OSMLS.Types
{
	public interface IAssemblyLoader
	{
		void LoadAssembly(Stream assemblyStream);
	}
}