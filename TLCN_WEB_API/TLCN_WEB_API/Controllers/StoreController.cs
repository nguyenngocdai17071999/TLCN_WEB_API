﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLCN_WEB_API.Models;

namespace TLCN_WEB_API.Controllers
{
    //[EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private static string key = "TLCN";
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public StoreController(IConfiguration config){
            _config = config;
        }
        IFirebaseClient client;

        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                return Ok(Check(list));
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetAllManage")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllmanage()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();
                //danh sách tìm kiếm
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                return Ok(list);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToi")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                return Ok(Check(gantoi(list)));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToiProvince")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(string id){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list){
                    if (item.ProvinceID == id)
                        list2.Add(item);
                }    
                return Ok(Check(gantoi(list2)));
            }
            catch{
                return Ok("Error");
            }
        }

        private void AddToFireBasebydis(LatLongStore latLongStore){
            client = new FireSharp.FirebaseClient(config);
            var data = latLongStore;
            //PushResponse response = client.Push("LatLongStore/", data);
            data.IDStore = latLongStore.IDStore;
            SetResponse setResponse = client.Set("LatLongStore/" + data.IDStore, data);
        }

        [HttpPost("CreateLatLong")]
        public IActionResult CreateDIS(LatLongStore latLongStore){
            try{
                AddToFireBasebydis(latLongStore);
                return Ok("OK");
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDLatLong")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDLatLong(string id)
        {
            try
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
                    if (item.IDStore == id)
                        list2.Add(item);
                }
                return Ok(list2);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetALLDistance")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetDistanceByIDall(string ID){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("LatLongStore");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<LatLongStore>();
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
                }
                return Ok(list);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetDistance")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetDistanceByID(string ID){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("LatLongStore");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<LatLongStore>();
                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list){
                    if (item.IDStore == ID)
                        return Ok(Calculate(10.851590, 106.763280, Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long)));
                }
                return Ok(list);
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetByIDOwner")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDOwner(string id){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();

                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list){
                    if (item.UserID == id)
                        list2.Add(item);
                }
                return Ok(Check(list2));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();

                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list){
                    if (item.StoreID == id)
                        list2.Add(item);
                }
                return Ok(Check(list2));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDManage")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDManage(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();

                //danh sách tìm kiếm
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list)
                {
                    if (item.StoreID == id)
                        list2.Add(item);
                }
                return Ok(list2);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDProvince")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDProvince(string id,string id2){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();

                //danh sách tìm kiếm
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list){
                    if (item.ProvinceID == id)
                        list2.Add(item);
                }
                if(id2!=null){
                    foreach (var item in list2){
                        item.ProvinceID = id2;
                        AddbyidToFireBase(item.StoreID, item);
                    }
                }                
                return Ok(Check(list2));
            }
            catch{
                return Ok("Error");
            }            
        }

        [HttpGet("GetByIDBusinessType")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDBusinessType(string id){
            try{
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Stores");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<GanToi>();
                //danh sách tìm kiếm

                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<GanToi>();
                foreach (var item in list){
                    if (item.BusinessTypeID == id)
                        list2.Add(item);
                }
                return Ok(Check(list2));
            }
            catch{
                return Ok("Error");
            }           
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] GanToi store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV" || GetRole(Email) == "-MO5VWchsca2XwktyNAw")
                    {
                        try{
                            AddbyidToFireBase(id, store);
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }                 
        }


        [HttpPost("UpdateRatePoint")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult UpdateRatePoint(string id, string RatePoint)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse response = client.Get("Stores");
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                    var list = new List<GanToi>();
                    //danh sách tìm kiếm
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                    }
                    var list2 = new List<GanToi>();
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
                    return Ok("Cập nhật thành công");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult deleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV" || GetRole(Email) == "-MO5VWchsca2XwktyNAw"){
                        try{
                            Delete(id);
                            return Ok(new[] { "Xóa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("BlockStore")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult BlockStore(string id, string status){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV"){
                        try{
                            client = new FireSharp.FirebaseClient(config);
                            FirebaseResponse response = client.Get("Stores");
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                            var list = new List<GanToi>();
                            //danh sách tìm kiếm
                            foreach (var item in data){
                                list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
                            }
                            var list2 = new List<GanToi>();
                            foreach (var item in list){
                                if (item.StoreID == id)
                                    list2.Add(item);
                            }
                            foreach (var item in list2){
                                item.Status = status;
                                AddbyidToFireBase(item.StoreID, item);
                            }
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("CreateStore")]
        public IActionResult RegisterStore( [FromBody] GanToi store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV"){
                        string err = "";
                        try{
                            AddToFireBase(store);
                            err = "Đăng ký thành công";
                        }
                        catch{
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
            
        }

        //tim ra ID tự động bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Stores");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<GanToi>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1){
                int dem = 0;
                foreach (var item in list){
                    if (item.StoreID == i.ToString())
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(GanToi store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            PushResponse response = client.Push("Stores/", data);
            data.StoreID = response.Result.name;
            data.Status = "1";
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, Store store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;
            SetResponse setResponse = client.Set("Store/" + data.StoreID, data);
        }
        private void AddbyidToFireBase(string id, GanToi store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;

            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Stores");
            dynamic data2 = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<GanToi>();
            var list2 = new List<GanToi>();
            //danh sách tìm kiếm
            foreach (var item in data2){
                list.Add(JsonConvert.DeserializeObject<GanToi>(((JProperty)item).Value.ToString()));
            }
            foreach(var item in list){
                if (item.StoreID == id)
                    list2.Add(item);
            }    
            foreach(var item in list2){
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

        private void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data = new Store();
           // data.StoreID = id;         

            SetResponse setResponse = client.Set("Stores/" + id, data);
        }

        public static string Decrypt(string toDecrypt){
            try{
                bool useHashing = true;
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                if (useHashing){
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch{
                return "Loi roi";
            }
        }
        public string GetRole(string Email){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            foreach (var item in list){
                if (item.Email.ToString() == Email)
                    return item.UserTypeID;
            }
            return "";
        }

        private UserModel AuthenticationUser(UserModel login){
            //get list user
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";

            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            //layas thongo tin taif khoan dang nhap
            UserModel user = null;
            foreach (var item in list){
                if (item.Email == login.EmailAddress && item.Password == Encrypt(login.PassWord)){
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password) };
                }
            }
            return user;
        }

        ////sap xep theo khoang cach gan toi
        //public List<GanToi> gantoi(List<Store> store)
        //{
        //    var ListGanToi = new List<GanToi>();
        //    for (int i = 0; i < store.Count; i++)
        //    {
        //        ListGanToi.Add(new GanToi(store[i].StoreID,
        //                     store[i].StoreAddress,
        //                     store[i].StoreName,
        //                     store[i].StorePicture,
        //                     store[i].OpenTime,
        //                     store[i].CLoseTime,
        //                     store[i].UserID,
        //                     store[i].ProvinceID,
        //                     store[i].MenuID,
        //                     store[i].BusinessTypeID,
        //                     store[i].RatePoint,
        //                     tinhtoankhoangcach(store[i].StoreID).ToString()));
        //    }
        //    GanToi a = new GanToi();
        //    for (int i = 0; i < ListGanToi.Count; i++)
        //    {
        //        for (int j = i + 1; j < ListGanToi.Count; j++)
        //        {
        //            if (Convert.ToDouble(ListGanToi[j].khoangcach) < Convert.ToDouble(ListGanToi[i].khoangcach))
        //            {
        //                //cach trao doi gia tri
        //                a = ListGanToi[i];
        //                ListGanToi[i] = ListGanToi[j];
        //                ListGanToi[j] = a;
        //            }
        //        }
        //    }

        //    return ListGanToi;
        //}

        public List<GanToi> gantoi(List<GanToi> store){
            var ListGanToi = new List<GanToi>();
            for (int i = 0; i < store.Count; i++){
                ListGanToi.Add(new GanToi(store[i].StoreID,
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
            GanToi a = new GanToi();
            for (int i = 0; i < ListGanToi.Count; i++){
                for (int j = i + 1; j < ListGanToi.Count; j++){
                    if (Convert.ToDouble(ListGanToi[j].khoangcach) < Convert.ToDouble(ListGanToi[i].khoangcach)){
                        //cach trao doi gia tri
                        a = ListGanToi[i];
                        ListGanToi[i] = ListGanToi[j];
                        ListGanToi[j] = a;
                    }
                }
            }    
            return ListGanToi;
        }

        private void AddToFireBasemoi(GanToi store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            // PushResponse response = client.Push("Stores/", data);
            //data.StoreID = response.Result.name;
            SetResponse setResponse = client.Set("Stores/" + data.StoreID, data);
        }
        private List<GanToi> Check(List<GanToi> store){
            var list = new List<GanToi>();
            for(int i=0; i<store.Count;i++){
                if (store[i].Status == "1")
                    list.Add(store[i]);
            }    
            return list;
        }

        public double tinhtoankhoangcach(string idStore){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("LatLongStore");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<LatLongStore>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<LatLongStore>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<LatLongStore>();
            foreach (var item in list) {
                if (item.IDStore == idStore)
                    return Calculate(10.851590, 106.763280, Convert.ToDouble(item.Lat), Convert.ToDouble(item.Long));
            }
            return 0;
        }

        // mã hóa dữ liệu MD5
        public static string Encrypt(string toEncrypt){
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing){
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        //thuc hien tao token
        private string GenerateJSONWebToken(UserModel userinfo){
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }


            public static double Calculate(double sLatitude, double sLongitude, double eLatitude,double eLongitude){
            
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

            public bool kiemtrathoigianlogin(DateTime date){
                int sophut1 = date.Minute;
                int sophut2 = DateTime.Now.Minute;
                if (sophut2 < sophut1)
                    sophut2 = sophut2 + 60;
                int ketqua = sophut2 - sophut1;
                if (ketqua < 30) return true;
                return false;
            }
    }
}
