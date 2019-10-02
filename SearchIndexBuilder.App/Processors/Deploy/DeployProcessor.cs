using System;
using System.IO;
using System.Reflection;

namespace SearchIndexBuilder.App.Processors.Deploy
{

    /// <summary>
    /// Handles the "deploy the endpoint" operation
    /// </summary>
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

        private static void updateResourceFileWithToken(string fileName, string token)
        {
            string data = File.ReadAllText(fileName);

            data = data.Replace("%%SECURITY_TOKEN%%", token);

            File.WriteAllText(fileName, data);
        }

        public static void RunDeploy(DeployOptions options)
        {
            Console.WriteLine("Deploying endpoint");
            Console.WriteLine("------------------");

            var file = Constants.EndpointFile;
            var fileName = Path.Combine(options.Website, file);

            if(File.Exists(fileName) && options.Overwrite == false)
            {
                Console.WriteLine($"The endpoint file already exists at {options.Website} and overwrite not allowed.");
                Console.WriteLine("It will not be delployed.");
                return;
            }

            try
            {
                writeResourceToFile($"SearchIndexBuilder.App.Endpoint.{file}", fileName);
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Unable to write the endpoint file to disk.");
                Console.WriteLine("This command requires write permissions to the folder specified with the -w parameter.");
                return;
            }

            if (string.IsNullOrWhiteSpace(options.Token))
            {
                options.Token = Guid.NewGuid().ToString();
            }

            try
            {
                updateResourceFileWithToken(fileName, options.Token);
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Unable to update endpoint file with security token.");
                Console.WriteLine("This command requires write permissions to the folder specified with the -w parameter.");
                return;
            }

            Console.WriteLine($"Deploying {file} to {options.Website}");
            Console.WriteLine($"Security token is: {options.Token}");
        }
    }

}