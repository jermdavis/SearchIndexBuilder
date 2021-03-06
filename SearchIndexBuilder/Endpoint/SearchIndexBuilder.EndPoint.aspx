﻿<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.ContentSearch" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string token = Request["t"];

        if(token != @"%%SECURITY_TOKEN%%")
        {
            noCommandResponse();
            return;
        }

        string cmd = Request["cmd"];

        if(string.IsNullOrWhiteSpace(cmd))
        {
            noCommandResponse();
            return;
        }

        cmd = cmd.Trim().ToLower();

        switch(cmd)
        {
            case "indexes":
                indexesResponse();
                break;
            case "items":
                itemsResponse();
                break;
            case "dbs":
                dbsResponse();
                break;
            case "process":
                processResponse();
                break;
        }
    }

    private void noCommandResponse()
    {
        Response.Write(DateTime.Now.ToString());
    }

    private void indexesResponse()
    {
        var data = ContentSearchManager.Indexes
            .Select(i => i.Name);

        var response = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        Response.ContentType = "application/json";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Write(response);
    }

    private void dbsResponse()
    {
        var data = Sitecore.Configuration.Factory.GetDatabaseNames();

        var response = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        Response.ContentType = "application/json";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Write(response);
    }

    private void itemsResponse()
    {
        var dbName = Request["db"];

        if (string.IsNullOrWhiteSpace(dbName))
        {
            noCommandResponse();
            return;
        }

        var query = Request["q"];
        if (string.IsNullOrEmpty(query))
        {
            fetchFromDb(dbName);
        }
        else
        {
            query = HttpUtility.UrlDecode(query);
            fetchFromQuery(dbName, query);
        }
    }

    private void fetchFromQuery(string dbName, string query)
    {
        var db = Sitecore.Configuration.Factory.GetDatabase(dbName);
        var items = db.SelectItems(query);

        var data = items
            .Select(i => new { n = i.Name, i = i.ID.ToString() });

        var response = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        Response.ContentType = "application/json";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Write(response);
    }

    private void fetchFromDb(string dbName)
    {
        var conStr = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

        var items = new Dictionary<Guid, string>();

        using (var conn = new SqlConnection(conStr))
        {
            conn.Open();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT [Name], [ID] FROM [Items]";

                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var name = result.GetString(0);
                        var id = result.GetGuid(1);

                        items.Add(id, name);
                    }
                }
            }
        }

        var data = items
            .Select(k => new { n = k.Value, i = k.Key });

        var response = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        Response.ContentType = "application/json";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Write(response);
    }

    private class Activity
    {
        public string Uri { get; internal set; }
        public string Index { get; internal set; }
        public string[] Messages { get; internal set; }
        public string Error { get; internal set; }
        public long Total { get; internal set; }

        public void Clear()
        {
            Uri = string.Empty;
            Index = string.Empty;
            Messages = null;
            Error = null;
            Total = 0;
        }
    }

    private class Logger
    {
        private List<object> _items = new List<object>();

        public void FlushActivity(Activity a)
        {
            _items.Add(new { uri = a.Uri, idx = a.Index, msg = a.Messages, err = a.Error, total = a.Total });
        }

        public string Flush()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_items);
        }
    }

    private void processResponse()
    {
        var itemId = Request["id"];
        var dbName = Request["db"];
        var indexName = Request["idx"];

        int timeout = 60;
        int.TryParse(Request["to"], out timeout);
        Page.Server.ScriptTimeout = timeout;

        if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(dbName))
        {
            noCommandResponse();
            return;
        }

        var indexes = indexName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        var log = new Logger();
        var activity = new Activity();

        try
        {
            var db = Sitecore.Configuration.Factory.GetDatabase(dbName);
            var item = db.GetItem(new Sitecore.Data.ID(itemId));

            activity.Clear();

            if (item == null)
            {
                activity.Error = "Item " + itemId + " not found";
                log.FlushActivity(activity);
            }
            else
            {

                foreach (var lng in item.Languages)
                {
                    var itm = db.GetItem(item.ID, lng);

                    if (itm.Versions.Count == 0 || itm.Language != lng)
                    {
                        continue;
                    }

                    foreach (var ver in itm.Versions.GetVersionNumbers())
                    {
                        var itm2 = db.GetItem(item.ID, lng, ver);
                        var indexableId = new SitecoreItemUniqueId(itm2.Uri);

                        activity.Uri = indexableId.Value.ToString();

                        foreach (var idx in indexes)
                        {
                            activity.Index = idx;

                            var index = Sitecore.ContentSearch.ContentSearchManager.GetIndex(idx);

                            var job = Sitecore.ContentSearch.Maintenance.IndexCustodian.UpdateItem(index, indexableId);
                            var result = job.Wait();

                            string[] strArray = new string[job.Status.Messages.Count];
                            job.Status.Messages.CopyTo(strArray, 0);
                            activity.Messages = strArray;
                            activity.Total = job.Status.Total;

                            log.FlushActivity(activity);

                            activity.Messages = null;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            activity.Error = ex.Message;
            log.FlushActivity(activity);
            activity.Error = null;
        }

        Response.ContentType = "application/json";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Write(log.Flush());
    }
</script>