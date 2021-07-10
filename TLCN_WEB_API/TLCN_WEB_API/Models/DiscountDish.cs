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
    public class DiscountDish
    {
        public string DiscountDishID { get; set; }
        public string DishcountTypeID { get; set; }
        public string DishID { get; set; }
        public string StoreID { get; set; }


        string columnName = "DiscountDish";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public List<DiscountDish> getAll(){                       // lấy danh sách khuyến mãi món ăn
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountDish>();
            //danh sách tìm kiếm
            if(data!=null){
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<DiscountDish>(((JProperty)item).Value.ToString()));
                }
            }
            return list;
        }

        public List<DiscountDish> getByID(string id){                    //lấy thông tin khuyến mãi của món ăn truyền vào IDDish
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DiscountDish>();
            var list2 = new List<DiscountDish>();
            if (data != null){
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<DiscountDish>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list){
                    if (item.DishID == id)
                        list2.Add(item);
                }
            }            
            return list2;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(DiscountDish discountDish){
            client = new FireSharp.FirebaseClient(config);
            var data = discountDish;
            PushResponse response = client.Push("DiscountDish/", data);
            data.DiscountDishID = response.Result.name;
            SetResponse setResponse = client.Set("DiscountDish/" + data.DiscountDishID, data);
            //kiem tra quan da co khuyen mai chua
            Discount quan = new Discount();
            var danhsachkmQuan = quan.getByidStore(discountDish.StoreID);
            int dem = 0;
            foreach (var item in danhsachkmQuan){
                if (item.IDDiscountType == discountDish.DishcountTypeID)
                    dem++;
            }
            if (dem == 0){                               // nếu quán chưa có khuyến mãi này thêm khuyến mãi vào danh sách khuyến mãi quán
                Discount a = new Discount();
                a.IDStore = discountDish.StoreID;
                a.IDDiscountType = discountDish.DishcountTypeID;
                a.AddToFireBase(a);
            }
        }

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, DiscountDish discountDish){
            client = new FireSharp.FirebaseClient(config);
            var data = discountDish;
            data.DiscountDishID = id;
            SetResponse setResponse = client.Set("DiscountDish/" + data.DiscountDishID, data);
            
        }
        //Xóa khuyến mãi quán
        public void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data = new DiscountDish();
            SetResponse setResponse = client.Set("DiscountDish/" + id, data);
        }
    }
}
