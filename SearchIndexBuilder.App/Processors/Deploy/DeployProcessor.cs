using System;
using System.IO;
using System.Reflection;

namespace SearchIndexBuilder.App.Processors.Deploy
{

    /// <summary>
    /// Handles the "deploy the endpoint" operation
    /// </summary>
    public class DeployProcessor : BaseProcessor<DeployOptions>
    {
        public static void RunProcess(DeployOptions options)
        {
            var dp = new DeployProcessor(options);
            dp.Run();
        }

        public DeployProcessor(DeployOptions options) : base(options)
        {
        }

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

        public override void Run()
        {
            Console.WriteLine("Deploying endpoint");
            Console.WriteLine("------------------");

            var file = Constants.EndpointFile;
            var fileName = Path.Combine(_options.Website, file);

            if(File.Exists(fileName) && _options.Overwrite == false)
            {
                Console.WriteLine($"The endpoint file already exists at {_options.Website} and overwrite not allowed.");
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

            if (string.IsNullOrWhiteSpace(_options.Token))
            {
                _options.Token = Guid.NewGuid().ToString();
            }

            try
            {
                updateResourceFileWithToken(fileName, _options.Token);
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Unable to update endpoint file with security token.");
                Console.WriteLine("This command requires write permissions to the folder specified with the -w parameter.");
                return;
            }

            Console.WriteLine($"Deploying {file} to {_options.Website}");
            Console.WriteLine($"Security token is: {_options.Token}");
        }
    }

}