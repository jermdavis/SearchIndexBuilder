using CommandLine;

namespace SearchIndexBuilder.App.Processors
{
    public class CoreOptions
    {
        [Option('a', "attach", Required = false, HelpText = "Causes the tool to pause before processing - to allow the debugger to attach.")]
        public bool Attach { get; set; } = false;
    }

}