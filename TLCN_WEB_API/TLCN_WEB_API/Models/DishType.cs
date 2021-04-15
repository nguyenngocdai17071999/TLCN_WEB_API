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
    public class DishType
    {
        public string DishType_ID { get; set; }
        public string DishyTypeName { get; set; }
        string columnName = "DishType";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public DishType(IConfiguration config)
        {
            _config = config;
        }

        public DishType()
        {
            DishType_ID = "";
            DishyTypeName = "";
        }

        public List<DishType> getById(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DishType>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DishType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<DishType>();
            foreach (var item in list)
            {
                if (item.DishType_ID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public List<DishType> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DishType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DishType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        private static string key = "TLCN";
        IFirebaseClient client;

        // thêm dư liệu lên firebase
        public void AddToFireBase(DishType dishType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = dishType;
            PushResponse response = client.Push("DishType/", data);
            data.DishType_ID = response.Result.name;
            SetResponse setResponse = client.Set("DishType/" + data.DishType_ID, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, DishType dishType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = dishType;
            data.DishType_ID = id;
            SetResponse setResponse = client.Set("DishType/" + data.DishType_ID, data);
        }
        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new DishType();
            // data.DishType_ID = id;
            SetResponse setResponse = client.Set("DishType/" + id, data);
        }
    }
}
