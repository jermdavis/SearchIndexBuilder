﻿using System.IO;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Reading and writing of config file data
    /// </summary>
    public class TextConfigFileManager : CoreConfigFileManager
    {
        public override OperationConfig Load(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                return null;
            }

            using (var file = System.IO.File.OpenText(filename))
            {
                return ExtractConfig(file);
            }
        }

        public override void Save(string filename, OperationConfig config)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                WriteConfig(file, config);
            }
        }   
    }
}
