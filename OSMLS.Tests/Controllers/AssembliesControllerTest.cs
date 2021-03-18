using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OSMLS.Controllers;
using OSMLS.Model;

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

			var modulesLibraryMock = new Mock<IModulesLibrary>();
			modulesLibraryMock.Setup(modulesLibrary => modulesLibrary.LoadModules(Stream.Null)).Verifiable();

			var assembliesController = new AssembliesController(modulesLibraryMock.Object);
			assembliesController.PostAssembly(assemblyFileMock.Object);

			assemblyFileMock.Verify(assemblyFile =>
					assemblyFile.OpenReadStream(),
				Times.Once
			);

			modulesLibraryMock.Verify(modulesLibrary =>
					modulesLibrary.LoadModules(Stream.Null),
				Times.Once
			);
		}
	}
}