namespace SearchIndexBuilder.App.EndpointProxies
{

    /// <summary>
    /// Model class used to deserialise the data returned from index operations on the remote endpoint
    /// </summary>
    public class Activity
    {
        public string Uri { get; set; }
        public string Index { get; set; }
        public string[] Messages { get; set; }
        public string Error { get; set; }
        public long Total { get; set; }
    }

}