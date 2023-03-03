using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using OSMLS.Model;
using OSMLS.Tests.Mocks;
using OSMLS.Types.Model;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Modules;
using Range = Moq.Range;

namespace OSMLS.Tests.Model
{
	public class ModelProviderTest
	{
		private readonly string _ExpectedOsmFilePath = $"{AppContext.BaseDirectory}/map.osm";

		private static (Mock<IInjectedModuleType>, Mock<IModule>) ComposeModuleTypeAndModuleMocks(
			int initializationOrder = 0)
		{
			var moduleMock = new Mock<IModule>();
			moduleMock
				.Setup(module => module.Update(It.IsAny<long>()))
				.Verifiable();

			var moduleTypeMock = MocksComposer.ComposeInjectedTypeMock<IInjectedModuleType>();
			moduleTypeMock
				.Setup(moduleType => moduleType.CreateInstance())
				.Returns(moduleMock.Object);
			moduleTypeMock
				.Setup(moduleType => moduleType.GetInitializationOrder())
				.Returns(initializationOrder);

			return (moduleTypeMock, moduleMock);
		}

		[Test]
		public async Task ShouldStartProperly()
		{
			var mapObjectsCollectionMock = new Mock<IInheritanceTreeCollection<Geometry>>();

			var moduleInitializationCallsCount = 0;

			var (module1TypeMock, module1Mock) = ComposeModuleTypeAndModuleMocks(2);
			module1Mock.Setup(module => module.Initialize(
				It.IsAny<string>(),
				It.IsAny<IImmutableDictionary<Type, IModule>>(),
				It.IsAny<IInheritanceTreeCollection<Geometry>>()
			)).Callback(() => Assert.That(moduleInitializationCallsCount++, Is.EqualTo(1))).Verifiable();

			var (module2TypeMock, module2Mock) = ComposeModuleTypeAndModuleMocks(1);
			module2Mock.Setup(module => module.Initialize(
				It.IsAny<string>(),
				It.IsAny<IImmutableDictionary<Type, IModule>>(),
				It.IsAny<IInheritanceTreeCollection<Geometry>>()
			)).Callback(() => Assert.That(moduleInitializationCallsCount++, Is.EqualTo(0))).Verifiable();

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);

			modelProvider.ModulesTypes.Add(module1TypeMock.Object);
			modelProvider.ModulesTypes.Add(module2TypeMock.Object);

			Assert.That(modelProvider.IsStopped, Is.True);

			var actualModulesInitialized = new List<INotifyModuleInitialized.ModuleInitializedEventArgs>();
			modelProvider.ModuleInitialized += (_, args) => actualModulesInitialized.Add(args);

			await modelProvider.StartAsync(CancellationToken.None);

			Assert.That(modelProvider.IsStopped, Is.False);

			module1Mock.Verify(module => module.Initialize(
				_ExpectedOsmFilePath,
				It.Is<ImmutableDictionary<Type, IModule>>(dictionary =>
					dictionary.Contains(module1TypeMock.Object.SystemType, module1Mock.Object) &&
					dictionary.Contains(module2TypeMock.Object.SystemType, module2Mock.Object) &&
					dictionary.Count == 2),
				mapObjectsCollectionMock.Object
			));

			module2Mock.Verify(module => module.Initialize(
				_ExpectedOsmFilePath,
				It.Is<ImmutableDictionary<Type, IModule>>(dictionary =>
					dictionary.Contains(module1TypeMock.Object.SystemType, module1Mock.Object) &&
					dictionary.Contains(module2TypeMock.Object.SystemType, module2Mock.Object) &&
					dictionary.Count == 2),
				mapObjectsCollectionMock.Object
			));

			Assert.That(
				actualModulesInitialized.Select(args => args.Module),
				Has.Exactly(2).AnyOf(module1Mock.Object, module2Mock.Object)
			);
		}

		[Test]
		public async Task ShouldThrowInvalidOperationExceptionOnStartingAlreadyStartedModel()
		{
			var mapObjectsCollectionMock = new Mock<IInheritanceTreeCollection<Geometry>>();

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);

			await modelProvider.StartAsync(CancellationToken.None);
			Assert.ThrowsAsync<InvalidOperationException>(() => modelProvider.StartAsync(CancellationToken.None));
		}

		[Test]
		public async Task ShouldUpdateModulesPerSecondWhileStartedProperly()
		{
			var mapObjectsCollectionMock = new Mock<IInheritanceTreeCollection<Geometry>>();

			var (moduleTypeMock, moduleMock) = ComposeModuleTypeAndModuleMocks();

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);
			modelProvider.ModulesTypes.Add(moduleTypeMock.Object);

			await modelProvider.StartAsync(CancellationToken.None);

			const int secondsOfWaiting = 10;
			Thread.Sleep(secondsOfWaiting * 1000);

			moduleMock.Verify(
				module => module.Update(It.IsAny<long>()),
				Times.Between(secondsOfWaiting - 1, secondsOfWaiting + 1, Range.Inclusive)
			);
		}

		[Test]
		public async Task ShouldNotUpdateModulesPerSecondWhilePausedProperly()
		{
			var mapObjectsCollectionMock = new Mock<IInheritanceTreeCollection<Geometry>>();

			var (moduleTypeMock, moduleMock) = ComposeModuleTypeAndModuleMocks();

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);
			modelProvider.ModulesTypes.Add(moduleTypeMock.Object);

			modelProvider.IsPaused = true;
			await modelProvider.StartAsync(CancellationToken.None);

			const int secondsOfWaiting = 10;
			Thread.Sleep(secondsOfWaiting * 1000);

			moduleMock.Verify(
				module => module.Update(It.IsAny<long>()),
				Times.Never
			);
		}

		[Test]
		public async Task ShouldStopProperly()
		{
			var testGeometry = new Point(new Coordinate());
			var mapObjectsCollectionMock = MocksComposer.ComposeMapObjectsCollectionMock(testGeometry);
			mapObjectsCollectionMock
				.Setup(mapObjectsCollection => mapObjectsCollection.Remove(testGeometry))
				.Verifiable();

			var (moduleTypeMock, moduleMock) = ComposeModuleTypeAndModuleMocks();
			moduleMock.Setup(module => module.Initialize(
				It.IsAny<string>(),
				It.IsAny<IImmutableDictionary<Type, IModule>>(),
				It.IsAny<IInheritanceTreeCollection<Geometry>>()
			));

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);

			modelProvider.ModulesTypes.Add(moduleTypeMock.Object);

			await modelProvider.StartAsync(CancellationToken.None);
			await modelProvider.StopAsync(CancellationToken.None);

			Assert.That(modelProvider.IsStopped, Is.True);

			mapObjectsCollectionMock
				.Verify(mapObjectsCollection => mapObjectsCollection.Remove(testGeometry));

			Assert.That(modelProvider.TypesToInitializedModules, Is.Empty);
		}

		[Test]
		public void ShouldNotThrowOnDispose()
		{
			var mapObjectsCollectionMock = new Mock<IInheritanceTreeCollection<Geometry>>();

			var modelProvider = new ModelProvider(mapObjectsCollectionMock.Object);
			Assert.DoesNotThrow(() => modelProvider.Dispose());
		}
	}
}