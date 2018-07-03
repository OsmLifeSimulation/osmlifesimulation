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

        //original size is 1
        public const float Resize = 1;

        public static Random rnd = new Random();

        public const string OsmFilePath = @"maps/map.osm";
    }
}
