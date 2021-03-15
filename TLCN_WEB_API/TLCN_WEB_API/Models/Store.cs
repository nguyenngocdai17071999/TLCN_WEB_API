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
    public class Store
    {
        private string v;
        private static string key = "TLCN";
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public Store(IConfiguration config)
        {
            _config = config;
        }
        IFirebaseClient client;
        public Store()
        {

        }

        public Store(string storeID, string storeAddress, string storeName, string storePicture, string openTime, string cLoseTime, string userID, string provinceID, string menuID, string businessTypeID, string ratePoint, string khoangcach, string status)
        {
            StoreID = storeID;
            StoreAddress = storeAddress;
            StoreName = storeName;
            StorePicture = storePicture;
            OpenTime = openTime;
            CLoseTime = cLoseTime;
            UserID = userID;
            ProvinceID = provinceID;
            MenuID = menuID;
            BusinessTypeID = businessTypeID;
            RatePoint = ratePoint;
            this.khoangcach = khoangcach;
            Status = status;
        }

        public string StoreID { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string StorePicture { get; set; }
        public string OpenTime { get; set; }
        public string CLoseTime { get; set; }
        public string UserID { get; set; }
        public string ProvinceID { get; set; }
        public string MenuID { get; set; }
        public string BusinessTypeID { get; set; }
        public string RatePoint { get; set; }
        public string khoangcach { get; set; }
        public string Status { get; set; }
        string columnname = "Stores";

        public List<Store> getAll() {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            return Check(list);
        }
        public List<Store> getAllManage()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<Store> getAllGanToi()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            return Check(gantoi(list));
        } 
        public List<Store> getAllGanToiProvince(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            return Check(gantoi(list2));
        }
        // thêm dư liệu lên firebase
        public void AddToFireBase(Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            PushResponse response = client.Push("Stores/", data);
            data.StoreID = response.Result.name;
            data.Status = "1";
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }
        public static double Calculate(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {

            var radiansOverDegrees = (Math.PI / 180.0);
            var sLatitudeRadians = sLatitude * radiansOverDegrees;
            var sLongitudeRadians = sLongitude * radiansOverDegrees;
            var eLatitudeRadians = eLatitude * radiansOverDegrees;
            var eLongitudeRadians = eLongitude * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Using 3956 as the number of miles around the earth
            var result2 = 3956.0 * 2.0 *
                          Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return Math.Round(result2, 2);
        }
        public double tinhtoankhoangcach(string idStore)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("LatLongStore");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<LatLongStore>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<LatLongStore>();
            foreach (var item in list)
            {
                if (item.IDStore == idStore)
                    return Calculate(10.851590, 106.763280, Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long));
            }
            return 0;
        }
        public void AddbyidToFireBase(string id, Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;

            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Stores");
            dynamic data2 = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            var list2 = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data2)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            foreach (var item in list2)
            {
                if (data.StoreID == null) data.StoreID = item.StoreID;
                if (data.StoreAddress == null) data.StoreAddress = item.StoreAddress;
                if (data.StoreName == null) data.StoreName = item.StoreName;
                if (data.StorePicture == null) data.StorePicture = item.StorePicture;
                if (data.OpenTime == null) data.OpenTime = item.OpenTime;
                if (data.CLoseTime == null) data.CLoseTime = item.CLoseTime;
                if (data.UserID == null) data.UserID = item.UserID;
                if (data.ProvinceID == null) data.ProvinceID = item.ProvinceID;
                if (data.MenuID == null) data.MenuID = item.MenuID;
                if (data.BusinessTypeID == null) data.BusinessTypeID = item.BusinessTypeID;
                if (data.RatePoint == null) data.RatePoint = item.RatePoint;
                if (data.khoangcach == null) data.khoangcach = item.khoangcach;
                if (data.Status == null) data.Status = item.Status;
            }
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }
        public void updateRatePoint(string id, string RatePoint) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            foreach (var item in list2)
            {
                item.RatePoint = RatePoint;
                AddbyidToFireBase(item.StoreID, item);
            }
        }

        public void blockStore(string id, string status) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            foreach (var item in list2)
            {
                item.Status = status;
                AddbyidToFireBase(item.StoreID, item);
            }
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Store();
            // data.StoreID = id;
            SetResponse setResponse = client.Set("Stores/" + id, data);
        }
        public void AddToFireBasemoi(Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            // PushResponse response = client.Push("Stores/", data);
            //data.StoreID = response.Result.name;
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }
        public List<Store> Check(List<Store> store)
        {
            var list = new List<Store>();
            for (int i = 0; i < store.Count; i++)
            {
                if (store[i].Status == "1")
                    list.Add(store[i]);
            }
            return list;
        }
        public void AddToFireBasebydis(LatLongStore latLongStore)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = latLongStore;
            //PushResponse response = client.Push("LatLongStore/", data);
            data.IDStore = latLongStore.IDStore;
            SetResponse setResponse = client.Set("LatLongStore/" + data.IDStore, data);
        }

        public List<LatLongStore> getByIDLatLong(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("LatLongStore");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<LatLongStore>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<LatLongStore>();
            foreach (var item in list)
            {
                if (item.IDStore == id)
                    list2.Add(item);
            }
            return list2;
        }
        public List<LatLongStore> getALLDistance(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("LatLongStore");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<LatLongStore>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<Store> getByIDOwner(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.UserID == id)
                    list2.Add(item);
            }
            return Check(list2);
        }
        public List<Store> getByID(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return Check(list2);
        }
        public List<Store> getByIDManage(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return list2;
        }
        public List<Store> getByIDProvince(string id,string id2) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            if (id2 != null)
            {
                foreach (var item in list2)
                {
                    item.ProvinceID = id2;
                    AddbyidToFireBase(item.StoreID, item);
                }
            }
            return Check(list2);
        }
        public List<Store> getByIDBusinessType(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.BusinessTypeID == id)
                    list2.Add(item);
            }
            return Check(list2);
        }


       
        public double getDistance(string id) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("LatLongStore");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<LatLongStore>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<LatLongStore>();
            foreach (var item in list)
            {
                if (item.IDStore == id)
                    return Calculate(10.851590, 106.763280, Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long));
            }
            return 0;
        }



        public List<Store> gantoi(List<Store> store)
        {
            var ListGanToi = new List<Store>();
            for (int i = 0; i < store.Count; i++)
            {
                ListGanToi.Add(new Store(store[i].StoreID,
                             store[i].StoreAddress,
                             store[i].StoreName,
                             store[i].StorePicture,
                             store[i].OpenTime,
                             store[i].CLoseTime,
                             store[i].UserID,
                             store[i].ProvinceID,
                             store[i].MenuID,
                             store[i].BusinessTypeID,
                             store[i].RatePoint,
                             store[i].khoangcach,
                             store[i].Status));
            }
            Store a = new Store();
            for (int i = 0; i < ListGanToi.Count; i++)
            {
                for (int j = i + 1; j < ListGanToi.Count; j++)
                {
                    if (Convert.ToDouble(ListGanToi[j].khoangcach) < Convert.ToDouble(ListGanToi[i].khoangcach))
                    {
                        //cach trao doi gia tri
                        a = ListGanToi[i];
                        ListGanToi[i] = ListGanToi[j];
                        ListGanToi[j] = a;
                    }
                }
            }
            return ListGanToi;
        }
    }
}
