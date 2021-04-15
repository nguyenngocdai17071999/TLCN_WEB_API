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
    public class Comment
    {
        public string CommentID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
        public string UserID { get; set; }
        public string StoreID { get; set; }
        public string ParentComment_ID { get; set; }

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        string columnName = "Comment";
        private static string key = "TLCN";
        private IConfiguration _config;
        public Comment(IConfiguration config)
        {
            _config = config;
        }

        public Comment()
        {
            CommentID = "";
            Content = "";
            Date = "";
            Image = "";
            UserID = "";
            StoreID = "";
            ParentComment_ID = "";
        }

        IFirebaseClient client;
        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Comment comment)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
        }

        public List<Comment> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Comment>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<Comment> getbyId(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Comment>();
            //danh sách tìm kiếm
            //if (data == null) return Ok("Error");
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Comment>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Comment();
            // data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + id, data);
        }

        public string GetIDUser(string Email)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            foreach (var item in list)
            {
                if (item.Email.ToString() == Email)
                    return item.UserID;
            }
            return "";
        }

        public string idchuquan(string idstore)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Stores");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID.ToString() == idstore)
                    return item.UserID;
            }
            return "";
        }

        // thêm dư liệu lên firebase
        public string AddToFireBase(Comment comment)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            PushResponse response = client.Push("Comment/", data);
            data.CommentID = response.Result.name;
            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
            return data.CommentID;
        }

    }
}
