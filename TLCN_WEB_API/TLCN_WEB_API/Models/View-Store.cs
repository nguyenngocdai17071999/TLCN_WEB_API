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
    public class View_Store
    {
        public string ViewID { get; set; }
        public string StoreID { get; set; }
        public string Date { get; set; }

        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        string columnName = "View_Store";

        IFirebaseClient client;

        public View_Store(){
            ViewID = "";
            StoreID = "";
            Date = "";
        }
        //update dữ liệu
        public void update(string id, View_Store view_Store){
            client = new FireSharp.FirebaseClient(config);
            var data = view_Store;
            data.ViewID = id;
            SetResponse setResponse = client.Set("View_Store/" + data.ViewID, data);
        }

        // thêm dư liệu lên firebase
        public void Add(View_Store view_Store){
            client = new FireSharp.FirebaseClient(config);
            var data = view_Store;
            PushResponse response = client.Push("View_Store/", data);
            data.ViewID = response.Result.name;
            SetResponse setResponse = client.Set("View_Store/" + data.ViewID, data);
        }

        public List<View_Store> getAll(){                                //lấy dữ liệu view của website
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<View_Store>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<View_Store>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<View_Store> getByIDStore(string id){                    //lấy dữ liệu view của quán IDStore
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<View_Store>();
            //danh sách tìm kiếm
            var listViewStore = new List<View_Store>();
            if (data != null){
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<View_Store>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list){
                    if (item.StoreID == id) listViewStore.Add(item);
                }
            }  
            return listViewStore;
        }

        public bool isCheck(int nam){                            //kiểm tra có phải năm nhuận hay không
            if ((nam % 4 == 0 && nam % 100 != 0) || nam % 400 == 0)
                return true;
            return false;
        }
        public int fun(int thang, int nam){                   //hàm tìm số ngày trong tháng
            if (thang == 1 || thang == 3 || thang == 5 || thang == 7 || thang == 8 || thang == 10 || thang == 12) return 31;
            else if (thang == 4 || thang == 6 || thang == 9 || thang == 11) return 30;
            else if (isCheck(nam)) return 29;
            else return 28;
        }
    }
}
