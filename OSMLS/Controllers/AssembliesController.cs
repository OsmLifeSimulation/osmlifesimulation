using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssembliesController : ControllerBase
	{
		public AssembliesController(IModulesLibrary modulesLibrary)
		{
			_ModulesLibrary = modulesLibrary;
		}
		
		private readonly IModulesLibrary _ModulesLibrary;

		[HttpPost]
		public void PostAssembly(IFormFile assembly) => 
			_ModulesLibrary.LoadModules(assembly.OpenReadStream());
	}
}