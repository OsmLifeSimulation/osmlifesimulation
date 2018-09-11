using Microsoft.Xna.Framework;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM
{
    public static class MathExtensions
    {
        public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.X, pointY = point.Y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.X;
            endY = endPoint.Y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.X; endY = endPoint.Y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }
        public static Vector2 LineCenter(Line line)
        {
            return new Vector2((line.Start.X + line.End.X) / 2, (line.Start.Y + line.End.Y) / 2);
        }
        public static double LineLength(Line line)
        {
            return Math.Sqrt(Math.Pow((line.End.Y - line.Start.Y), 2) + Math.Pow((line.End.X - line.Start.X), 2));
        }
        public static bool LinesIntersects(Line a, Line b)
        {
            return LinesIntersects(a.Start, a.End, b.Start, b.End);
        }
        // a1 is line1 start, a2 is line1 end, b1 is line2 start, b2 is line2 end
        public static bool LinesIntersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            return true;
        }

        private static double toRad(double val)
        {
            return val * (Math.PI / 180);
        }
        public static Vector2 LatLongToEastNorth(double latitude, double longitude)
        {
            //This will not work unless you have your lats and longs in decimal degrees.
            latitude = toRad(latitude);
            longitude = toRad(longitude);

            double a = 6377563.396, b = 6356256.910; // Airy 1830 major &amp; minor semi-axes
                                                     //double a = 6378137.0, b = 6356752.314245; WGS84 major &amp; minor semi-axes

            double F0 = 0.9996012717; // NatGrid scale factor on central meridian
            double lat0 = toRad(49);
            double lon0 = toRad(-2); // NatGrid true origin
            double N0 = -100000, E0 = 400000; // northing &amp; easting of true origin, metres
            double e2 = 1 - (b * b) / (a * a); // eccentricity squared
            double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;

            double cosLat = Math.Cos(latitude), sinLat = Math.Sin(latitude);
            double nu = a * F0 / Math.Sqrt(1 - e2 * sinLat * sinLat); // transverse radius of curvature
            double rho = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinLat * sinLat, 1.5); // meridional radius of curvature

            double eta2 = nu / rho - 1;

            double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (latitude - lat0);
            double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(latitude - lat0) * Math.Cos(latitude + lat0);
            double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (latitude - lat0)) * Math.Cos(2 * (latitude + lat0));
            double Md = (35 / 24) * n3 * Math.Sin(3 * (latitude - lat0)) * Math.Cos(3 * (latitude + lat0));
            double M = b * F0 * (Ma - Mb + Mc - Md); // meridional arc

            double cos3lat = cosLat * cosLat * cosLat;
            double cos5lat = cos3lat * cosLat * cosLat;
            double tan2lat = Math.Tan(latitude) * Math.Tan(latitude);
            double tan4lat = tan2lat * tan2lat;

            double I = M + N0;
            double II = (nu / 2) * sinLat * cosLat;
            double III = (nu / 24) * sinLat * cos3lat * (5 - tan2lat + 9 * eta2);
            double IIIA = (nu / 720) * sinLat * cos5lat * (61 - 58 * tan2lat + tan4lat);
            double IV = nu * cosLat;
            double V = (nu / 6) * cos3lat * (nu / rho - tan2lat);
            double VI = (nu / 120) * cos5lat * (5 - 18 * tan2lat + tan4lat + 14 * eta2 - 58 * tan2lat * eta2);

            double dLon = longitude - lon0;
            double dLon2 = dLon * dLon, dLon3 = dLon2 * dLon, dLon4 = dLon3 * dLon, dLon5 = dLon4 * dLon, dLon6 = dLon5 * dLon;

            double N = I + II * dLon2 + III * dLon4 + IIIA * dLon6; //This is the northing
            double E = E0 + IV * dLon + V * dLon3 + VI * dLon5; //This is the easting  

            return new Vector2(Convert.ToSingle(E), Convert.ToSingle(N));
        }

        //It is unacceptable to store these values here, need to store it for each vector of UTM coordinates
        //But another way we need to store a lot of useless data
        static int Zone;
        static char Letter;

        public static Vector2 Deg2UTM(NodeXml node)
        {
            return Deg2UTM(float.Parse(node.Lat), float.Parse(node.Lon));
        }
        public static Vector2 Deg2UTM(double lat, double lon)
        {
            double easting;
            double northing;
            int zone;
            char letter;

            zone = (int)Math.Floor(lon / 6 + 31);
            if (lat < -72)
                letter = 'C';
            else if (lat < -64)
                letter = 'D';
            else if (lat < -56)
                letter = 'E';
            else if (lat < -48)
                letter = 'F';
            else if (lat < -40)
                letter = 'G';
            else if (lat < -32)
                letter = 'H';
            else if (lat < -24)
                letter = 'J';
            else if (lat < -16)
                letter = 'K';
            else if (lat < -8)
                letter = 'L';
            else if (lat < 0)
                letter = 'M';
            else if (lat < 8)
                letter = 'N';
            else if (lat < 16)
                letter = 'P';
            else if (lat < 24)
                letter = 'Q';
            else if (lat < 32)
                letter = 'R';
            else if (lat < 40)
                letter = 'S';
            else if (lat < 48)
                letter = 'T';
            else if (lat < 56)
                letter = 'U';
            else if (lat < 64)
                letter = 'V';
            else if (lat < 72)
                letter = 'W';
            else
                letter = 'X';
            easting = 0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) * 0.9996 * 6399593.62 / Math.Pow((1 + Math.Pow(0.0820944379, 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)), 0.5) * (1 + Math.Pow(0.0820944379, 2) / 2 * Math.Pow((0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)))), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2) / 3) + 500000;
            easting = Math.Round(easting * 100) * 0.01;
            northing = (Math.Atan(Math.Tan(lat * Math.PI / 180) / Math.Cos((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) - lat * Math.PI / 180) * 0.9996 * 6399593.625 / Math.Sqrt(1 + 0.006739496742 * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) * (1 + 0.006739496742 / 2 * Math.Pow(0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)))), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) + 0.9996 * 6399593.625 * (lat * Math.PI / 180 - 0.005054622556 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + 4.258201531e-05 * (3 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 4 - 1.674057895e-07 * (5 * (3 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 4 + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 3);
            if (letter < 'M')
                northing = northing + 10000000;
            northing = Math.Round(northing * 100) * 0.01;

            Zone = zone;
            Letter = letter;

            return new Vector2(Convert.ToSingle(easting), Convert.ToSingle(northing));
        }

        public static Vector2 UTM2Deg(Vector2 UTM)
        {
            int zone = Zone;
            char letter = Letter;
            double easting = UTM.X;
            double northing = UTM.Y;
            double hem;
            if (letter > 'M')
                hem = 'N';
            else
                hem = 'S';
            double north;
            if (hem == 'S')
                north = northing - 10000000;
            else
                north = northing;
            var latitude = (north / 6366197.724 / 0.9996 + (1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) - 0.006739496742 * Math.Sin(north / 6366197.724 / 0.9996) * Math.Cos(north / 6366197.724 / 0.9996) * (Math.Atan(Math.Cos(Math.Atan((Math.Exp((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3)) - Math.Exp(-(easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3))) / 2 / Math.Cos((north - 0.9996 * 6399593.625 * (north / 6366197.724 / 0.9996 - 0.006739496742 * 3 / 4 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Pow(0.006739496742 * 3 / 4, 2) * 5 / 3 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 - Math.Pow(0.006739496742 * 3 / 4, 3) * 35 / 27 * (5 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 3)) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) + north / 6366197.724 / 0.9996))) * Math.Tan((north - 0.9996 * 6399593.625 * (north / 6366197.724 / 0.9996 - 0.006739496742 * 3 / 4 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Pow(0.006739496742 * 3 / 4, 2) * 5 / 3 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 - Math.Pow(0.006739496742 * 3 / 4, 3) * 35 / 27 * (5 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 3)) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) + north / 6366197.724 / 0.9996)) - north / 6366197.724 / 0.9996) * 3 / 2) * (Math.Atan(Math.Cos(Math.Atan((Math.Exp((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3)) - Math.Exp(-(easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3))) / 2 / Math.Cos((north - 0.9996 * 6399593.625 * (north / 6366197.724 / 0.9996 - 0.006739496742 * 3 / 4 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Pow(0.006739496742 * 3 / 4, 2) * 5 / 3 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 - Math.Pow(0.006739496742 * 3 / 4, 3) * 35 / 27 * (5 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 3)) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) + north / 6366197.724 / 0.9996))) * Math.Tan((north - 0.9996 * 6399593.625 * (north / 6366197.724 / 0.9996 - 0.006739496742 * 3 / 4 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Pow(0.006739496742 * 3 / 4, 2) * 5 / 3 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 - Math.Pow(0.006739496742 * 3 / 4, 3) * 35 / 27 * (5 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 3)) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) + north / 6366197.724 / 0.9996)) - north / 6366197.724 / 0.9996)) * 180 / Math.PI;
            latitude = Math.Round(latitude * 10000000);
            latitude = latitude / 10000000;
            var longitude = Math.Atan((Math.Exp((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3)) - Math.Exp(-(easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) / 3))) / 2 / Math.Cos((north - 0.9996 * 6399593.625 * (north / 6366197.724 / 0.9996 - 0.006739496742 * 3 / 4 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Pow(0.006739496742 * 3 / 4, 2) * 5 / 3 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 - Math.Pow(0.006739496742 * 3 / 4, 3) * 35 / 27 * (5 * (3 * (north / 6366197.724 / 0.9996 + Math.Sin(2 * north / 6366197.724 / 0.9996) / 2) + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 4 + Math.Sin(2 * north / 6366197.724 / 0.9996) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2) * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) / 3)) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))) * (1 - 0.006739496742 * Math.Pow((easting - 500000) / (0.9996 * 6399593.625 / Math.Sqrt((1 + 0.006739496742 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)))), 2) / 2 * Math.Pow(Math.Cos(north / 6366197.724 / 0.9996), 2)) + north / 6366197.724 / 0.9996)) * 180 / Math.PI + zone * 6 - 183;
            longitude = Math.Round(longitude * 10000000);
            longitude = longitude / 10000000;

            return new Vector2(Convert.ToSingle(latitude), Convert.ToSingle(longitude));
        }

        public static IEnumerable<T> AdjacentBottomRightElements<T>(List<List<T>> arr, int row, int column)
        {
            int rows = arr.Count;
            int columns = arr[0].Count;

            for (int j = row/* - 1*/; j <= row + 1; j++)
                for (int i = column/* - 1*/; i <= column + 1; i++)
                    if (i >= 0 && j >= 0 && i < columns && j < rows && !(j == row && i == column))
                        yield return arr[j][i];
        }

        public static IEnumerable<T> AdjacentElements<T>(T[,] arr, int row, int column)
        {
            int rows = arr.GetLength(0);
            int columns = arr.GetLength(1);

            for (int j = row - 1; j <= row + 1; j++)
                for (int i = column - 1; i <= column + 1; i++)
                    if (i >= 0 && j >= 0 && i < columns && j < rows && !(j == row && i == column))
                        yield return arr[j, i];
        }
    }
}
