namespace SearchIndexBuilder.App.Processors.Retry
{
    public class RetryProcessor : BaseProcessor<RetryOptions>
    {
        public static void RunProcess(RetryOptions option)
        {

        }

        public RetryProcessor(RetryOptions options) :base(options)
        {
        }
    }

}
