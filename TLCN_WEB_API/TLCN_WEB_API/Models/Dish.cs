using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Dish
    {
        public string Dish_ID { get; set; }
        public string DishName { get; set; }
        public string DishPrice { get; set; }
        public string DishPicture { get; set; }
        public string Store_ID { get; set; }

        private static string key = "TLCN";
        string columnName = "Dishes";
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        IFirebaseClient client;

        public Dish(){
            Dish_ID = "";
            DishName = "";
            DishPrice = "";
            DishPicture = "";
            Store_ID = "";            
        }

        public List<Dish> getAll() {                                       //lấy dữ liệu tất cả món ăn
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<Dish> getByID(string id) {                             //Xem thông tin món ăn IDDish
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();      
            foreach (var item in list){
                if (item.Dish_ID == id)
                    list2.Add(item);
            }
            return list2;
        }
        public List<Dish> getByIDStore(string id) {                         //Lấy dữ liệu tất cả món ăn của quán ăn IDStore
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list){              //kiểm tra món ăn của quán thì thêm vào danh sách
                if (item.Store_ID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public List<Store> search(string dishname,double Lat, double Long) {            //Hàm tìm kiếm quán ăn theo Keyword
            dishname = convertToUnSign3(dishname.ToLower());                         //Bỏ dấu keyword
            Store Store = new Store();
            Dish  Dish = new Dish();

            var StoreID = new List<string>();                                       
            var danhsachStore = Store.getAll(Lat, Long);                            //danh sách quán
            var danhsachDish = Dish.getAll();                                       //danh sách món ăn

            foreach (var item in danhsachDish){                                          //bỏ dấu tên món ăn để so sánh
                if ((convertToUnSign3(item.DishName.ToLower())).Contains(dishname)){
                    StoreID.Add(item.Store_ID);
                }
            }

            if (StoreID.Count == 0){                                                        //bỏ dấu tên quán ăn để so sánh
                foreach (var item in danhsachStore){                                         // nếu dựa vào tìm quán theo món ăn có sẽ k tìm theo tên quán
                    if ((convertToUnSign3(item.StoreName.ToLower())).Contains(dishname)){
                        StoreID.Add(item.StoreID);
                    }
                }
            }

            var StoreID_Checklap = new List<string>();                             //kiêm tra ID các quán tránh trùng lặp
            foreach (var item in StoreID){
                int dem = 0;
                foreach (var item2 in StoreID_Checklap){
                    if (item == item2)
                        dem++;
                }
                if (dem == 0) StoreID_Checklap.Add(item);
            }

            List<Store> danhsachSearch = new List<Store>();

            foreach(var item in danhsachStore){                              //them vao danh sách tìm kiếm các quán có ID trống trong search
                foreach (var item2 in StoreID_Checklap){
                    if (item.StoreID == item2)
                        danhsachSearch.Add(item);
                }
            }
            return Store.Check(danhsachSearch);
        }


        public string convertToUnSign3(string s)                                     //hàm lược bỏ các dấu trong chữ
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(Dish dish)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = dish;
            PushResponse response = client.Push("Dishes/", data);
            data.Dish_ID = response.Result.name;
            SetResponse setResponse = client.Set("Dishes/" + data.Dish_ID, data);
        }

        //Update dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Dish dish)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = dish;
            data.Dish_ID = id;
            SetResponse setResponse = client.Set("Dishes/" + data.Dish_ID, data);
        }
        //Xóa data
        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Dish();
            //data.Dish_ID = id;
            SetResponse setResponse = client.Set("Dishes/" + id, setnull(data));
        }
        //set null các thuộc tính model
        private Dish setnull(Dish dish)
        {
            dish.Dish_ID = null;
            dish.DishName = null;
            dish.DishPrice = null;
            dish.DishPicture = null;
            dish.Store_ID = null;
            return dish;
        }
    }
}
