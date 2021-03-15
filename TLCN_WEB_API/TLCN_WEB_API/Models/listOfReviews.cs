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
    public class listOfReviews
    {
        public string RatePoint { get; set; }
        public string ReViewID { get; set; }
        public string StoreID { get; set; }
        public string UserID { get; set; } 
        public string CommentID { get; set; }
        string columnName = "listOfReviews";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public listOfReviews(IConfiguration config)
        {
            _config = config;
        }

        public listOfReviews()
        {
        }

        private static string key = "TLCN";
        IFirebaseClient client;

        public List<listOfReviews> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<listOfReviews>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<listOfReviews>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<listOfReviews> getByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<listOfReviews>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<listOfReviews>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<listOfReviews>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public List<listOfReviews> getByIDComment(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<listOfReviews>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<listOfReviews>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<listOfReviews>();
            foreach (var item in list)
            {
                if (item.CommentID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public void AddToFireBase(listOfReviews list)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = list;
            PushResponse response = client.Push("listOfReviews/", data);
            data.ReViewID = response.Result.name;
            SetResponse setResponse = client.Set("listOfReviews/" + data.ReViewID, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, listOfReviews list)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = list;
            PushResponse response = client.Push("listOfReviews/", data);
            data.ReViewID = id;
            SetResponse setResponse = client.Set("listOfReviews/" + data.ReViewID, data);
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new listOfReviews();
            //PushResponse response = client.Push("listOfReviews/", data);
            //data.ReViewID = id;
            SetResponse setResponse = client.Set("listOfReviews/" + id, data);
        }

    }
}
