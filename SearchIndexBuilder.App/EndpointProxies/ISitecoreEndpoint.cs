using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.App.EndpointProxies
{

    public interface ISitecoreEndpoint
    {
        IEnumerable<string> FetchDatabases();
        IEnumerable<ItemEntry> FetchItemIds(string database, string query);
        IEnumerable<string> FetchIndexes();
        bool IndexItem(Guid id, string databaseName, IEnumerable<string> indexes);
    }

}