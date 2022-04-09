using System;
using System.ComponentModel;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Observable.Geometries.Actor;

namespace OSMLS.Tests.Mocks
{
	public class TestGeometryActor : Point, IActor
	{
		public TestGeometryActor() : base(new Coordinate())
		{
		}

		public event EventHandler CoordinatesChanged
		{
			add => throw new NotSupportedException();
			remove => throw new NotSupportedException();
		}

		public virtual event PropertyChangedEventHandler PropertyChanged
		{
			add => throw new NotSupportedException();
			remove => throw new NotSupportedException();
		}

		// ReSharper disable once UnassignedGetOnlyAutoProperty
		public virtual Guid Id { get; } = Guid.NewGuid();
	}
}