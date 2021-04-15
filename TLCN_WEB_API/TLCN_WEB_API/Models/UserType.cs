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
    public class UserType
    {
        public string UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        string columnName = "UserType";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        private IConfiguration _config;
        public UserType(IConfiguration config)
        {
            _config = config;
        }

        public UserType()
        {
        }

        private static string key = "TLCN";
        IFirebaseClient client;
        public List<UserType> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            //danh sách tìm kiếm
            //var list2 = new List<User>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<UserType> getByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<UserType>();
            foreach (var item in list)
            {
                if (item.UserTypeID == id)
                    list2.Add(item);
            }
            return list2;
        }
        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, UserType usertype)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = usertype;
            //PushResponse response = client.Push("UserType/", data);
            data.UserTypeID = id;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new UserType();
            //PushResponse response = client.Push("UserType/", data);
            //data.UserTypeID = id;
            SetResponse setResponse = client.Set("UserType/" + id, data);
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(UserType userType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = userType;
            PushResponse response = client.Push("UserType/", data);
            data.UserTypeID = response.Result.name;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }
    }
}
