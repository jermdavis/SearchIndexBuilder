using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchIndexBuilder
{
    
    /// <summary>
    /// Implement the "wait for keypress or wait for debugger attachment" logic
    /// Feels like there should be a better way to do this...
    /// </summary>
    public class DebuggerWaiter
    {
        private bool _done = false;
        private AutoResetEvent _flag = new AutoResetEvent(false);

        private void WaitForKey()
        {
            Console.ReadKey();
            _done = true;
            _flag.Set();
        }

        private void WaitForDebugger()
        {
            while (!_done)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    _done = true;
                    _flag.Set();
                    return;
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        public void Wait()
        {
            var keyThread = new Thread(new ThreadStart(WaitForKey));
            var debugThread = new Thread(new ThreadStart(WaitForDebugger));

            keyThread.Start();
            debugThread.Start();

            while (!_done)
            {
                _flag.WaitOne(2000);
            }

            keyThread.Abort();
            debugThread.Abort();
        }
    }

}