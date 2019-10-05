namespace SearchIndexBuilder.EndpointProxies
{

    public interface ISitecoreEndpointFactory
    {
        ISitecoreEndpoint Create(string url);
    }

}