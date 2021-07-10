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
    public class District
    {
        public string DistrictID { get; set; }
        public string ProvinceID { get; set; }
        public string DistrictName { get; set; }
        string columnName = "District";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public District(){
            DistrictID = "";
            ProvinceID = "";
            DistrictName = "";
        }

        public List<District> getAll(){                           //lấy thông tin tất cả quận
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<District>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<District>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public List<District> getByID(string id){                             //Xem thông tin chi tiết của quận IDDistrict
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<District>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<District>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<District>();
            foreach (var item in list){
                if (item.DistrictID == id)
                    list2.Add(item);
            }
            return list2;
        }
        public List<District> getByIDProvince(string id){                                    //danh sách quận của thành phố truyền vào IDProvince
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<District>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<District>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<District>();
            foreach (var item in list){
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            return list2;
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(District district){
            client = new FireSharp.FirebaseClient(config);
            var data = district;
            PushResponse response = client.Push("District/", data);
            data.DistrictID = response.Result.name;
            SetResponse setResponse = client.Set("District/" + data.DistrictID, data);
        }

        //update dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, District district){
            client = new FireSharp.FirebaseClient(config);
            var data = district;
            data.DistrictID = id;
            SetResponse setResponse = client.Set("District/" + data.DistrictID, data);
        }
        public void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data = new District();
            SetResponse setResponse = client.Set("District/" + id, data);
        }
        //set null
        private District setnull(District district) {
            district.DistrictID = "";
            district.ProvinceID = "";
            district.DistrictName = "";
            return district;
        }
    }
}
