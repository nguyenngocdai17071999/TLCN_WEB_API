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
    public class DishTypeController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private static string key = "TLCN";
        IFirebaseClient client;

        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("DishType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DishType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<DishType>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }
        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("DishType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DishType>();
            //danh sách tìm kiếm

            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<DishType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<DishType>();
            foreach (var item in list){
                if (item.DishType_ID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id,string token, [FromBody] DishType dishType){
            try{
                if(GetRole(token)== "-MO5VBnzdGsuypsTzHaV")
                {
                    AddbyidToFireBase(id, dishType);
                    return Ok(new[] { "sửa thành công" });
                }
                else{
                    return Ok("Bạn không có quyền");
                }
            }
            catch{
                return Ok(new[] { "Lỗi rồi" });
            }
        }

        [HttpPost("CreateDishType")]
        public IActionResult RegisterDishType(string token,[FromBody] DishType dishType){
            string err = "";
            try{
                if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
                {
                    AddToFireBase(dishType);
                    err = "Đăng ký thành công";
                }
                else{
                    err = "Bạn không có quyền";                 
                }
            }
            catch{
                err = "Lỗi rồi";
            }
            return Ok(new[] { err });
        }

        //tim ra ID tự động bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("DishType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<DishType>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<DishType>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1){
                int dem = 0;
                foreach (var item in list){
                    if (item.DishType_ID == i.ToString())
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }

        // thêm dư liệu lên firebase
        private void AddToFireBase(DishType dishType){
            client = new FireSharp.FirebaseClient(config);
            var data = dishType;
            PushResponse response = client.Push("DishType/", data);
            data.DishType_ID = response.Result.name;
            SetResponse setResponse = client.Set("DishType/" + data.DishType_ID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, DishType dishType){
            client = new FireSharp.FirebaseClient(config);
            var data = dishType;
            data.DishType_ID = id;
            SetResponse setResponse = client.Set("DishType/" + data.DishType_ID, data);
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

        public string GetIDToken(string token){
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
                    return item.UserID;
            }
            return "";
        }

        public static string Decrypt(string toDecrypt){
            try{
                bool useHashing = true;
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                if (useHashing)
                {
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
    }
}
