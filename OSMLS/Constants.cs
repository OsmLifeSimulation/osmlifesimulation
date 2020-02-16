using System;
using System.IO;
using System.Xml.Serialization;

namespace OSMLS
{
    public static class Constants
    {
        public const string SettingsDirectoryPath = @"settings/";
        public const string PresetsFilePath = SettingsDirectoryPath + "Presets";

        public const string ModulesDirectoryPath = @"modules/";

        public static T DeserializeXmlOrCreateNew<T>(string path)
        {
            try
            {
                return DeserializeXml<T>(path);
            }
            catch (Exception)
            {
                var obj = (T)Activator.CreateInstance(typeof(T), new object[] { });
                SerializeXml(path, obj);
                return obj;
            }
        }

        public static void SerializeXml<T>(string path, T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamWriter(path);
            serializer.Serialize(reader, obj);
            reader.Close();
        }
        public static T DeserializeXml<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamReader(path);
            var obj = (T)serializer.Deserialize(reader);
            reader.Close();

            return obj;
        }
    }
}
