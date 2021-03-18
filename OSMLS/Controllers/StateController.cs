using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

		public StateController(IModelService modelService)
		{
			_ModelService = modelService;
		}

		private readonly IModelService _ModelService;

		[HttpGet]
		public State GetState() =>
			_ModelService.IsStopped ? State.Stopped :
			_ModelService.IsPaused ? State.Paused :
			State.Active;

		[HttpPut]
		public async Task PutState(State state, CancellationToken cancellationToken = default)
		{
			switch (state)
			{
				case State.Active:
					if (_ModelService.IsStopped)
						await _ModelService.StartAsync(cancellationToken);

					_ModelService.IsPaused = false;

					break;

				case State.Paused:
					_ModelService.IsPaused = true;

					break;

				case State.Stopped:
					await _ModelService.StopAsync(cancellationToken);

					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}