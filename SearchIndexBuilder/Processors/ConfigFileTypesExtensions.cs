namespace SearchIndexBuilder.Processors
{
    public static class ConfigFileTypesExtensions
    {
        public static IConfigFileManager Create(this ConfigFileTypes type, IConfigFileManager existing = null)
        {
            switch (type)
            {
                case ConfigFileTypes.GZip:
                    return new GZipStreamConfigFileManager();
                case ConfigFileTypes.Archive:
                    return new ZipArchiveConfigFileManager();
                default:
                    if (existing == null)
                    {
                        return new TextConfigFileManager();
                    }
                    break;
            }

            return existing;
        }
    }
}