using System.Linq;
using Moq;
using NUnit.Framework;
using OSMLS.Model.Modules;
using OSMLS.Tests.Mocks;
using OSMLS.Types;
using OSMLS.Types.Model;

namespace OSMLS.Tests.Model.Modules
{
	public class ModulesLibraryTest
	{
		private (
			Mock<IInjectedModuleType> osmlsModuleTypeMock,
			Mock<IInjectedType> nonOsmlsModuleTypeMock,
			Mock<IInjectedTypesProvider> typesProviderMock) ComposeTestModulesAndAssembliesProviderMocks()
		{
			var osmlsModuleTypeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();

			var nonOsmlsModuleTypeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedType>();

			var typesProviderMock = new Mock<IInjectedTypesProvider>();
			typesProviderMock
				.Setup(typesProvider => typesProvider.GetTypes())
				.Returns(new[] { osmlsModuleTypeMock.Object, nonOsmlsModuleTypeMock.Object })
				.Verifiable();

			return (osmlsModuleTypeMock, nonOsmlsModuleTypeMock, typesProviderMock);
		}

		[Test]
		public void ShouldReturnModulesTypesProperly()
		{
			var (osmlsModuleTypeMock, _, assembliesProviderMock) = ComposeTestModulesAndAssembliesProviderMocks();

			var modulesLibrary = new ModulesLibrary(assembliesProviderMock.Object);
			var actualModulesTypes = modulesLibrary.ModulesTypes.ToList();

			Assert.That(actualModulesTypes, Has.Count.EqualTo(1));
			Assert.That(actualModulesTypes, Contains.Item(osmlsModuleTypeMock.Object));
		}

		[Test]
		public void ShouldGetModuleTypeByNameProperly()
		{
			var (osmlsModuleTypeMock, _, assembliesProviderMock) = ComposeTestModulesAndAssembliesProviderMocks();

			var modulesLibrary = new ModulesLibrary(assembliesProviderMock.Object);
			var actualModuleType = modulesLibrary.GetModuleType(osmlsModuleTypeMock.Object.FullName);

			Assert.AreSame(osmlsModuleTypeMock.Object, actualModuleType);
		}

		[Test]
		public void ShouldThrowInvalidOperationExceptionWhenGettingNonExistingModuleTypeByName()
		{
			var (_, nonOsmlsModuleTypeMock, assembliesProviderMock) = ComposeTestModulesAndAssembliesProviderMocks();

			var modulesLibrary = new ModulesLibrary(assembliesProviderMock.Object);

			Assert.That(
				() => modulesLibrary.GetModuleType(nonOsmlsModuleTypeMock.Object.FullName),
				Throws.InvalidOperationException
			);
		}
	}
}