using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class BusinessType
    {
        public string BusinessTypeID { get; set; }
        public string BusinessTypeName { get; set; }

        string columnName = "BusinessType";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };


        public List<BusinessType> getAll(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<BusinessType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<BusinessType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<BusinessType> getByID(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<BusinessType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<BusinessType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<BusinessType>();
            foreach (var item in list){
                if (item.BusinessTypeID == id)
                    list2.Add(item);
            }
            return list2;
        }


        private void AddbyidToFireBase(string id, BusinessType businessType){
            client = new FireSharp.FirebaseClient(config);
            var data = businessType;
            PushResponse response = client.Push("BusinessType/", data);
            data.BusinessTypeID = id;
            SetResponse setResponse = client.Set("BusinessType/" + data.BusinessTypeID, data);
        }
    }
}
