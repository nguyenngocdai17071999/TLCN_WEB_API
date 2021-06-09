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
            StoreID = "";
            StoreAddress = "";
            StoreName = "";
            StorePicture = "";
            OpenTime = "";
            CLoseTime = "";
            UserID = "";
            ProvinceID = "";
            BusinessTypeID = "";
            RatePoint = "";
            khoangcach = "";
            Status = "";
            Lat = "";
            Long = "";
            DistrictID = "";
            NumberView = "0";
        }

        public string StoreID { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string StorePicture { get; set; }
        public string OpenTime { get; set; }
        public string CLoseTime { get; set; }
        public string UserID { get; set; }
        public string ProvinceID { get; set; }
        public string BusinessTypeID { get; set; }
        public string RatePoint { get; set; }
        public string khoangcach { get; set; }
        public string Status { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string DistrictID { get; set; }
        public string NumberView { get; set; }
        string columnname = "Stores-New";

        public List<Store> getAll(double LatNew, double LongNew) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }   
            if(LatNew!=0 && LongNew!=0)
            {
                foreach (var item in list)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list);
        }

        public List<Store> getAllGanToi(double LatNew, double LongNew)
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
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(gantoi(list));
        }

        public List<Store> getAllCheck(double LatNew, double LongNew)
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
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            var list2 = new List<Store>();
            foreach(var item in list)
            {
                if(item.Status=="-1")
                {
                    list2.Add(item);
                }    
            }
            return list2;
        }

        public List<Store> getAllGanToiProvince(string id, double LatNew, double LongNew)
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
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(gantoi(list2));
        }
        // thêm dư liệu lên firebase
        public void AddToFireBase(Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            PushResponse response = client.Push("Stores-New/", data);
            data.StoreID = response.Result.name;
            SetResponse setResponse = client.Set("Stores-New/" + data.StoreID, data);
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

        public void AddbyidToFireBase(string id, Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;

            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Stores-New");
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
                if (data.BusinessTypeID == null) data.BusinessTypeID = item.BusinessTypeID;
                if (data.RatePoint == null) data.RatePoint = item.RatePoint;
                if (data.khoangcach == null) data.khoangcach = item.khoangcach;
                if (data.Status == null) data.Status = item.Status;
                if (data.Lat == null) data.Status = item.Lat;
                if (data.Long == null) data.Status = item.Long;
            }
            SetResponse setResponse = client.Set("Stores-New/" + data.StoreID, data);
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
                if (item.Status == "-1")
                {
                    item.Status = status;
                    AddbyidToFireBase(item.StoreID, item);
                    User infoOwner = new User();
                    infoOwner.sendMail(infoOwner.GetEmailByID(item.UserID), item.StoreName);
                }
                else
                {
                    item.Status = status;
                    AddbyidToFireBase(item.StoreID, item);
                }               
            }
        }

        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Store();
            // data.StoreID = id;
            SetResponse setResponse = client.Set("Stores-New/" + id, data);
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

        public List<Store> getByIDOwner(string id,double LatNew,double LongNew) {
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
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> getByID(string id, double LatNew, double LongNew) {
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
            View_Store view_Store = new View_Store();
            foreach (var item in list2)
            {
                view_Store.StoreID = item.StoreID;
                view_Store.Date = DateTime.Now.ToString();
                view_Store.Add(view_Store);
            }
            var listview = new List<View_Store>();
            listview = view_Store.getByIDStore(id);
            foreach(var item in list2)
            {
                item.NumberView = listview.Count().ToString();
                AddbyidToFireBase(item.StoreID, item);
            }
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> getByIDDistrict(string id, double LatNew, double LongNew)
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
                if (item.DistrictID == id)
                    list2.Add(item);
            }
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return list2;
        }


        public List<Store> getByIDManage(string id, double LatNew, double LongNew) {
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

            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return list2;
        }
        public List<Store> getByIDProvince(string id,string id2, double LatNew, double LongNew) {
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
            if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list2)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }
        public List<Store> getByIDBusinessType(string id, double LatNew, double LongNew) {
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
             if (LatNew != 0 && LongNew != 0)
            {
                foreach (var item in list)
                {
                    item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> gantoi(List<Store> store)
        {
            for (int i = 0; i < store.Count; i++)
            {
                for (int j = i + 1; j < store.Count; j++)
                {
                    if (Convert.ToDouble(store[j].khoangcach) < Convert.ToDouble(store[i].khoangcach))
                    {
                        var a = new Store();
                        //cach trao doi gia tri
                        a = store[i];
                        store[i] = store[j];
                        store[j] = a;
                    }
                }
            }
            return store;
        }
    }
}
