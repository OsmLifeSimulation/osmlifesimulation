using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSM
{
    static class Settings
    {
        public static PresetsXml Presets;

        public static void Init()
        {

            Presets = Constants.DeserializeXmlOrCreateNew<PresetsXml>(Constants.PresetsPath);
        }
    }
}
