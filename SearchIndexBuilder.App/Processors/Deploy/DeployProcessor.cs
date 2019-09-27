using SearchIndexBuilder.App.CommandLineOptions;
using System;
using System.IO;
using System.Reflection;

namespace SearchIndexBuilder.App.Processors.Deploy
{

    public class DeployProcessor
    {
        private static void writeResourceToFile(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }

        public static void RunDeploy(DeployOptions options)
        {
            var file = "SearchIndexBuilder.EndPoint.aspx";
            var fileName = Path.Combine(options.Website, file);
            writeResourceToFile($"SearchIndexBuilder.App.Endpoint.{file}", fileName);

            Console.WriteLine($"Deploying {file} to {options.Website}");
        }
    }

}