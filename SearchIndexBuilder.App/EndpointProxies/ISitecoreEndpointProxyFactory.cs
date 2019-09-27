namespace SearchIndexBuilder.App.EndpointProxies
{

    public interface ISitecoreEndpointFactory
    {
        ISitecoreEndpoint Create(string url);
    }

    public class FakeEndpointFactory : ISitecoreEndpointFactory
    {
        public ISitecoreEndpoint Create(string url)
        {
            return new FakeEndpoint();
        }
    }

    public class SitecoreWebEndpointFactory : ISitecoreEndpointFactory
    {
        public ISitecoreEndpoint Create(string url)
        {
            return new SitecoreWebEndpoint(url);
        }
    }

}