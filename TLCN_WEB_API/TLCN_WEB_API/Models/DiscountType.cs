using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class DiscountType
    {
        public string DiscountTypeID { get; set; }
        public string DiscountTypeName { get; set; }
        public string DiscountTypePicture { get; set; }
        string columnName = "DiscountType";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public DiscountType()
        {
            DiscountTypeID = "";
            DiscountTypeName = "";
            DiscountTypePicture = "";
        }

        public List<DiscountType> getAll()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<DiscountType> getByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<DiscountType>();
            foreach (var item in list)
            {
                if (item.DiscountTypeID == id)
                    list2.Add(item);
            }
            return list2;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(DiscountType discountType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = discountType;
            PushResponse response = client.Push("DiscountType/", data);
            data.DiscountTypeID = response.Result.name;
            SetResponse setResponse = client.Set("DiscountType/" + data.DiscountTypeID, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, DiscountType discountType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = discountType;
            data.DiscountTypeID = id;
            SetResponse setResponse = client.Set("DiscountType/" + data.DiscountTypeID, data);
        }
        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new DiscountType();
            SetResponse setResponse = client.Set("DiscountType/" + id,setnull(data));
        }
        public DiscountType setnull(DiscountType a)
        {
            a.DiscountTypeID = null;
            a.DiscountTypeName = null;
            a.DiscountTypePicture = null;
            return a;
        }
    }
}
