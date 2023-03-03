using System;
using NetTopologySuite.Geometries;

namespace OSMLSGlobalLibrary.Observable.Geometries.Implementation
{
	/// <summary>
	/// Creates CoordinateSequences represented as an array of Coordinates.
	/// </summary>
	[Serializable]
	internal sealed class ObservableCoordinateSequenceFactory : CoordinateSequenceFactory
	{
		private ObservableCoordinateSequenceFactory() : base(Ordinates.XYZM)
		{
		}

		/// <summary>
		/// Returns the singleton instance of NotifyingOnChangeCoordinateArraySequenceFactory.
		/// </summary>
		public static ObservableCoordinateSequenceFactory Instance { get; } =
			new ObservableCoordinateSequenceFactory();

		/// <summary>
		///  Returns a NotifyingOnChangeCoordinateArraySequence based on the given array (the array is not copied).
		/// </summary>
		/// <param name="coordinates">the coordinates, which may not be null nor contain null elements.</param>
		/// <returns></returns>
		public override CoordinateSequence Create(Coordinate[] coordinates)
		{
			return new ObservableCoordinateArraySequence(coordinates);
		}

		public override CoordinateSequence Create(CoordinateSequence coordinateSequence)
		{
			return new ObservableCoordinateArraySequence(coordinateSequence);
		}

		public override CoordinateSequence Create(int size, int dimension, int measures)
		{
			var spatial = dimension - measures;

			if (measures > 1)
			{
				measures = 1; // clip measures
			}

			if (spatial > 3)
			{
				spatial = 3; // clip spatial dimension
				// throw new ArgumentException("spatial dimension must be <= 3");
			}

			if (spatial < 2)
			{
				spatial = 2; // handle bogus dimension
			}

			return new ObservableCoordinateArraySequence(size, spatial + measures, measures);
		}
	}
}