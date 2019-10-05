using CommandLine;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Common parameters shared by all processors
    /// </summary>
    public class CoreOptions
    {
        [Option('a', "attach", Required = false, HelpText = "Causes the tool to pause before processing - to allow the debugger to attach.")]
        public bool Attach { get; set; } = false;

        [Option('f', "fake", Required = false, HelpText = "Uses the a fake endpoint proxy - allowing the code to run without Sitecore, or a deployed endpoint")]
        public bool Fake { get; set; } = false;
    }

}