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
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public List<Discount> getAll(){                          // lấy tất cả dữ liệu khuyến mãi
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

        public List<Store> getByidDiscountType(string idDiscountType){            //lấy danh sách khuyến mãi theo loại khuyên mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Discount>();
            foreach (var item in list){
                if (item.IDDiscountType == idDiscountType)                    //danh sách khuyến mãi
                    list2.Add(item);
            }
            Store a = new Store();
            var danhsachstore = a.getAll(0,0);
            var danhsachcantim = new List<Store>();
            foreach(var item in list2){
                foreach(var item2 in danhsachstore){
                    if (item2.StoreID == item.IDStore)                     //danh sách quán khuyến mãi  
                        danhsachcantim.Add(item2);
                }
            }
            return danhsachcantim;
        }

        public string getiddiscount(string idstore, string iddiscounttype){              //xem thông tin ID khuyến mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            
            foreach(var item in list){
                if (item.IDStore == idstore && item.IDDiscountType == iddiscounttype){
                    return item.IDDiscount;
                }
            }
            return "";
        }

        public Discount getDiscount(string iddiscount)
        {              //xem thông tin ID khuyến mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            var discount = new Discount();
            foreach (var item in list)
            {
                if (item.IDDiscount == iddiscount)
                {
                    discount=item;
                }
            }
            return discount;
        }

        public List<Discount> getByidStore(string IdStore){                            //lấy danh sách khuyến mãi của quán
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Discount>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Discount>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Discount>();
            foreach (var item in list){
                if (item.IDStore == IdStore)
                    list2.Add(item);
            }
            return list2;
        }

        public bool kiemtrakhuyenmai(string IDStoreCheck, string IDDiscountTypeCheck) {
            var danhsachkhuyenmai = new List<Discount>();
            danhsachkhuyenmai = getByidStore(IDStoreCheck);
            foreach(var item in danhsachkhuyenmai) {
                if (item.IDDiscountType == IDDiscountTypeCheck)
                    return true;
            }
            return false;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(Discount discount){
            if (kiemtrakhuyenmai(discount.IDStore, discount.IDDiscountType) == false) {
                client = new FireSharp.FirebaseClient(config);
                var data = discount;
                PushResponse response = client.Push("Discount/", data);
                data.IDDiscount = response.Result.name;
                SetResponse setResponse = client.Set("Discount/" + data.IDDiscount, data);
                Store danhsachstore = new Store();
                var store = danhsachstore.getByID(discount.IDStore, 0, 0);
                foreach (var item in store){ //bật biến khuyến mãi của quán thành true                
                    if(item.Discount==false){
                        item.Discount = true;
                        danhsachstore.AddbyidToFireBase(item.StoreID, item);
                    }                    
                }
            }
           
        }

        //update dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Discount discount){
            client = new FireSharp.FirebaseClient(config);
            var data = discount;
            data.IDDiscount = id;
            SetResponse setResponse = client.Set("Discount/" + data.IDDiscount, data);
        }
        //xóa dữ liệu IDDiscount
        public void Delete(string id){
            if(id!=""){
                client = new FireSharp.FirebaseClient(config);
                var discounts = getDiscount(id);           //lấy thông tin khuyến mãi
                var data = new Discount();
                SetResponse setResponse = client.Set("Discount/" + id, setnull(data));


                var danhsachkhuyenmai = getByidStore(discounts.IDStore);
                if (danhsachkhuyenmai.Count==0)       //nếu chỉ có 1 khuyến mãi của quán
                {

                        Store danhsachstore = new Store();
                        var store = danhsachstore.getByID(discounts.IDStore, 0, 0);
                        foreach (var item2 in store) //tắt biến khuyến mãi của quán thành false
                        {
                            item2.Discount = false;
                            danhsachstore.AddbyidToFireBase(item2.StoreID, item2);
                        }
                    
                }
            }
        }
        //set null các thành phần dữ liệu đễ xóa
        public Discount setnull(Discount a){
            a.IDDiscount = null;
            a.IDDiscountType = null;
            a.IDStore = null;
            return a;
        }
    }
}
