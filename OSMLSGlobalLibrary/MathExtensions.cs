using NetTopologySuite.Geometries;
using System;

namespace OSMLSGlobalLibrary
{
    public static class MathExtensions
    {
        /// <summary>
        /// Converts Lat/Lon coordinates (EPSG:4326) to spherical mercator projection (EPSG:3857).
        /// </summary>
        /// <param name="latitude">Latitude coordinate.</param>
        /// <param name="longitude">Longitude coordinate.</param>
        /// <returns>Spherical mercator projection.</returns>
        public static Coordinate LatLonToSpherMerc(double latitude, double longitude)
        {
            latitude *= (Math.PI / 180);
            longitude *= (Math.PI / 180);

            double a = 6378137.0;

            var x = a * longitude;
            var y = a * Math.Log(Math.Tan((Math.PI / 4) + (latitude / 2)));

            return new Coordinate(x, y);
        }
    }
}
