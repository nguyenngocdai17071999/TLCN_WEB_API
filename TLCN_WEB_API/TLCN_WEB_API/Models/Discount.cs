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
    public class Discount
    {
        public string IDDiscount { get; set; }
        public string IDStore { get; set; }
        public string IDDiscountType { get; set; }

        string columnName = "Discount";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public List<Discount> getAll()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<Store> getByidDiscountType(string idDiscountType)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Discount>();
            foreach (var item in list)
            {
                if (item.IDDiscountType == idDiscountType)
                    list2.Add(item);
            }
            Store a = new Store();
            var danhsachstore = a.getAll(0,0);
            var danhsachcantim = new List<Store>();
            foreach(var item in list2)
            {
                foreach(var item2 in danhsachstore)
                {
                    if (item2.StoreID == item.IDStore)
                        danhsachcantim.Add(item2);
                }
            }
            return danhsachcantim;
        }

        public string getiddiscount(string idstore, string iddiscounttype)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            
            foreach(var item in list)
            {
                if (item.IDStore == idstore && item.IDDiscountType == iddiscounttype)
                {
                    return item.IDDiscount;
                }
            }
            return "";
        }

        public List<Discount> getByidStore(string IdStore)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Discount>();
            foreach (var item in list)
            {
                if (item.IDStore == IdStore)
                    list2.Add(item);
            }
            return list2;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(Discount discount)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = discount;
            PushResponse response = client.Push("Discount/", data);
            data.IDDiscount = response.Result.name;
            SetResponse setResponse = client.Set("Discount/" + data.IDDiscount, data);
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Discount discount)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = discount;
            data.IDDiscount = id;
            SetResponse setResponse = client.Set("Discount/" + data.IDDiscount, data);
        }
        public void Delete(string id)
        {
            if(id!="")
            {
                client = new FireSharp.FirebaseClient(config);
                var data = new Discount();
                SetResponse setResponse = client.Set("Discount/" + id, setnull(data));
            }
        }
        public Discount setnull(Discount a)
        {
            a.IDDiscount = null;
            a.IDDiscountType = null;
            a.IDStore = null;
            return a;
        }
    }
}
