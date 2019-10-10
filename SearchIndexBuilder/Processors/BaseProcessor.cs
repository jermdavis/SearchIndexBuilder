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

        public virtual void Run()
        {
            if (_options.Attach)
            {
                Console.WriteLine("Pausing for debugger - press any key to continue...");
                Console.ReadKey();
            }

            //_configFileManager = new GZipStreamConfigFileManager();
            //_configFileManager = new ZipArchiveConfigFileManager();
            _configFileManager = new TextConfigFileManager();

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
