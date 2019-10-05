using System.Linq;

namespace SearchIndexBuilder.EndpointProxies
{

    /// <summary>
    /// Model type for representing the result of an indexing operation
    /// </summary>
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