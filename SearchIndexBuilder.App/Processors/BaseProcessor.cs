using SearchIndexBuilder.App.EndpointProxies;
using System;

namespace SearchIndexBuilder.App.Processors
{


    public abstract class BaseProcessor<T> : IVerbProcessor where T : CoreOptions
    {
        protected T _options;
        protected ISitecoreEndpointFactory _endpointFactory;

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
