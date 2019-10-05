using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.Processors.Retry
{

    [Verb("retry", HelpText = "Allows you to convert a finished config with errors into a new config to reprocess the errors.")]
    public class RetryOptions : CoreOptions
    {
        [Option('s', "source", Required = false, HelpText = "The file to read the config json from")]
        public string SourceFile { get; set; } = "config.json";

        [Option('t', "target", Required = false, HelpText = "The file to write the config to")]
        public string TargetFile { get; set; } = "retry-config.json";

        [Option('o', "overwrite", Required = false, HelpText = "If the target config file exists, should it be overwritten?")]
        public bool Overwrite { get; set; } = false;
    }

}
