using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM
{
    public static class Constants
    {
        //Correction by X and Y
        public const int 
            XCorr = 0, 
            YCorr = 0;

        //if this value is X, the grid lines will be at a distance of X pixels
        public const int GridFrequency = 20;
        public static Point AreaExtension { get; private set; } = new Point(1000, 1000); 

        //original size is 1
        public const float Resize = 1;

        public static Random rnd { get; private set; } = new Random();

        public const string OsmFilePath = @"maps/map.osm";
    }
}
