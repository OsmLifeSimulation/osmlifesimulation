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
		public ModulesController(IModulesLibrary modulesLibrary, IModelService modelService)
		{
			_ModulesLibrary = modulesLibrary;
			_ModelService = modelService;
		}

		private readonly IModulesLibrary _ModulesLibrary;
		private readonly IModelService _ModelService;

		[HttpGet]
		public IEnumerable<string> GetModules() => _ModulesLibrary.ModulesTypes.Select(type => type.FullName);

		[HttpPost("Model")]
		public void PostModelModule(string typeName)
		{
			_ModelService.ModulesTypes.Add(_ModulesLibrary.GetType(typeName));
		}

		[HttpGet("Model")]
		public IEnumerable<string> GetModelModules() =>
			_ModelService.ModulesTypes.Select(type => type.FullName);

		[HttpDelete("Model")]
		public void DeleteModelModule(string typeName) =>
			_ModelService.ModulesTypes.Remove(_ModulesLibrary.GetType(typeName));
	}
}