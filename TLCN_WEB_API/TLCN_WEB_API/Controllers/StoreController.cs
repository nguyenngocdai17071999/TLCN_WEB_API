using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        IFirebaseClient client;

        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            //foreach (var item in list)
            //{
            //    item.UserID = "-MO5VBnzdGsuypsTzHaV";
            //    //Menu
            //    if (item.MenuID == 1.ToString()) item.MenuID = "-MO5Zzr3lkP4JyMDWlMB";
            //    if (item.MenuID == 2.ToString()) item.MenuID = "-MO5_-8EkRDBaXKaf8I1";
            //    if (item.MenuID == 3.ToString()) item.MenuID = "-MO5_-PrbERuXTtThvBz";
            //    if (item.MenuID == 4.ToString()) item.MenuID = "-MO5_-hkA3gy-oHFetwx";
            //    if (item.MenuID == 5.ToString()) item.MenuID = "-MO5_-y7xiYExLHQHr75";
            //    if (item.MenuID == 6.ToString()) item.MenuID = "-MO5_0E3pGAqYNsgvfWs";
            //    if (item.MenuID == 7.ToString()) item.MenuID = "-MO5_0VlqzT7EyaAN6Rl";
            //    if (item.MenuID == 8.ToString()) item.MenuID = "-MO5_0l9vIhDKFd6jPJl";
            //    if (item.MenuID == 9.ToString()) item.MenuID = "-MO5_110wXkvxTOEGaKH";
            //    if (item.MenuID == 10.ToString()) item.MenuID = "-MO5_1HtJ1HDTevZx1R5";
            //    if (item.MenuID == 11.ToString()) item.MenuID = "-MO5_1bRIDZiUswXcxFo";
            //    if (item.MenuID == 12.ToString()) item.MenuID = "-MO5_1sApfzvOXzAJnjk";
            //    if (item.MenuID == 13.ToString()) item.MenuID = "-MO5_27xZTHzHL18d5n9";
            //    if (item.MenuID == 14.ToString()) item.MenuID = "-MO5_2OEtO3IGKocmqoA";
            //    if (item.MenuID == 15.ToString()) item.MenuID = "-MO5_2dSpTjQHeX1kcmk";
            //    //Business
            //    if (item.BusinessTypeID == 1.ToString()) item.BusinessTypeID = "-MO5Cz_llCf2_rUub83G";
            //    if (item.BusinessTypeID == 2.ToString()) item.BusinessTypeID = "-MO5D8R5bYJi_ybkSrkK";
            //    if (item.BusinessTypeID == 3.ToString()) item.BusinessTypeID = "-MO5DAFH_xIWbwXghatD";
            //    if (item.BusinessTypeID == 4.ToString()) item.BusinessTypeID = "-MO5EMKryhVhmTApYtvU";
            //    if (item.BusinessTypeID == 5.ToString()) item.BusinessTypeID = "-MO5ENYkjevkEqQX3uJL";
            //    if (item.BusinessTypeID == 6.ToString()) item.BusinessTypeID = "-MO5EPD9QZAtsAWJslE4";
            //    if (item.BusinessTypeID == 7.ToString()) item.BusinessTypeID = "-MO5ER8JdEr-1LMrCJUw";
            //    if (item.BusinessTypeID == 8.ToString()) item.BusinessTypeID = "-MO5ESshKBVGpKne_E6B";
            //    if (item.BusinessTypeID == 9.ToString()) item.BusinessTypeID = "-MO5EUYyNlJW-nXyMtu2";
            //    if (item.BusinessTypeID == 10.ToString()) item.BusinessTypeID = "-MO5EZRXxibWz7IXEk4i";
            //    if (item.BusinessTypeID == 11.ToString()) item.BusinessTypeID = "-MO5EbRvQ9QFImlz6BAX";
            //    if (item.BusinessTypeID == 12.ToString()) item.BusinessTypeID = "-MO5EdbiBHQubApdBJvg";
            //    if (item.BusinessTypeID == 13.ToString()) item.BusinessTypeID = "-MO5EfsDHFK_K_m-YW-3";
            //    if (item.BusinessTypeID == 14.ToString()) item.BusinessTypeID = "-MO5Ei2cxyBSsBg10nN8";
            //    if (item.BusinessTypeID == 15.ToString()) item.BusinessTypeID = "-MO5Ek2X0XGknBcIm3bu";
            //    if (item.BusinessTypeID == 16.ToString()) item.BusinessTypeID = "-MO5Emdqsa_KEj1-uYTO";
            //    if (item.BusinessTypeID == 17.ToString()) item.BusinessTypeID = "-MO5Ep-axbR2167KfT9Z";
            //    //Province
            //    if (item.ProvinceID == 1.ToString()) item.ProvinceID = "-MO5bKqT2OC5v60we4Ma";
            //    if (item.ProvinceID == 2.ToString()) item.ProvinceID = "-MO5bL6kqqN40BPbblhr";
            //    if (item.ProvinceID == 3.ToString()) item.ProvinceID = "-MO5bLO7iu4Q0fkaLh_P";
            //    if (item.ProvinceID == 4.ToString()) item.ProvinceID = "-MO5bLeJGGn0jo3By9sd";
            //    if (item.ProvinceID == 5.ToString()) item.ProvinceID = "-MO5bLvmHyKToeWQh-iM";
            //    if (item.ProvinceID == 6.ToString()) item.ProvinceID = "-MO5bMBo04N4V9XHa6X4";
            //    if (item.ProvinceID == 7.ToString()) item.ProvinceID = "-MO5bMT09PJkT3hKXetu";
            //    if (item.ProvinceID == 8.ToString()) item.ProvinceID = "-MO5bMilti40JLa-EjBJ";
            //    if (item.ProvinceID == 9.ToString()) item.ProvinceID = "-MO5bN-8mhU7nO2O2JIq";
            //    if (item.ProvinceID == 10.ToString()) item.ProvinceID = "-MO5bNGDO2EOfYctMIMx";
            //    if (item.ProvinceID == 11.ToString()) item.ProvinceID = "-MO5bNXFjqLN4BYeQ-vx";
            //    if (item.ProvinceID == 12.ToString()) item.ProvinceID = "-MO5bNn2lFnFz8H7Abdl";
            //    if (item.ProvinceID == 13.ToString()) item.ProvinceID = "-MO5bO2WdoqndteuZQDj";
            //    if (item.ProvinceID == 14.ToString()) item.ProvinceID = "-MO5bOJwZq4fUNt69y6b";
            //    if (item.ProvinceID == 15.ToString()) item.ProvinceID = "-MO5bO_eLoGjhFZSIGWd";
            //    if (item.ProvinceID == 16.ToString()) item.ProvinceID = "-MO5bOrkCLyBQJRmW9F-";
            //    if (item.ProvinceID == 17.ToString()) item.ProvinceID = "-MO5bP767niqkvz76tOD";
            //    if (item.ProvinceID == 18.ToString()) item.ProvinceID = "-MO5bPNap5JRY4XO9ehH";
            //    if (item.ProvinceID == 19.ToString()) item.ProvinceID = "-MO5bPdCM3szTmTOoYzA";
            //    if (item.ProvinceID == 20.ToString()) item.ProvinceID = "-MO5bPttGfvjOE4PipCO";
            //    if (item.ProvinceID == 21.ToString()) item.ProvinceID = "-MO5bQDbCIsTFYbhvq2q";
            //    if (item.ProvinceID == 22.ToString()) item.ProvinceID = "-MO5bQUKu8F4qZgbiBIf";
            //    if (item.ProvinceID == 23.ToString()) item.ProvinceID = "-MO5bQuCss2dEThgVyK_";
            //    if (item.ProvinceID == 24.ToString()) item.ProvinceID = "-MO5bR9KYbejGRn8OFCn";
            //    if (item.ProvinceID == 25.ToString()) item.ProvinceID = "-MO5bRQ3XJ2e-K6ignRV";
            //    if (item.ProvinceID == 26.ToString()) item.ProvinceID = "-MO5bRfsitCR54GxXsbs";
            //    if (item.ProvinceID == 27.ToString()) item.ProvinceID = "-MO5bRynKnXBP7HVTcGy";
            //    if (item.ProvinceID == 28.ToString()) item.ProvinceID = "-MO5bSF_-Uvpg0v3a8Cw";
            //    if (item.ProvinceID == 29.ToString()) item.ProvinceID = "-MO5bSWLRw1aYLDJ8C2t";
            //    if (item.ProvinceID == 30.ToString()) item.ProvinceID = "-MO5bSmAs9euk6XPgTnn";
            //    if (item.ProvinceID == 31.ToString()) item.ProvinceID = "-MO5bT1hZQQzTqmrCkLd";
            //    if (item.ProvinceID == 32.ToString()) item.ProvinceID = "-MO5bTIKdcCcOSom1Ny-";
            //    if (item.ProvinceID == 33.ToString()) item.ProvinceID = "-MO5bTZKfBeFG3MpZzEu";
            //    if (item.ProvinceID == 34.ToString()) item.ProvinceID = "-MO5bTogI3NYY1ODzM9f";
            //    if (item.ProvinceID == 35.ToString()) item.ProvinceID = "-MO5bU3ymt74E4nLbTbe";
            //    if (item.ProvinceID == 36.ToString()) item.ProvinceID = "-MO5bUL8VEDdwOQ4C7vK";
            //    if (item.ProvinceID == 37.ToString()) item.ProvinceID = "-MO5bUaNASTMuNVvdXWc";
            //    if (item.ProvinceID == 38.ToString()) item.ProvinceID = "-MO5bUq-9IYk_0OrjfM8";
            //    if (item.ProvinceID == 39.ToString()) item.ProvinceID = "-MO5bV5lAkx1revx4uKC";
            //    if (item.ProvinceID == 40.ToString()) item.ProvinceID = "-MO5bVMeQ1PV5FM4N-Gv";
            //    if (item.ProvinceID == 41.ToString()) item.ProvinceID = "-MO5bVcrxxiCW79QxcYZ";
            //    if (item.ProvinceID == 42.ToString()) item.ProvinceID = "-MO5bVt1WRtfMC_ETWZz";
            //    if (item.ProvinceID == 43.ToString()) item.ProvinceID = "-MO5bW8XZFUMcD2rDDIV";
            //    if (item.ProvinceID == 44.ToString()) item.ProvinceID = "-MO5bWPQ7Z9WNYqMmfsO";
            //    if (item.ProvinceID == 45.ToString()) item.ProvinceID = "-MO5bWf5rm06GrIblKvv";
            //    if (item.ProvinceID == 46.ToString()) item.ProvinceID = "-MO5bWvPhBo9K81_IB_o";
            //    if (item.ProvinceID == 47.ToString()) item.ProvinceID = "-MO5bXAXjHFvjh4fhpN3";
            //    if (item.ProvinceID == 48.ToString()) item.ProvinceID = "-MO5bXQHQ0jfmO2XNOVB";
            //    if (item.ProvinceID == 49.ToString()) item.ProvinceID = "-MO5bXgOe1UNMRO6zCix";
            //    if (item.ProvinceID == 50.ToString()) item.ProvinceID = "-MO5bXyUW4lASxkm0dp1";
            //    if (item.ProvinceID == 51.ToString()) item.ProvinceID = "-MO5bYTZjpver6T932Ul";
            //    if (item.ProvinceID == 52.ToString()) item.ProvinceID = "-MO5bYjKyXsMN1U7aolQ";
            //    if (item.ProvinceID == 53.ToString()) item.ProvinceID = "-MO5bYzfuP4-ZD6hdKv8";
            //    if (item.ProvinceID == 54.ToString()) item.ProvinceID = "-MO5bZFa8ewyf9m9xIR0";
            //    if (item.ProvinceID == 55.ToString()) item.ProvinceID = "-MO5bZVz-m7IQlHwsGNV";
            //    if (item.ProvinceID == 56.ToString()) item.ProvinceID = "-MO5bZlay428hZ_LfvQS";
            //    if (item.ProvinceID == 57.ToString()) item.ProvinceID = "-MO5b_1K2_tF_C4GVDo3";
            //    if (item.ProvinceID == 58.ToString()) item.ProvinceID = "-MO5b_IdTM5ZRNtHps4q";
            //    if (item.ProvinceID == 59.ToString()) item.ProvinceID = "-MO5b_b9qxQ0OrGKZy1J";
            //    if (item.ProvinceID == 60.ToString()) item.ProvinceID = "-MO5b_s8L18GOjXeQvA8";
            //    if (item.ProvinceID == 61.ToString()) item.ProvinceID = "-MO5ba7Z3b9Cus9tkfVY";
            //    if (item.ProvinceID == 62.ToString()) item.ProvinceID = "-MO5baPAv6Zs9bqAK936";
            //    if (item.ProvinceID == 63.ToString()) item.ProvinceID = "-MO5baf507GHHTAP1JfP";

            //    AddToFireBase(item);
            //}
            return Ok(list);
        }
        [HttpGet("GetByID/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByID(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }
        [HttpGet("GetByIDProvince/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByIDProvince(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpGet("GetByIDBusinessType/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByIDBusinessType(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.BusinessTypeID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpPost("EditByID/{id:int}")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, string token, [FromBody] Store store){
            if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
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

        [HttpPost("CreateStore")]
        public IActionResult RegisterStore(string token, [FromBody] Store store){
            if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
            {
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

        //tim ra ID tự động bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
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
        private void AddToFireBase(Store store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            PushResponse response = client.Push("Store/", data);
            data.StoreID = response.Result.name;
            SetResponse setResponse = client.Set("Store/" + data.StoreID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, Store store){
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;
            SetResponse setResponse = client.Set("Store/" + data.StoreID, data);
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
        public string GetRole(string token){
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
                if (item.Email.ToString() == Decrypt(token))
                    return item.UserTypeID;
            }
            return "";
        }
    }
}
