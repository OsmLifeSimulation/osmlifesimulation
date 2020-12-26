using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssembliesController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<string> GetAssemblies()
		{
			throw new NotImplementedException();
		}

		[HttpPost]
		public void PostAssembly(IFormFile assembly)
		{
			throw new NotImplementedException();
		}
	}
}