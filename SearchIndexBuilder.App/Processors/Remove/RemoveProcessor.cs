using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.App.Processors.Remove
{

    public class RemoveProcessor
    {
        public static void RunRemove(RemoveOptions options)
        {
            var file = Constants.EndpointFile;
            var fileName = Path.Combine(options.Website, file);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                Console.WriteLine($"Endpoint removed from {options.Website}");
            }
            else
            {
                Console.WriteLine($"Endpoint not found at {options.Website}");
            }
        }
    }

}
