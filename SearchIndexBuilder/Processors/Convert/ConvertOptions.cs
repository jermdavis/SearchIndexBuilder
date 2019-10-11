using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.Processors.Convert
{

    [Verb("convert", HelpText = "Converts config files between formats")]
    public class ConvertOptions : CoreOptions
    {
        [Option('s', "source", Required = true, HelpText = "The source config file to read in.")]
        public string SourceFile { get; set; }

        [Option('t', "target", Required = true, HelpText = "The target config file to write out.")]
        public string TargetFile { get; set; }

        [Option('w', "writeformat", Required = true, HelpText = "The format to use for the target config file.")]
        public ConfigFileTypes TargetFileType { get; set; }

        [Option('o', "overwrite", Required = false, HelpText = "If the target config file exists, should it be overwritten?")]
        public bool Overwrite { get; set; } = false;
    }

}
