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
    public class Menu
    {
        public string MenuID { get; set; }
        public string MenuName { get; set; }
        string columnName = "Menu";
        private static string key = "TLCN";

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public Menu(IConfiguration config)
        {
            _config = config;
        }
        public Menu()
        {
        }

        IFirebaseClient client;

        public List<Menu> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<Menu> getByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }

            var list2 = new List<Menu>();
            foreach (var item in list)
            {
                if (item.MenuID == id)
                    list2.Add(item);
            }
            foreach (var item in list)
            {
                AddToFireBase(item);
            }
            return list2;
        }


        // thêm dư liệu lên firebase
        public void AddToFireBase(Menu menu)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = menu;
            PushResponse response = client.Push("Menu/", data);
            data.MenuID = response.Result.name;
            SetResponse setResponse = client.Set("Menu/" + data.MenuID, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Menu menu)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = menu;
            data.MenuID = id;
            SetResponse setResponse = client.Set("Menu/" + data.MenuID, data);
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Menu();
            // data.MenuID = id;
            SetResponse setResponse = client.Set("Menu/" + id, data);
        }
    }
}
