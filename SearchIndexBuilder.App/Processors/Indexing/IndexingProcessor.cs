using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.App.Processors.Indexing
{

    public class IndexingProcessor : BaseProcessor<IndexingOptions>
    {
        public static void RunProcess(IndexingOptions options)
        {
            var ip = new IndexingProcessor(options);
            ip.Run();
        }

        private ConfigFileManager _configFileManager;
        private bool _cancelTriggered = false;

        public IndexingProcessor(IndexingOptions options) : base(options)
        {
            _configFileManager = new ConfigFileManager();
            _options = options;
        }

        private void ProcessAllGroups(ProcessState state)
        {
            Console.CancelKeyPress += CancelHandler;
            
            while (state.Config.Items.Count > 0 && !_cancelTriggered)
            {
                ProcessGroup(state);

                var percentage = (float)state.Config.Processed.Count / (float)state.Config.TotalItems * 100f;

                Console.WriteLine();
                Console.WriteLine($">> {state.Config.Processed.Count}/{state.Config.TotalItems}. ({percentage:0.00}%) Time: {state.Average.CurrentAverage().TotalMilliseconds}ms");

                var estimatedRemaining = new TimeSpan(state.Average.CurrentAverage().Ticks * state.Config.Items.Count);

                if (state.Config.Processed.Count != state.Config.TotalItems)
                {
                    Console.WriteLine($">> Estimated remaining: {estimatedRemaining} - ending {DateTime.Now + estimatedRemaining}");
                }
                Console.WriteLine();

                // backup config file after each group?
            }

            Console.CancelKeyPress -= CancelHandler;
        }

        private void ProcessGroup(ProcessState state)
        {
            int processed = 0;
            int retries = 0;
            do
            {
                state.sw.Restart();

                var itm = state.Config.Items.Peek();
                bool result = ProcessItem(state, itm);

                state.sw.Stop();
                state.Average.Record(state.sw.Elapsed, state.Options.Pause);

                HandleResult(result, state, itm, ref retries, ref processed);

                if (_cancelTriggered)
                {
                    break;
                }

                if (result == true && _cancelTriggered == false && state.Options.Pause > 0)
                {
                    Console.WriteLine($">> Pausing for {state.Options.Pause}ms");
                    System.Threading.Thread.Sleep(state.Options.Pause);
                }
            } while (processed < state.Options.OutputEvery && state.Config.Items.Count > 0);
        }

        private bool ProcessItem(ProcessState state, ItemEntry itm)
        {
            bool result;

            try
            {
                Console.Write($"{itm.Id} {itm.Name}...");

                var indexResult = state.Endpoint.IndexItem(state.Config.Token, itm, state.Config.Database, state.Config.Indexes, state.Options.Timeout);
                result = !indexResult.Error;

                if (indexResult.Error)
                {
                    var err = new ItemFailure() {
                        At = DateTime.Now,
                        Item = itm,
                        Errors = indexResult.Activities.Select(a =>a.Error).ToArray(),
                        FailureType = FailureType.Warning
                    };
                    state.Config.Errors.Enqueue(err);
                }
            }
            catch (Exception ex)
            {
                result = false;
                var err = new ItemFailure()
                {
                    At = DateTime.Now,
                    Item = itm,
                    Errors = new string[] { formatException(ex) },
                    FailureType = FailureType.Warning
                };
                state.Config.Errors.Enqueue(err);
            }

            return result;
        }

        private string formatException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append($"[{ex.GetType().Name}]");
                sb.Append(ex.Message);

                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        private void HandleResult(bool result, ProcessState state, ItemEntry itm, ref int retries, ref int processed)
        {
            if (result == false)
            {
                if (retries == state.Options.Retries)
                {
                    retries = 0;
                    processed += 1;
                    state.Config.Processed.Enqueue(state.Config.Items.Dequeue());

                    state.Config.Errors.Enqueue(new ItemFailure() { At=DateTime.Now, Item = itm, Errors = new string[] { "Too many retries - aborting" }, FailureType = FailureType.Error });
                    Console.WriteLine($"\r{itm.Id} {itm.Name} -- {state.sw.Elapsed.TotalMilliseconds}ms -- Too many retries");
                }
                else
                {
                    retries += 1;
                    Console.WriteLine($"\r{itm.Id} {itm.Name} -- {state.sw.Elapsed.TotalMilliseconds}ms -- Warning #{retries}");
                    RandomBackOff(retries);
                }
            }
            else
            {
                var msg = string.Empty;
                if (retries > 0)
                {
                    msg = " - Transient error corrected";
                }

                Console.WriteLine($"\r{itm.Id} {itm.Name} -- {state.sw.Elapsed.TotalMilliseconds}ms{msg}");
                state.Config.Processed.Enqueue(state.Config.Items.Dequeue());
                processed += 1;
                retries = 0;
            }
        }

        private static void RandomBackOff(int errorCount)
        {
            int msToWait = 1000 + (1000 * (errorCount * errorCount));
            Console.WriteLine($">> Backing off for {msToWait}ms");
            System.Threading.Thread.Sleep(msToWait);
        }

        private void SaveState(ProcessState state)
        {
            var newFile = _configFileManager.Backup(state.Options.ConfigFile);
            Console.WriteLine($">> Moved existing config {state.Options.ConfigFile} to {newFile}");

            _configFileManager.Save(state.Options.ConfigFile, state.Config);
            Console.WriteLine($">> Config saved to {state.Options.ConfigFile}");
        }

        private void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (_cancelTriggered == false)
            {
                Console.WriteLine(Environment.NewLine + ">>Cancel triggered! Finishing current operation and tidying up...");
                _cancelTriggered = true;
            }
            args.Cancel = true;
        }

        public override void Run()
        {
            base.Run();

            Console.WriteLine("Running indexing");
            Console.WriteLine("----------------");

            var config = _configFileManager.Load(_options.ConfigFile);
            if (config == null)
            {
                Console.WriteLine($">> Error: Config file '{_options.ConfigFile}' not found");
                return;
            }

            if (config.Processed.Count == config.TotalItems)
            {
                Console.WriteLine($">> No items to process.");
                return;
            }

            ISitecoreEndpoint endPoint = _endpointFactory.Create(config.Url);
            var state = new ProcessState(endPoint, _options, config);
            state.Config.Attempts += 1;
            var startedAt = DateTime.Now;

            ProcessAllGroups(state);

            var endedAt = DateTime.Now;
            var elapsed = endedAt - startedAt;
            state.Config.Elapsed += elapsed;

            SaveState(state);

            Console.WriteLine();
            Console.WriteLine($">> Finished");
            Console.WriteLine($">>   at {endedAt} - after {elapsed}");
            Console.WriteLine($">>   with {state.Config.Errors.Count()} errors");
            Console.WriteLine($">>   total time now {state.Config.Elapsed} after {state.Config.Attempts} attempts.");
        }
    }

}
