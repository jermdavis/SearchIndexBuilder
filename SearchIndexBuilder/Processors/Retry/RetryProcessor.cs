using System;
using System.IO;
using System.Linq;

namespace SearchIndexBuilder.Processors.Retry
{
    
    /// <summary>
    /// Generates a new config file from the errors in an already-processed config
    /// </summary>
    public class RetryProcessor : BaseProcessor<RetryOptions>
    {
        public static void RunProcess(RetryOptions option)
        {
            var rp = new RetryProcessor(option);
            rp.Run();
        }

        public RetryProcessor(RetryOptions options) : base(options)
        {
        }

        public override void Run()
        {
            base.Run();

            if (!File.Exists(_options.SourceFile))
            {
                Console.WriteLine($"The source config file '{_options.SourceFile}' was not found");
                return;
            }

            if (File.Exists(_options.TargetFile) && !_options.Overwrite)
            {
                Console.WriteLine($"The target config file already exists at {_options.TargetFile} and overwrite not allowed.");
                Console.WriteLine("It will not be overwritten.");
                return;
            }

            var cfg = _configFileManager.Load(_options.SourceFile);

            UpdateConfigData(cfg);

            _configFileManager.Save(_options.TargetFile, cfg);

            Console.WriteLine($"New config file {_options.TargetFile} generated with {cfg.Items.Count} items to process.");
        }

        private void UpdateConfigData(OperationConfig cfg)
        {
            var errorItems = cfg.Errors
                .Where(e => e.FailureType == FailureType.Error)
                .Select(e => e.Item);

            foreach (var itm in errorItems)
            {
                cfg.Items.Enqueue(itm);
            }

            cfg.Errors.Clear();
            cfg.Attempts = 0;
            cfg.Elapsed = TimeSpan.Zero;
            cfg.Processed.Clear();
        }
    }

}
