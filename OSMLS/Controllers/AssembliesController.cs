using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;
using OSMLS.Types;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssembliesController : ControllerBase
	{
		public AssembliesController(IAssemblyLoader assemblyLoader)
		{
			_AssemblyLoader = assemblyLoader;
		}

		private readonly IAssemblyLoader _AssemblyLoader;

		[HttpPost]
		public void PostAssembly(IFormFile assembly) => 
			_AssemblyLoader.LoadAssembly(assembly.OpenReadStream());
	}
}