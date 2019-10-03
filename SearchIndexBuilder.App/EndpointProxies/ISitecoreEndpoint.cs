using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.App.EndpointProxies
{

    public interface ISitecoreEndpoint
    {
        IEnumerable<string> FetchDatabases(string token);
        IEnumerable<ItemEntry> FetchItemIds(string token, string database, string query);
        IEnumerable<string> FetchIndexes(string token);
        IndexResult IndexItem(string token, Guid id, string databaseName, IEnumerable<string> indexes, int timeout);
    }

}