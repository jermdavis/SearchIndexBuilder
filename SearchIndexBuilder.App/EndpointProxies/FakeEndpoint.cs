using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.App.EndpointProxies
{

    public class FakeEndpointFactory : ISitecoreEndpointFactory
    {
        public ISitecoreEndpoint Create(string url)
        {
            return new FakeEndpoint();
        }
    }

    /// <summary>
    /// For test purposes, this endpoint proxy can be used when you don't want to target a Sitecore instance
    /// </summary>
    /// <remarks>
    /// Just returns data for most operations - but will randomly add errors during indexing calls.
    /// </remarks>
    public class FakeEndpoint : ISitecoreEndpoint
    {
        private Random _rnd = new Random();

        public IEnumerable<string> FetchDatabases(string token)
        {
            return new string[] { "master", "web" };
        }

        public IEnumerable<string> FetchIndexes(string token)
        {
            return new string[] { "sitecore_master_index", "sitecore_web_index" };
        }

        public IEnumerable<ItemEntry> FetchItemIds(string token, string database, string query)
        {
            return new ItemEntry[] {
                new ItemEntry() { Name="one", Id=Guid.NewGuid() },
                new ItemEntry() { Name="two", Id=Guid.NewGuid() },
                new ItemEntry() { Name="three", Id=Guid.NewGuid() },
                new ItemEntry() { Name="four", Id=Guid.NewGuid() },
                new ItemEntry() { Name="five", Id=Guid.NewGuid() },
                new ItemEntry() { Name="six", Id=Guid.NewGuid() }
            };
        }

        public IndexResult IndexItem(string token, Guid id, string databaseName, IEnumerable<string> indexes)
        {
            System.Threading.Thread.Sleep(50 + _rnd.Next(5000));

            int rnd = _rnd.Next(10);

            if(rnd >= 4)
            {
                return new IndexResult(new Activity[] { new Activity() { Index = "sitecore_test_index", Uri = "http://uri/test?2", Error = "Stuff broke", Messages = new string[] { "Thing one", "Thing two" } } });
            }

            if(rnd == 3)
            {
                throw new ArgumentException("Error", new Exception("Something broke!"));
            }

            return new IndexResult(new Activity[] { new Activity() { Index = "sitecore_test_index", Uri = "http://uri/test?2", Messages = new string[] { "Thing one", "Thing two" } } });
        }
    }

}