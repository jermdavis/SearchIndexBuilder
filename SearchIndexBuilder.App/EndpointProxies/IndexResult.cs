using System.Linq;

namespace SearchIndexBuilder.App.EndpointProxies
{
    public class IndexResult
    {
        public bool Error { get; }
        public Activity[] Activities { get; }

        public IndexResult(Activity[] results)
        {
            Activities = results;
            Error = results.Where(a => !string.IsNullOrWhiteSpace(a.Error)).Any();
        }
    }

}