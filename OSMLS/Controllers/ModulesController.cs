using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;
using OSMLS.Model.Modules;
using OSMLS.Services;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ModulesController
	{
		public ModulesController(IModulesLibrary modulesLibrary, IModelProvider modelProvider)
		{
			_ModulesLibrary = modulesLibrary;
			_ModelProvider = modelProvider;
		}

		private readonly IModulesLibrary _ModulesLibrary;
		private readonly IModelProvider _ModelProvider;

		[HttpGet]
		public IEnumerable<string> GetModules() => _ModulesLibrary.ModulesTypes.Select(type => type.FullName);

		[HttpPost("Model")]
		public void PostModelModule(string typeName)
		{
			_ModelProvider.ModulesTypes.Add(_ModulesLibrary.GetModuleType(typeName));
		}

		[HttpGet("Model")]
		public IEnumerable<string> GetModelModules() =>
			_ModelProvider.ModulesTypes.Select(type => type.FullName);

		[HttpDelete("Model")]
		public void DeleteModelModule(string typeName) =>
			_ModelProvider.ModulesTypes.Remove(_ModulesLibrary.GetModuleType(typeName));
	}
}