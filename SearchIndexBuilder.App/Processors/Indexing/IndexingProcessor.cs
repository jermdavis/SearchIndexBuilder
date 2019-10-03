using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchIndexBuilder.App.Processors.Indexing
{

    /// <summary>
    /// Handles the process of re-indexing items
    /// </summary>
    public class IndexingProcessor
    {
        public static void RunProcess(IndexingOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            Console.WriteLine("Running indexing");
            Console.WriteLine("----------------");

            OperationConfig cfg;

            if (!System.IO.File.Exists(options.ConfigFile))
            {
                Console.WriteLine($"Error: Config file '{options.ConfigFile}' not found");
                return;
            }

            using (var file = System.IO.File.OpenText(options.ConfigFile))
            {
                var data = file.ReadToEnd();
                cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationConfig>(data);
            }

            ISitecoreEndpoint endPoint = endpointFactory.Create(cfg.Url);

            var items = new Queue<ItemEntry>(cfg.Items/*.Skip(10000).Take(17)*/);
            var state = new ProcessState(endPoint, items, options, cfg);

            var total = items.Count();
            var startTime = DateTime.Now;

            processAllGroups(state, total);

            var endTime = DateTime.Now;

            Console.WriteLine(">>Finished");
            Console.WriteLine($">> at {endTime} - after {endTime-startTime}");

            Console.WriteLine($">> with {state.Errors.Count()} errors");
            foreach (var err in state.Errors)
            {
                Console.WriteLine(err);
            }
        }

        private static void processAllGroups(ProcessState state, int total)
        {
            bool cancelWasTriggered = false;
            int totalProcessed = 0;

            while (state.Items.Count > 0 && !cancelWasTriggered)
            {
                var processed = processGroup(state, out cancelWasTriggered);

                totalProcessed += processed;
                var percentage = (float)totalProcessed / (float)total * 100;

                Console.WriteLine();
                Console.WriteLine($">>{processed} done. {totalProcessed}/{total}. ({percentage:0.00}%) Time: {state.Average.CurrentAverage().TotalMilliseconds}ms");

                var ticksPerItem = processed > 0 ? state.Average.CurrentAverage().Ticks / processed : state.Average.CurrentAverage().Ticks;
                var estimatedRemaining = new TimeSpan(ticksPerItem * (total - totalProcessed));

                Console.WriteLine($">>Estimated remaining: {estimatedRemaining} - end at {DateTime.Now + estimatedRemaining}");
                Console.WriteLine();

                if(cancelWasTriggered)
                {
                    backupState(state);
                }
            }

            if(!cancelWasTriggered)
            {
                backupErrors(DateTime.Now, state);
            }
        }

        private static void backupState(ProcessState state)
        {
            Console.WriteLine("Cancel was triggered - saving state and exiting");

            var now = DateTime.Now;

            var baseFilename = extractBaseFilename(state.Options.ConfigFile, "backup-");

            var configFile = $"backup-{now.ToString("yyyyMMdd-hhmm")}-{baseFilename}";
            using (var file = System.IO.File.CreateText(configFile))
            {
                state.Config.Items = state.Items.ToArray();
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(state.Config, Newtonsoft.Json.Formatting.Indented);
                file.WriteLine(data);
            }
            Console.WriteLine($">>Saved updated config state for restart to {configFile}");

            backupErrors(now, state);
        }

        private static string extractBaseFilename(string name, string prefix)
        {
            var result = name;
            
            while(result.Contains(prefix))
            {
                result = result.Substring(14 + prefix.Length);
            }

            return result;
        }

        private static void backupErrors(DateTime now, ProcessState state)
        {
            var baseFilename = extractBaseFilename(state.Options.ConfigFile, "backup-");
            var errorFile = $"errors-{now.ToString("yyyyMMdd-hhmm")}-{baseFilename}";
            using (var file = System.IO.File.CreateText(errorFile))
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(state.Errors, Newtonsoft.Json.Formatting.Indented);
                file.WriteLine(data);
            }
            Console.WriteLine($">>Saved error state to {errorFile}");
        }

        private static int processGroup(ProcessState state, out bool cancelWasTriggered)
        {
            cancelWasTriggered = false;
            bool cancelTriggered = false;

            Console.CancelKeyPress += (sender, args) => {
                // lock?
                if (cancelTriggered == false)
                {
                    Console.WriteLine(Environment.NewLine + ">>Cancel triggered! Finishing current operation and tidying up...");
                    cancelTriggered = true;
                }
                args.Cancel = true;
            };

            int processed = 0;
            int retries = 0;
            do
            {
                state.sw.Restart();

                var itm = state.Items.Peek();

                bool result = processItem(state, itm);

                state.sw.Stop();
                state.Average.Record(state.sw.Elapsed, state.Options.Pause);

                handleResult(result, state, itm, ref retries, ref processed);

                if(cancelTriggered)
                {
                    cancelWasTriggered = true;
                    break;
                }

                if (result == true && cancelTriggered == false && state.Options.Pause > 0)
                {
                    Console.WriteLine($">>Pausing for {state.Options.Pause}ms");
                    System.Threading.Thread.Sleep(state.Options.Pause);
                }
            } while (processed < state.Options.OutputEvery && state.Items.Count > 0);

            return processed;
        }

        private static bool processItem(ProcessState state, ItemEntry itm)
        {
            bool result;

            try
            {
                Console.Write($"{itm.Id} {itm.Name}...");

                result = state.Endpoint.IndexItem(state.Config.Token, itm.Id, state.Config.Database, state.Config.Indexes);

                if (result == false)
                {
                    state.Errors.Add($">>WARN: {itm.Id} [{itm.Name}] Indexing error");
                }
            }
            catch (Exception ex)
            {
                result = false;
                state.Errors.Add($">>WARN: {itm.Id} [{itm.Name}] {formatException(ex)}");
            }

            return result;
        }

        private static void handleResult(bool result, ProcessState state, ItemEntry itm, ref int retries, ref int processed)
        {
            if (result == false)
            {
                if (retries == state.Options.Retries)
                {
                    retries = 0;
                    processed += 1;
                    state.Items.Dequeue();

                    state.Errors.Add($">>ERR:  {itm.Id} - Too many retries - aborting");
                    Console.WriteLine($"\r{itm.Id} {itm.Name} -- {state.sw.Elapsed.TotalMilliseconds}ms -- Too many retries");
                }
                else
                {
                    retries += 1;
                    Console.WriteLine($"\r{itm.Id} {itm.Name} -- {state.sw.Elapsed.TotalMilliseconds}ms -- Warning #{retries}");
                    randomBackOff(retries);
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
                state.Items.Dequeue();
                processed += 1;
                retries = 0;
            }
        }

        private static void randomBackOff(int errorCount)
        {
            // add random component to back off?
            int msToWait = 1000 + (1000 * (errorCount * errorCount));
            Console.WriteLine($">>Backing off for {msToWait}ms");
            System.Threading.Thread.Sleep(msToWait);
        }

        private static string formatException(Exception ex)
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
    }

}
