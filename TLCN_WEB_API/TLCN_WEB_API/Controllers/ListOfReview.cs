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
    [Route("api/[controller]")]
    [ApiController]
    public class ListOfReview : ControllerBase
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
        public IActionResult GetAll()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("listOfReviews");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<listOfReviews>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<listOfReviews>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }

        [HttpGet("GetByID")]
        // phương thức get by id store dữ liệu từ firebase 
        public IActionResult GetByID(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("listOfReviews");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<listOfReviews>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<listOfReviews>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<listOfReviews>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, string token, [FromBody] listOfReviews list)
        {
            try
            {
                if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
                {
                    AddbyidToFireBase(id, list);
                    return Ok(new[] { "sửa thành công" });
                }
                else
                {
                    return Ok(new[] { "Bạn Không có quyền" });
                }
            }
            catch
            {
                return Ok(new[] { "Lỗi rồi" });
            }
        
        }
        [HttpPost("CreateListOfReviews")]
        public IActionResult RegisterBusinessType(string token, [FromBody] listOfReviews list)
        {
            string err = "";
            try
            {
                if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
                {
                    AddToFireBase(list);
                    err = "Đăng ký thành công";
                }
                else
                {
                    err = "Bạn Không có quyền";
                }
            }
            catch
            {
                err = "Lỗi rồi";
            }
            return Ok(new[] { err });
        }

        // thêm dư liệu lên firebase
        private void AddToFireBase(listOfReviews list)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = list;
            PushResponse response = client.Push("listOfReviews/", data);
            data.ReViewID = response.Result.name;
            SetResponse setResponse = client.Set("listOfReviews/" + data.ReViewID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, listOfReviews list)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = list;
            PushResponse response = client.Push("listOfReviews/", data);
            data.ReViewID = id;
            SetResponse setResponse = client.Set("listOfReviews/" + data.ReViewID, data);
        }

        public string GetRole(string token)
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
                if (item.Email.ToString() == Decrypt(token))
                    return item.UserTypeID;
            }
            return "";
        }

        public string GetIDToken(string token)
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
                if (item.Email.ToString() == Decrypt(token))
                    return item.UserID;
            }
            return "";
        }

        public static string Decrypt(string toDecrypt)
        {
            try
            {
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
            catch
            {
                return "Loi roi";
            }
        }
    }



}
