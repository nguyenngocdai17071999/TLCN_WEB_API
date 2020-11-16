using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCLN_WEB_API.Models;

namespace TCLN_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        private static string key = "TLCN";
        IFirebaseClient client;

        [HttpGet("GetAll")]

        //phương thức get dữ liệu từ firebase
        public IActionResult getAll()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            //var list2 = new List<User>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }

        [HttpGet("GetByID/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> getbyid(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            foreach (var item in list)
            {
                if (item.UserID == id)
                    list2.Add(item);
            }

            return Ok(list2);
        }

        [HttpPost("EditByID/{id:int}")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult Edit(int id, [FromBody] User user)
        {

            try
            {
                AddbyidToFireBase(id, user);
                return Ok(new[] { "ok" });
            }
            catch
            {
                return Ok(new[] { "not ok" });
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult Create([FromBody] User user)
        {

            try
            {
                AddToFireBase(user);
                return Ok(new[] { "ok" });
            }
            catch
            {
                return Ok(new[] { "not ok" });
            }


        }
        //tim ra ID tự động của user bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            foreach (var item in list)
            {
                while (item.UserID == i)
                {
                    i++;
                }
            }
            return i;
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(User user)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            //  PushResponse response = client.Push("User/", data);
            data.UserID = GetID();
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }
        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(int id, User user)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            //  PushResponse response = client.Push("User/", data);
            data.UserID = id;
            // data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }
        // mã hóa dữ liệu MD5
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

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

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}
