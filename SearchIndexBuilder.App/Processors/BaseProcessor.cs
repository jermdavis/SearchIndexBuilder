using System;

namespace SearchIndexBuilder.App.Processors
{


    public abstract class BaseProcessor<T> : IVerbProcessor where T : CoreOptions
    {
        protected T _options;

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
        }
    }

}
