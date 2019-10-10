namespace SearchIndexBuilder.Processors
{
    public interface IConfigFileManager
    {
        string Backup(string filename);
        OperationConfig Load(string filename);
        void Save(string filename, OperationConfig config);
        long SizeOfSave(string filename);
    }
}