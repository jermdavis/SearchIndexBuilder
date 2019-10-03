using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.App.Processors.Remove
{

    /// <summary>
    /// Handles removing the endpoint from Sitecore
    /// </summary>
    public class RemoveProcessor : BaseProcessor<RemoveOptions>
    {
        public static void RunProcess(RemoveOptions options)
        {
            var rp = new RemoveProcessor(options);
            rp.Run();
        }

        public RemoveProcessor(RemoveOptions options) : base(options)
        {
        }

        public override void Run()
        {
            base.Run();

            Console.WriteLine("Removing endpoint");
            Console.WriteLine("-----------------");

            var file = Constants.EndpointFile;
            var fileName = Path.Combine(_options.Website, file);

            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Error: Unable to remove the endpoint file from disk.");
                    Console.WriteLine("This command requires delete permissions in the folder specified with the -w parameter.");
                    return;
                }
                Console.WriteLine($"Endpoint removed from {_options.Website}");
            }
            else
            {
                Console.WriteLine($"Endpoint not found at {_options.Website}");
            }
        }
    }

}
