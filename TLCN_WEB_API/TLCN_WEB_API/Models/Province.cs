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
    public class Province
    {
        public string ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        string columnName = "Provinces";
        private static string key = "TLCN";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public Province(IConfiguration config)
        {
            _config = config;
        }

        public Province()
        {
        }

        IFirebaseClient client;


        public List<Province> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Province>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Province>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<Province> getByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Province>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Province>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Province>();
            foreach (var item in list)
            {
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            return list2;
        }


        // thêm dư liệu lên firebase
        public void AddToFireBase(Province province)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = province;
            PushResponse response = client.Push("Provinces/", data);
            data.ProvinceID = data.ProvinceID;
            SetResponse setResponse = client.Set("Provinces/" + data.ProvinceID, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Province province)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = province;
            data.ProvinceID = id;
            SetResponse setResponse = client.Set("Provinces/" + data.ProvinceID, data);
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Province();
            //  data.ProvinceID = id;
            SetResponse setResponse = client.Set("Provinces/" + id, data);
        }

    }
}
