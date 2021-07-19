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
        public string DiscountRule { get; set; }
        public string IDStore { get; set; }
        public string Content { get; set; }
        string columnName = "DiscountType";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public DiscountType(){
            DiscountTypeID = "";
            DiscountTypeName = "";
            DiscountTypePicture = "";
            DiscountRule = "";
            IDStore = "";
            Content = "";
        }

        public List<DiscountType> getAll(){                           //lấy danh sách loại khuyến mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<DiscountType> getAllAmin()
        {                           //lấy danh sách loại khuyến mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            var list2 = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }

            foreach(var item in list) {
                if (item.DiscountRule == "Admin") list2.Add(item);
            }
            return list2;
        }

        public List<DiscountType> getAllOwner(String IDStore)
        {                           //lấy danh sách loại khuyến mãi
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            var list2 = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }

            foreach (var item in list)
            {
                if (item.DiscountRule == "Owner" && item.IDStore==IDStore) list2.Add(item);
            }
            return list2;
        }

        public List<DiscountType> getByID(string id){                    // xem thông tin loại khuyến mãi truyền vào IDDiscountType
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }
            //thông tin khuyến mãi loại
            var list2 = new List<DiscountType>();
            foreach (var item in list){
                if (item.DiscountTypeID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public DiscountType getByIDDiscount(string id)
        {                    // xem thông tin loại khuyến mãi truyền vào IDDiscountType
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountType>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<DiscountType>(((JProperty)item).Value.ToString()));
            }
            //thông tin khuyến mãi loại
            var list2 = new DiscountType();
            foreach (var item in list)
            {
                if (item.DiscountTypeID == id)
                    list2=item;
            }
            return list2;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(DiscountType discountType){
            client = new FireSharp.FirebaseClient(config);
            var data = discountType;
            PushResponse response = client.Push("DiscountType/", data);
            data.DiscountTypeID = response.Result.name;
            SetResponse setResponse = client.Set("DiscountType/" + data.DiscountTypeID, data);
        }

        public void AddToFireBaseOwner(string IDStore,DiscountType discountType)                     //chủ quán tạo
        {
            client = new FireSharp.FirebaseClient(config);
            var data = discountType;
            PushResponse response = client.Push("DiscountType/", data);
            data.DiscountTypeID = response.Result.name;
            SetResponse setResponse = client.Set("DiscountType/" + data.DiscountTypeID, data);
            Discount discount = new Discount();
            discount.IDDiscountType = data.DiscountTypeID;
            discount.IDStore = IDStore;
            discount.AddToFireBase(discount);
        }

        //Update dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, DiscountType discountType){
            client = new FireSharp.FirebaseClient(config);

            var data = discountType;
            data.DiscountTypeID = id;
            SetResponse setResponse = client.Set("DiscountType/" + data.DiscountTypeID, data);
        }
        //Xóa dữ liệu
        public void Delete(string id) {
            client = new FireSharp.FirebaseClient(config);
            var data = new DiscountType();
            SetResponse setResponse = client.Set("DiscountType/" + id, setnull(data));
            Discount discount = new Discount();
            var danhsachdiscount = discount.getAll();
            foreach (var item in danhsachdiscount)      //xóa khuyến mãi quán
            {
                if (item.IDDiscountType == id)
                    discount.Delete(item.IDDiscount);
            }
            DiscountDish discountDish = new DiscountDish();
            var danhsachdiscountdish = discountDish.getAll();
            foreach(var item in danhsachdiscountdish)                      //xóa khuyến mãi món ăn
            {
                if (item.DishcountTypeID == id)
                    discountDish.Delete(item.DiscountDishID);
            }
        }
        //Set null dữ liệu
        public DiscountType setnull(DiscountType a){
            a.DiscountTypeID = null;
            a.DiscountTypeName = null;
            a.DiscountTypePicture = null;
            a.IDStore = null;
            a.Content = null;
            a.DiscountRule = null;
            return a;
        }
    }
}
