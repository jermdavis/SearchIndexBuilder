using System;
using System.IO;

namespace SearchIndexBuilder.Processors.Convert
{
    public class ConvertProcessor : BaseProcessor<ConvertOptions>
    {
        public static void RunProcess(ConvertOptions options)
        {
            var dp = new ConvertProcessor(options);
            dp.Run();
        }

        public ConvertProcessor(ConvertOptions options) : base(options)
        {
        }

        public override void Run()
        {
            base.OverrideFileType(_options.SourceFile);
            base.Run();

            var inFile = _configFileManager.VerifyFilename(_options.SourceFile);
            var cfg = _configFileManager.Load(inFile);

            if (cfg == null)
            {
                Console.WriteLine($"Error: Source config file '{inFile}' not found");
                return;
            }

            IConfigFileManager newFileManager = _options.TargetFileType.Create();

            var outFile = newFileManager.VerifyFilename(_options.TargetFile);
            if (File.Exists(outFile) && _options.Overwrite == false)
            {
                Console.WriteLine($"Target config file {outFile} exists.");
                Console.WriteLine($"Overwrite not specified so file will not be overwritten");
                return;
            }

            newFileManager.Save(newFileManager.VerifyFilename(outFile), cfg);

            Console.WriteLine($"Config from '{inFile}' converted to '{_options.TargetFileType}' format and saved as {outFile}");
        }
    }
}
