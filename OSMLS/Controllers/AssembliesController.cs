using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssembliesController : ControllerBase
	{
		public AssembliesController(ModulesLibrary modulesLibrary)
		{
			_ModulesLibrary = modulesLibrary;
		}
		
		private readonly ModulesLibrary _ModulesLibrary;

		[HttpPost]
		public void PostAssembly(IFormFile assembly) => 
			_ModulesLibrary.LoadModules(assembly.OpenReadStream());
	}
}