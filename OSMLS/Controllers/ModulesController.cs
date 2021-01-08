using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;
using OSMLS.Services;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ModulesController
	{
		public ModulesController(ModulesLibrary modulesLibrary, ModelService modelService)
		{
			_ModulesLibrary = modulesLibrary;
			_ModelService = modelService;
		}

		private readonly ModulesLibrary _ModulesLibrary;
		private readonly ModelService _ModelService;

		[HttpGet]
		public IEnumerable<string> GetModules() => _ModulesLibrary.ModulesTypes.Select(type => type.FullName);

		[HttpPost("Model")]
		public void PostModelModule(string typeName)
		{
			_ModelService.Modules.Add(ModulesLibrary.GetType(typeName));
		}

		[HttpGet("Model")]
		public IEnumerable<string> GetModelModules() =>
			_ModelService.Modules.Select(type => type.FullName);

		[HttpDelete("Model")]
		public void DeleteModelModule(string typeName) =>
			_ModelService.Modules.Remove(ModulesLibrary.GetType(typeName));
	}
}