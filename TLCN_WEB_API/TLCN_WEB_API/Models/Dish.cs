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
        public string DishType_ID { get; set; }
        public string Menu_ID { get; set; }

        private static string key = "TLCN";
        string columnName = "Dishes";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        IFirebaseClient client;
        private IConfiguration _config;
        public Dish(IConfiguration config)
        {
            _config = config;
        }

        public Dish()
        {
        }

        public List<Dish> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<Dish> getByID(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list)
            {
                if (item.Dish_ID == id)
                    list2.Add(item);
            }
            return list2;
        }
        public List<Dish> getByIDMenu(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list)
            {
                if (item.Menu_ID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public List<Store> search(string dishname) {
            dishname = convertToUnSign3(dishname.ToLower());
            Store store = new Store();
            //danh sach store
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse responsestore = client.Get("Stores");
            dynamic datastore = JsonConvert.DeserializeObject<dynamic>(responsestore.Body);
            var liststore = new List<Store>();

            //danh sách tìm kiếm
            foreach (var itemstore in datastore)
            {
                liststore.Add(JsonConvert.DeserializeObject<Store>(((JProperty)itemstore).Value.ToString()));
            }

            //danh sach dish
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse responsedish = client.Get("Dishes");
            dynamic datadish = JsonConvert.DeserializeObject<dynamic>(responsedish.Body);
            var listdish = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var itemdish in datadish)
            {
                listdish.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)itemdish).Value.ToString()));
            }

            //danh sach menu
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse responseMenu = client.Get("Menu");
            dynamic datamenu = JsonConvert.DeserializeObject<dynamic>(responseMenu.Body);
            var listMenu = new List<Menu>();

            //danh sách tìm kiếm
            foreach (var itemMenu in datamenu)
            {
                listMenu.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)itemMenu).Value.ToString()));
            }

            var list2 = new List<Store>();
            var MenuID = new List<string>();

            foreach (var item in listdish)
            {
                if ((convertToUnSign3(item.DishName.ToLower())).Contains(dishname))
                {
                    MenuID.Add(item.Menu_ID);
                }
            }
            var MenuID2 = new List<string>();
            foreach (var item in MenuID)
            {
                MenuID2.Add(item);
                break;
            }
            foreach (var item in MenuID)
            {
                int dem = 0;
                foreach (var item2 in MenuID2)
                {
                    if (item == item2)
                        dem++;
                }
                if (dem == 0) MenuID2.Add(item);
            }
            if (MenuID2.Count == 0)
            {
                var list3 = new List<Store>();
                foreach (var item in liststore)
                {
                    if ((convertToUnSign3(item.StoreName.ToLower())).Contains(dishname))
                        list3.Add(item);
                }
                if (list3.Count == 0)
                {
                    var list4 = new List<Store>();
                    foreach (var item in liststore)
                    {
                        if ((convertToUnSign3(item.StoreAddress.ToLower())).Contains(dishname))
                            list4.Add(item);
                    }

                    return store.Check(list4);

                }
                return store.Check(list3);
            }
            else
            {
                foreach (var item in liststore)
                {
                    foreach (var item2 in MenuID2)
                    {
                        if (item.MenuID == item2) list2.Add(item);
                    }
                }
                return store.Check(list2);
            }
        }

        public List<Dish> getbyIdType(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list)
            {
                if (item.DishType_ID == id)
                    list2.Add(item);
            }
            return list2;
        }

        public string convertToUnSign3(string s)
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

        //thêm dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, Dish dish)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = dish;
            data.Dish_ID = id;
            SetResponse setResponse = client.Set("Dishes/" + data.Dish_ID, data);
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Dish();
            //data.Dish_ID = id;
            SetResponse setResponse = client.Set("Dishes/" + id, data);
        }
    }
}
