using SearchIndexBuilder.EndpointProxies;
using System;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Shared logic for all the procesors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseProcessor<T> : IVerbProcessor where T : CoreOptions
    {
        protected T _options;
        protected ISitecoreEndpointFactory _endpointFactory;
        protected IConfigFileManager _configFileManager;

        public BaseProcessor(T options)
        {
            _options = options;
        }

        public void OverrideFileType(string filename)
        {
            if(filename.EndsWith(GZipStreamConfigFileManager.FileExtension))
            {
                _configFileManager = new GZipStreamConfigFileManager();
            }

            if (filename.EndsWith(ZipArchiveConfigFileManager.FileExtension))
            {
                _configFileManager = new ZipArchiveConfigFileManager();
            }
        }

        public virtual void Run()
        {
            if (_options.Attach)
            {
                Console.WriteLine("Pausing for debugger - press any key to continue...");
                var dw = new DebuggerWaiter();
                dw.Wait();
            }

            _configFileManager = _options.ConfigFileType.Create(_configFileManager);

            Console.WriteLine($"[Reading {_configFileManager.Extension} file format]");

            if(_options.Fake)
            {
                Console.WriteLine("[Using fake endpoint factory]");
                _endpointFactory = new FakeEndpointFactory();
            }
            else
            {
                _endpointFactory = new SitecoreWebEndpointFactory();
            }
        }
    }

}