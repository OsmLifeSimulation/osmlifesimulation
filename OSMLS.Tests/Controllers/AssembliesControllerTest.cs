using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OSMLS.Controllers;
using OSMLS.Types;

namespace OSMLS.Tests.Controllers
{
	public class AssembliesControllerTest
	{
		[Test]
		public void ShouldPostAssemblyProperly()
		{
			var assemblyFileMock = new Mock<IFormFile>();
			assemblyFileMock
				.Setup(assemblyFile => assemblyFile.OpenReadStream())
				.Returns(Stream.Null)
				.Verifiable();

			var assemblyLoaderMock = new Mock<IAssemblyLoader>();
			assemblyLoaderMock.Setup(assemblyLoader => assemblyLoader.LoadAssembly(Stream.Null)).Verifiable();

			var assembliesController = new AssembliesController(assemblyLoaderMock.Object);
			assembliesController.PostAssembly(assemblyFileMock.Object);

			assemblyFileMock.Verify(assemblyFile =>
					assemblyFile.OpenReadStream(),
				Times.Once
			);

			assemblyLoaderMock.Verify(assemblyLoader =>
					assemblyLoader.LoadAssembly(Stream.Null),
				Times.Once
			);
		}
	}
}