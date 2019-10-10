namespace SearchIndexBuilder.Processors
{
    public interface IConfigFileManager
    {
        string RuntimeBackupFilename(string filename);
        string VerifyFilename(string filename);
        string Backup(string filename);
        OperationConfig Load(string filename);
        void Save(string filename, OperationConfig config);
        long SizeOfSave(string filename);
    }
}