using System;
using Microsoft.AspNetCore.Mvc;

namespace OSMLS.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class StateController : ControllerBase
	{
		public enum State
		{
			Active,
			Paused,
			Stopped
		}

		[HttpGet]
		public State GetState()
		{
			throw new NotImplementedException();
		}

		[HttpPut]
		public void PutState(State state)
		{
			throw new NotImplementedException();
		}
	}
}