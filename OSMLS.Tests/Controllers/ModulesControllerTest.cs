﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OSMLS.Controllers;
using OSMLS.Model;
using OSMLS.Services;

namespace OSMLS.Tests.Controllers
{
	public class ModulesControllerTest
	{
		private IList<Type> ComposeTestModulesTypes() =>
			new List<Type> {typeof(string), typeof(int), typeof(ModulesControllerTest)};

		[Test]
		public void ShouldGetModulesProperly()
		{
			var testModulesTypes = ComposeTestModulesTypes();

			var modulesLibraryMock = new Mock<IModulesLibrary>();
			modulesLibraryMock.Setup(modulesLibrary => modulesLibrary.ModulesTypes).Returns(testModulesTypes);

			var modulesController = new ModulesController(modulesLibraryMock.Object, null);
			var modulesTypes = modulesController.GetModules();

			CollectionAssert.AreEqual(testModulesTypes.Select(type => type.FullName), modulesTypes);
		}

		[Test]
		public void ShouldPostModelModuleProperly()
		{
			var testModuleType = typeof(string);
			var testModuleTypeName = testModuleType.FullName;

			var modulesLibraryMock = new Mock<IModulesLibrary>();
			modulesLibraryMock
				.Setup(modulesLibrary => modulesLibrary.GetType(testModuleTypeName))
				.Returns(testModuleType);

			var modelServiceModulesMock = new Mock<IList<Type>>();
			modelServiceModulesMock
				.Setup(modelServiceModules => modelServiceModules.Add(testModuleType))
				.Verifiable();

			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock
				.Setup(modelService => modelService.ModulesTypes)
				.Returns(modelServiceModulesMock.Object);

			var modulesController = new ModulesController(modulesLibraryMock.Object, modelServiceMock.Object);
			modulesController.PostModelModule(testModuleTypeName);

			modelServiceModulesMock.Verify(modelServiceModules =>
					modelServiceModules.Add(testModuleType),
				Times.Once
			);
		}

		[Test]
		public void ShouldGetModelModulesProperly()
		{
			var testModulesTypes = ComposeTestModulesTypes();

			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock.Setup(modulesLibrary => modulesLibrary.ModulesTypes).Returns(testModulesTypes);

			var modulesController = new ModulesController(null, modelServiceMock.Object);
			var modelModulesTypes = modulesController.GetModelModules();

			CollectionAssert.AreEqual(testModulesTypes.Select(type => type.FullName), modelModulesTypes);
		}

		[Test]
		public void ShouldDeleteModelModuleProperly()
		{
			var testModuleType = typeof(string);
			var testModuleTypeName = testModuleType.FullName;

			var modulesLibraryMock = new Mock<IModulesLibrary>();
			modulesLibraryMock
				.Setup(modulesLibrary => modulesLibrary.GetType(testModuleTypeName))
				.Returns(testModuleType);

			var modelServiceModulesMock = new Mock<IList<Type>>();
			modelServiceModulesMock
				.Setup(modelServiceModules => modelServiceModules.Remove(testModuleType))
				.Verifiable();

			var modelServiceMock = new Mock<IModelService>();
			modelServiceMock
				.Setup(modelService => modelService.ModulesTypes)
				.Returns(modelServiceModulesMock.Object);

			var modulesController = new ModulesController(modulesLibraryMock.Object, modelServiceMock.Object);
			modulesController.DeleteModelModule(testModuleTypeName);

			modelServiceModulesMock.Verify(modelServiceModules =>
					modelServiceModules.Remove(testModuleType),
				Times.Once
			);
		}
	}
}