<%@ Page Language="C#" AutoEventWireup="true" %>
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
            .Select(i => new { Name = i.Name, Id = i.ID.ToString() });

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
            .Select(k => new { Name = k.Value, Id = k.Key });

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

        public void Clear()
        {
            Uri = string.Empty;
            Index = string.Empty;
            Messages = null;
            Error = null;
        }
    }

    private class Logger
    {
        private StringBuilder _result = new StringBuilder();

        public void FlushActivity(Activity a)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.Append("uri:\"" + a.Uri + "\"");
            sb.Append(",idx:\"" + a.Index + "\"");

            if(a.Messages != null)
            {
                sb.Append(",msg:[");
                bool first = true;
                foreach(var msg in a.Messages)
                {
                    if(!first)
                    {
                        sb.Append(",");
                    }
                    first = false;
                    sb.Append("\"" + msg + "\"");
                }
                sb.Append("]");
            }

            if(!string.IsNullOrWhiteSpace(a.Error))
            {
                sb.Append(",err:\"" + a.Error + "\"");
            }

            sb.Append("}");

            if(_result.Length > 0)
            {
                _result.Append(",");
            }
            _result.Append(sb.ToString());
        }

        public string Flush()
        {
            var result = "[" + _result.ToString() + "]";
            _result.Clear();

            return result;
        }
    }

    private void processResponse()
    {
        var itemId = Request["id"];
        var dbName = Request["db"];
        var indexName = Request["idx"];

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