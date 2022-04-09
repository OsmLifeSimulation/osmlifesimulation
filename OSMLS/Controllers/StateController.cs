using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSMLS.Model;
using OSMLS.Services;

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

		public StateController(IModelProvider modelProvider)
		{
			_ModelProvider = modelProvider;
		}

		private readonly IModelProvider _ModelProvider;

		[HttpGet]
		public State GetState() =>
			_ModelProvider.IsStopped ? State.Stopped :
			_ModelProvider.IsPaused ? State.Paused :
			State.Active;

		[HttpPut]
		public async Task PutState(State state, CancellationToken cancellationToken = default)
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (state)
			{
				case State.Active:
					if (_ModelProvider.IsStopped)
						await _ModelProvider.StartAsync(cancellationToken);

					_ModelProvider.IsPaused = false;

					break;

				case State.Paused:
					_ModelProvider.IsPaused = true;

					break;

				case State.Stopped:
					await _ModelProvider.StopAsync(cancellationToken);

					break;
			}
		}
	}
}