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
    public class RemoveProcessor
    {
        public static void RunRemove(RemoveOptions options)
        {
            Console.WriteLine("Removing endpoint");
            Console.WriteLine("-----------------");

            var file = Constants.EndpointFile;
            var fileName = Path.Combine(options.Website, file);

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
                Console.WriteLine($"Endpoint removed from {options.Website}");
            }
            else
            {
                Console.WriteLine($"Endpoint not found at {options.Website}");
            }
        }
    }

}
