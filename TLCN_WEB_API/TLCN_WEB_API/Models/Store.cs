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
        IFirebaseConfig config = new FirebaseConfig {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        IFirebaseClient client;
        public Store() {
            StoreID = "";
            StoreAddress = "";
            StoreName = "";
            StorePicture = "";
            OpenTime = "";
            CLoseTime = "";
            UserID = "";
            ProvinceID = "";
            BusinessTypeID = "";
            RatePoint = "0";
            khoangcach = "";
            Status = "";
            Lat = "";
            Long = "";
            DistrictID = "";
            NumberView = "0";
            Discount = false;
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
        public bool Discount { get; set; }

        string columnname = "Stores";

        public List<Store> getAll(double LatNew, double LongNew) { //lấy danh sách tất cả các quán ăn
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data) {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            if (LatNew != 0 && LongNew != 0) {          //nếu thay đổi location tính lại khoản cách
                foreach (var item in list) {
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list);
        }

        public List<Store> getAllGanToi(double LatNew, double LongNew) {              //lấy danh sách tất cả các quán sắp xếp khoảng cách gần tới xa
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data) {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            if (LatNew != 0 && LongNew != 0) {           //nếu đổi location thì tỉnh lại khoảng cách
                foreach (var item in list) {
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(gantoi(list));
        }

        public List<Store> getAllCheck(double LatNew, double LongNew) {                     //lấy danh sách quán cần xác nhận lên live        
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data) {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            if (LatNew != 0 && LongNew != 0) {               //nếu thay đổi location thì tính lại khoản cách
                foreach (var item in list) {
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            var list2 = new List<Store>();           //lọc quán cần xác nhận với status = -1
            foreach (var item in list) {
                if (item.Status == "-1") {
                    list2.Add(item);
                }
            }
            return list2;
        }

        public List<Store> getAllGanToiProvince(string id, double LatNew, double LongNew) { //lấy danh sách tất cả các quán sắp xếp khoảng cách gần tới xa theo IDProvince
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data) {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list) {                         //lọc quán theo IDProvince thêm vào danh sách
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            if (LatNew != 0 && LongNew != 0) {                          //Nếu thay đổi location thì tính lại khoảng cách
                foreach (var item in list2) {
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(gantoi(list2));
        }
        // thêm dư liệu lên firebase
        public void AddToFireBase(Store store) {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            PushResponse response = client.Push("Stores/", data);
            data.StoreID = response.Result.name;
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }

        public static double Calculate(double sLatitude, double sLongitude, double eLatitude, double eLongitude) {       //tính khoảng cách giữa 2 location 
            var radiansOverDegrees = (Math.PI / 180.0);                   //radian trên độ
            var sLatitudeRadians = sLatitude * radiansOverDegrees;         //tọa độ radian
            var sLongitudeRadians = sLongitude * radiansOverDegrees;
            var eLatitudeRadians = eLatitude * radiansOverDegrees;
            var eLongitudeRadians = eLongitude * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;       //khoang cách giữa kinh độ
            var dLatitude = eLatitudeRadians - sLatitudeRadians;          //khoang cách giữa vĩ độ

            //áp dụng công thức Haversine để tính khoảng cách 
            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // sử dụng 3956 như số dặm quanh trái đất
            var result2 = 3956.0 * 2.0 *                        //bán kính trung bình trái đất tính bằng km = 6,371km = 3958,76 dặm
                          Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return Math.Round(result2, 2);//làm tròn với 2 sô thập phân
        }

        //Update dữ liệu
        public void AddbyidToFireBase(string id, Store store)                //truyền vào iDStore
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
            if (data2 != null) {
                foreach (var item in data2) {
                    list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list) {                          //tìm kiếm nội dung trong csdl để so sánh
                    if (item.StoreID == id)
                        list2.Add(item);
                }
                foreach (var item in list2) {                              //cập nhật các nội dung k chỉnh sửa
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
            }
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }

        public double updateRatePoint(string id) {                            //cập nhật điểm đánh giá của quán truyền vào IDStore
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data) {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list) {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            double a = tinhratepoint(id);
            foreach (var item in list2) {
                item.RatePoint = a.ToString();
                AddbyidToFireBase(item.StoreID, item);
            }
            return a;
        }

        public List<Comment> danhsachrate(string IDStoretinh) {                   //danh sách rating
            Comment comment = new Comment();
            var danhsachcomment = new List<Comment>();
            danhsachcomment = comment.getbyId(IDStoretinh);
            var danhsachUserID = new List<Comment>();
            foreach (var item in danhsachcomment)
            {
                if (danhsachUserID.Count == 0) danhsachUserID.Add(item);
                else
                {
                    int dem = 0;
                    foreach (var item2 in danhsachUserID)
                    {
                        if (item.UserID == item2.UserID) dem++;
                    }
                    if (dem == 0) danhsachUserID.Add(item);
                }
            }
            var danhsachrate = new List<Comment>();
            foreach (var item in danhsachUserID)
            {
                var danhsachcmtuser = comment.getbyId(item.StoreID, item.UserID);
                string ngay = danhsachcmtuser[0].Date.ToString();
                string cmtID = danhsachcmtuser[0].CommentID;
                User user = new User();
                for (int i = 1; i < danhsachcmtuser.Count; i++)
                {
                    if (user.checkAdmin(user.GetEmailByID(danhsachcmtuser[i].UserID)) == false
                        && user.checkOwner(user.GetEmailByID(danhsachcmtuser[i].UserID)) == false)
                    {
                        if (kiemtrangaynaotruoc(ngay, danhsachcmtuser[i].Date) == false)
                        {
                            ngay = danhsachcmtuser[i].Date;
                            cmtID = danhsachcmtuser[i].CommentID;
                        }
                    }
                }
                foreach (var item2 in danhsachcmtuser)
                {
                    if (item2.CommentID == cmtID) danhsachrate.Add(item2);
                }
            }
            return danhsachrate;
        }

        public double tinhratepoint(string IDStoretinh) {
            Comment comment = new Comment();
            var danhsachcomment = new List<Comment>();
            danhsachcomment = comment.getbyId(IDStoretinh);
            var danhsachUserID = new List<Comment>();
            foreach (var item in danhsachcomment)
            {
                if (danhsachUserID.Count == 0) danhsachUserID.Add(item);
                else
                {
                    int dem = 0;
                    foreach(var item2 in danhsachUserID)
                    {
                        if (item.UserID == item2.UserID) dem++;
                    }
                    if (dem == 0) danhsachUserID.Add(item);
                }
            }
            var danhsachrate = new List<Comment>();
            foreach (var item in danhsachUserID)
            {
                var danhsachcmtuser = comment.getbyId(item.StoreID, item.UserID);
                string ngay = danhsachcmtuser[0].Date.ToString();
                string cmtID = danhsachcmtuser[0].CommentID;
                User user = new User();
                for (int i = 1; i < danhsachcmtuser.Count; i++)
                {
                    if(user.checkAdmin(user.GetEmailByID(danhsachcmtuser[i].UserID)) == false 
                        && user.checkOwner(user.GetEmailByID(danhsachcmtuser[i].UserID)) == false)
                    {
                        if (kiemtrangaynaotruoc(ngay, danhsachcmtuser[i].Date) == false)
                        {
                            ngay = danhsachcmtuser[i].Date;
                            cmtID = danhsachcmtuser[i].CommentID;
                        }
                    }                                           
                }
                foreach(var item2 in danhsachcmtuser)
                {
                    if (item2.CommentID == cmtID) danhsachrate.Add(item2);
                }
            }
            double tongrate = 0.0;
            int demrate = 0;
            foreach(var item in danhsachrate)
            {
                tongrate = tongrate + Convert.ToInt32(item.RatePoint);
                demrate++;
            }
            if (demrate == 0) return 0;
            tongrate = tongrate/demrate;
            return Math.Round(tongrate, 2);
        }

        public bool kiemtrangaynaotruoc(string ngay1, string ngay2){ //true là ngày 1 truoc,false la ngay 2 truoc        
            DateTime Time1 = DateTime.Parse(ngay1);
            DateTime Time2 = DateTime.Parse(ngay2);
            if (Time1.Year <= Time2.Year)
            {
                if (Time1.Year == Time2.Year){
                    if(Time1.Month <= Time2.Month){
                        if (Time1.Month == Time2.Month){
                            if (Time1.Day <= Time2.Day){
                                if (Time1.Day == Time2.Day){
                                    string newstring1 = ngay1.Substring(ngay1.Length - 2, 2);
                                    string newstring2 = ngay1.Substring(ngay2.Length - 2, 2);
                                    if (newstring1 == newstring1){
                                        if (Time1.Hour <= Time2.Hour){
                                            if (Time1.Hour == Time2.Hour){
                                                if (Time1.Minute <= Time2.Minute){
                                                    if (Time1.Minute == Time2.Minute){
                                                        if (Time1.Second <= Time2.Second){
                                                            if (Time1.Second == Time2.Second){
                                                                return true;
                                                            }
                                                            else return true;
                                                        }
                                                        else return false;
                                                    }
                                                    else return true;
                                                }
                                                else return false;
                                            }
                                            else return true;
                                        }
                                        else return false;
                                    }
                                    else if (newstring1 == "AM" && newstring2 == "PM") return true;
                                    else if (newstring1 == "PM" && newstring2 == "AM") return false;
                                }
                                else return true;
                            }
                            else return false;
                        }
                        else return true;
                    }
                    else return false;
                }    
                else return true;
            }
            else return false;
            return true;
        }

        public void blockStore(string id, string status) {                                 //đổi status của quán khóa quán
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){                                       //tìm kiếm quán khóa
                if (item.StoreID == id)
                    list2.Add(item);
            }
            foreach (var item in list2){
                if (item.Status == "-1"){                                                           //nếu tài xác nhận quán thì gửi gmail cho chủ khoản là đã xác nhận
                    item.Status = status;
                    AddbyidToFireBase(item.StoreID, item);
                    User infoOwner = new User();
                    UserType a = new UserType();
                    a.sendMail(infoOwner.GetEmailByID(item.UserID), item.StoreName);
                }
                else{
                    item.Status = status;                                                         //Nếu khóa quán thì lưu lại dữ liệu
                    AddbyidToFireBase(item.StoreID, item);
                }               
            }
        }

        //xóa quán
        public void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data = new Store();
            data.StoreID =        null;
            data.StoreAddress =   null;
            data.StoreName =      null;
            data.StorePicture =   null;
            data.OpenTime =       null;
            data.CLoseTime =      null;
            data.UserID =         null;
            data.ProvinceID =     null;
            data.BusinessTypeID = null;
            data.RatePoint =      null;
            data.khoangcach =     null;
            data.Status =         null;
            data.Lat =            null;
            data.Long =           null;
            data.DistrictID =     null;
            data.NumberView =     null;
            SetResponse setResponse = client.Set("Stores/" + id, data);
        }

        //kiểm tra quán có bị khóa hoặc chưa xác nhận hay không status 1 là bình thường
        public List<Store> Check(List<Store> store){
            var list = new List<Store>();
            for (int i = 0; i < store.Count; i++){
                if (store[i].Status == "1")
                    list.Add(store[i]);
            }
            return list;
        }

        
        public List<Store> getByIDOwner(string id,double LatNew,double LongNew) {          //lấy danh sách chủ quán thoe IDUser chủ quán
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){                                      //kiểm tra quán nào thuộc sở hữu chủ thì thêm vào danh sách
                if (item.UserID == id)
                    list2.Add(item);
            }
            if (LatNew != 0 && LongNew != 0){                            //Nếu có thay đổi location thì tính toán lại khoảng cách
                foreach (var item in list2){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> getByID(string id, double LatNew, double LongNew) {            //xem chi tiết quán ăn truyền vào IDStore
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.StoreID == id)                                                   //lấy thông tin quán ăn
                    list2.Add(item);
            }
            View_Store view_Store = new View_Store();
            foreach (var item in list2){                                                //mỗi lần xem chi tiết quán ăn sẽ lưu lại lượt view 
                view_Store.StoreID = item.StoreID;
                view_Store.Date = DateTime.Now.ToString();
                view_Store.Add(view_Store);
            }
            var listview = new List<View_Store>();
            listview = view_Store.getByIDStore(id);
            foreach(var item in list2){                                             //cập nhật lượt view của quán ăn
                item.NumberView = listview.Count().ToString();
                AddbyidToFireBase(item.StoreID, item);
            }
            if (LatNew != 0 && LongNew != 0){                                      //nếu có thay đổi location thì tính toán l
                foreach (var item in list2){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public void ViewWebsite(){
            View_Store view_Store = new View_Store();
            //mỗi lần xem chi tiết quán ăn sẽ lưu lại lượt view 
            view_Store.StoreID = "";
            view_Store.Date = DateTime.Now.ToString();
            view_Store.Add(view_Store);
        }

        public List<Store> getByIDDistrict(string id, double LatNew, double LongNew){             //lấy danh sách quán theo quận truyền vào IDDistrict
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){                                                 //lấy danh sách quán theo quận
                if (item.DistrictID == id)
                    list2.Add(item);
            }
            if (LatNew != 0 && LongNew != 0){                                       //nếu có thay đổi location thì tính toán lại khoảng cách
                foreach (var item in list2){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }


        public List<Store> getByIDManage(string id, double LatNew, double LongNew) {        // lấy danh sách quán cho quản lý không cần check live
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){                                  // them danh sách quán
                if (item.StoreID == id)
                    list2.Add(item);
            }

            if (LatNew != 0 && LongNew != 0){                            //nếu thay đổi location thì tính lại khoảng cách
                foreach (var item in list2){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return list2;
        }
        public List<Store> getByIDProvince(string id,string id2, double LatNew, double LongNew) {      //lấy danh sách quán theo tỉnh truyền vào IDProvince
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.ProvinceID == id)                                //thêm quán vào danh sách
                    list2.Add(item);
            }
            if (LatNew != 0 && LongNew != 0){                                //nếu có thay đổi location thì tính toán lại khoảng cách
                foreach (var item in list2){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> getByIDBusinessType(string id, double LatNew, double LongNew) {      //lấy danh sách quán theo mô hình kinh doanh
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnname);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.BusinessTypeID == id)                                          //thêm quán vào danh sách
                    list2.Add(item);
            }
             if (LatNew != 0 && LongNew != 0){
                foreach (var item in list){
                    if (item.Lat != "" && item.Long != "")
                        item.khoangcach = Calculate(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long), LatNew, LongNew).ToString();
                }
            }
            return Check(list2);
        }

        public List<Store> gantoi(List<Store> store){                      //sắp xếp danh sách quán theo thứ tự từ gần đến xa của khoảng cách
            for (int i = 0; i < store.Count; i++){
                for (int j = i + 1; j < store.Count; j++){
                    if (Convert.ToDouble(store[j].khoangcach) < Convert.ToDouble(store[i].khoangcach)){
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
