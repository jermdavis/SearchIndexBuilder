using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.EndpointProxies
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
        private int _itemsToCreate = 15;

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
            var items = new List<ItemEntry>();

            for(int i=0; i<_itemsToCreate; i++)
            {
                items.Add(new ItemEntry(){
                    Name = $"Item {i.ToString()}",
                    Id = Guid.NewGuid()
                });
            }

            return items;
        }

        public IndexResult IndexItem(string token, ItemEntry itm, string databaseName, IEnumerable<string> indexes, int timeout)
        {
            System.Threading.Thread.Sleep(50 + _rnd.Next(2500));

            int rnd = _rnd.Next(10);

            if(itm.Name == "Item 2")
            {
                return new IndexResult(new Activity[] { new Activity() { Index = "sitecore_test_index", Uri = "http://uri/test?2", Error = "Item two always fails internally", Messages = new string[] { "Thing one" } } });
            }

            if(rnd == 2)
            {
                return new IndexResult(new Activity[] { new Activity() { Index = "sitecore_test_index", Uri = "http://uri/test?2", Error = "Error from indexing", Messages = new string[] { "Thing one", "Thing two" } } });
            }

            if(rnd == 3)
            {
                throw new ArgumentException("Error", new Exception("Exception thrown!"));
            }

            return new IndexResult(new Activity[] { new Activity() { Index = "sitecore_test_index", Uri = "http://uri/test?2", Messages = new string[] { "Thing one", "Thing two" } } });
        }
    }

}