using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSM
{
    public static class Constants
    {
        //Correction by X and Y
        public const int
            XCorr = 0,
            YCorr = 0;

        public static Point AreaExtension { get; private set; } = new Point(1000, 1000);

        //original size is 1
        public const float Resize = 1;

        public static Random Rnd { get; private set; } = new Random();

        public const string OsmFolderPath = @"maps/";

        const string SettingsFolderPath = @"settings/";
        public const string PresetsPath = SettingsFolderPath + "Presets";
        public const string ControlsPath = SettingsFolderPath + "Controls";

        public const string ModulesPath = @"modules/";
        public const string ModuleIdentifier = "MainModule";

        public static T DeserializeXmlOrCreateNew<T>(string path)
        {
            try
            {
                return DeserializeXml<T>(path);
            }
            catch (Exception)
            {
                var obj = (T)Activator.CreateInstance(typeof(T), new object[] {});
                SerializeXml(path, obj);
                return obj;
            }
        }

        public static void SerializeXml<T>(string path, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter reader = new StreamWriter(path);
            serializer.Serialize(reader, obj);
            reader.Close();
        }
        public static T DeserializeXml<T>(string path) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(path);
            T obj = (T)serializer.Deserialize(reader);
            reader.Close();

            return obj;
        }
    }
}
