using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;

namespace OSMLSGlobalLibrary.Geometries.Observable.Implementation
{
	internal class ObservableCoordinateArraySequence : CoordinateArraySequence
	{
		public ObservableCoordinateArraySequence(Coordinate[] coordinates) : base(coordinates)
		{
		}

		public ObservableCoordinateArraySequence(int size, int dimension, int measures) : base(size, dimension,
			measures)
		{
		}

		public ObservableCoordinateArraySequence(CoordinateSequence coordinateSequence) : base(
			coordinateSequence)
		{
		}

		public event EventHandler OrdinateChanged = delegate { };

		public override void SetOrdinate(int index, int ordinateIndex, double value)
		{
			base.SetOrdinate(index, ordinateIndex, value);

			OrdinateChanged.Invoke(this, EventArgs.Empty);
		}
	}
}