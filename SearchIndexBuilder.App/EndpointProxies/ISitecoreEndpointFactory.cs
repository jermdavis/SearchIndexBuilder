namespace SearchIndexBuilder.App.EndpointProxies
{

    public interface ISitecoreEndpointFactory
    {
        ISitecoreEndpoint Create(string url);
    }

}