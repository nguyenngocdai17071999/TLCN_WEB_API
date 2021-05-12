using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class View_Store
    {
        public string ViewID { get; set; }
        public string StoreID { get; set; }
        public string Date { get; set; }

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        string columnName = "View_Store";
        private static string key = "TLCN";
        private IConfiguration _config;
        IFirebaseClient client;
        public View_Store(IConfiguration config)
        {
            _config = config;
        }

        public View_Store()
        {
            ViewID = "";
            StoreID = "";
            Date = "";
        }

        public void update(string id, View_Store view_Store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = view_Store;
            data.ViewID = id;
            SetResponse setResponse = client.Set("View_Store/" + data.ViewID, data);
        }

        // thêm dư liệu lên firebase
        public void Add(View_Store view_Store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = view_Store;
            PushResponse response = client.Push("View_Store/", data);
            data.ViewID = response.Result.name;
            SetResponse setResponse = client.Set("View_Store/" + data.ViewID, data);
        }


        public List<View_Store> getAll()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<View_Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<View_Store>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<View_Store> getByIDStore(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<View_Store>();
            //danh sách tìm kiếm
            var listViewStore = new List<View_Store>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<View_Store>(((JProperty)item).Value.ToString()));
                }

                foreach (var item in list)
                {
                    if (item.StoreID == id) listViewStore.Add(item);
                }
            }  
            return listViewStore;
        }

    }
}
