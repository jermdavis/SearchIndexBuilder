using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.EndpointProxies
{

    public interface ISitecoreEndpoint
    {
        IEnumerable<string> FetchDatabases(string token);
        IEnumerable<ItemEntry> FetchItemIds(string token, string database, string query);
        IEnumerable<string> FetchIndexes(string token);
        IndexResult IndexItem(string token, ItemEntry itm, string databaseName, IEnumerable<string> indexes, int timeout);
    }

}