using Moq;
using NUnit.Framework;
using OSMLS.Controllers;
using OSMLS.Services;

namespace OSMLS.Tests.Controllers
{
	public class StateControllerTest
	{
		[Test]
		public void ShouldGetStateProperlyWhenModelServiceIsActive()
		{
			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock.Setup(modelService => modelService.IsPaused).Returns(false);
			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(false);

			var stateController = new StateController(modelServiceMock.Object);
			var state = stateController.GetState();

			Assert.AreEqual(StateController.State.Active, state);
		}

		[Test]
		public void ShouldGetStateProperlyWhenModelServiceIsPaused()
		{
			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock.Setup(modelService => modelService.IsPaused).Returns(true);
			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(false);

			var stateController = new StateController(modelServiceMock.Object);
			var state = stateController.GetState();

			Assert.AreEqual(StateController.State.Paused, state);
		}

		[Test]
		public void ShouldGetStateProperlyWhenModelServiceIsStopped()
		{
			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock.Setup(modelService => modelService.IsPaused).Returns(false);
			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(true);

			var stateController = new StateController(modelServiceMock.Object);
			var state = stateController.GetState();

			Assert.AreEqual(StateController.State.Stopped, state);
		}

		[Test]
		public void ShouldGetStateProperlyWhenModelServiceIsPausedAndStopped()
		{
			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock.Setup(modelService => modelService.IsPaused).Returns(true);
			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(true);

			var stateController = new StateController(modelServiceMock.Object);
			var state = stateController.GetState();

			Assert.AreEqual(StateController.State.Stopped, state);
		}

		[Test]
		public void ShouldPutActiveStateProperlyWhenModelServiceIsStopped()
		{
			var modelServiceMock = new Mock<IModelService>();

			modelServiceMock.Setup(modelService => modelService.StartAsync(default)).Verifiable();
			modelServiceMock.SetupSet(modelService => modelService.IsPaused = false).Verifiable();

			modelServiceMock.Setup(modelService => modelService.IsPaused).Returns(true);
			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(true);

			var stateController = new StateController(modelServiceMock.Object);
			stateController.PutState(StateController.State.Active);

			modelServiceMock.Verify(modelService =>
					modelService.StartAsync(default),
				Times.Once
			);

			modelServiceMock.VerifySet(modelService =>
					modelService.IsPaused = false,
				Times.Once
			);
		}

		[Test]
		public void ShouldPutActiveStateProperlyWhenModelServiceIsActiveOrPaused()
		{
			var modelServiceMock = new Mock<IModelService>();

			modelServiceMock.SetupSet(modelService => modelService.IsPaused = false).Verifiable();

			modelServiceMock.Setup(modelService => modelService.IsStopped).Returns(false);

			var stateController = new StateController(modelServiceMock.Object);
			stateController.PutState(StateController.State.Active);

			modelServiceMock.VerifySet(modelService =>
					modelService.IsPaused = false,
				Times.Once
			);
		}

		[Test]
		public void ShouldPutPausedStateProperly()
		{
			var modelServiceMock = new Mock<IModelService>();

			modelServiceMock.SetupSet(modelService => modelService.IsPaused = true).Verifiable();

			var stateController = new StateController(modelServiceMock.Object);
			stateController.PutState(StateController.State.Paused);

			modelServiceMock.VerifySet(modelService =>
					modelService.IsPaused = true,
				Times.Once
			);
		}

		[Test]
		public void ShouldPutStoppedStateProperly()
		{
			var modelServiceMock = new Mock<IModelService>();

			modelServiceMock.Setup(modelService => modelService.StopAsync(default)).Verifiable();

			var stateController = new StateController(modelServiceMock.Object);
			stateController.PutState(StateController.State.Stopped);

			modelServiceMock.Verify(modelService =>
					modelService.StopAsync(default),
				Times.Once
			);
		}
	}
}