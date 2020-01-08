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
