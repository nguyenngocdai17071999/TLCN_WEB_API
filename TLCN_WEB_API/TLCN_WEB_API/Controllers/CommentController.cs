using System;
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
   // [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private static string key = "TLCN";
        private IConfiguration _config;
        public CommentController(IConfiguration config)
        {
            _config = config;
        }
        IFirebaseClient client;

        [HttpGet("GetAll")]

        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Comment");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Comment>();
                //danh sách tìm kiếm
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
                }
                return Ok(list);
            }
            catch
            {
                return Ok("Error");
            }

        }
        [HttpGet("GetByID")]
        // phương thức get by id store dữ liệu từ firebase 
        public IActionResult GetByID(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Comment");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Comment>();
                //danh sách tìm kiếm

                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
                }
                var list2 = new List<Comment>();
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

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Comment comment)
        {

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    AddbyidToFireBase(id, comment);
                    return Ok(new[] { "sửa thành công" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });

            }
            catch
            {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult deleteByID(string idcomment, string idusercomment, string idStore)
        {

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if(GetIDUser(Email)==idusercomment|| GetIDUser(Email) ==idchuquan(idStore))
                    {
                        Delete(idcomment);
                        return Ok(new[] { "Xóa thành công" });
                    }
                    return Ok(new[] { "Bạn không được xóa" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });

            }
            catch
            {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateComment")]
        public IActionResult RegisterComment([FromBody] Comment comment)
        {
            string err = "";
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    string Idcomment = AddToFireBase(comment);
                    err = Idcomment;
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
                
            }
            catch
            {
                err = "Error";
            }
            return Ok(new[] { err });

        }



        
        // thêm dư liệu lên firebase
        private string AddToFireBase(Comment comment)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            PushResponse response = client.Push("Comment/", data);

            data.CommentID = response.Result.name;

            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
            return data.CommentID;
        }

        public string GetRole(string Email)
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
                if (item.Email.ToString() == Email)
                    return item.UserTypeID;
            }
            return "";
        }

        public string GetIDUser(string Email)
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
                if (item.Email.ToString() == Email)
                    return item.UserID;
            }
            return "";
        }

        public string idchuquan(string idstore)
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
                if (item.StoreID.ToString() == idstore)
                    return item.UserID;
            }
            return "";
        }

        private UserModel AuthenticationUser(UserModel login)
        {
            //get list user
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";

            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            //layas thongo tin taif khoan dang nhap
            UserModel user = null;
            foreach (var item in list)
            {
                if (item.Email == login.EmailAddress && item.Password == Encrypt(login.PassWord))
                {
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password) };
                }
            }
            return user;
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

        //thuc hien tao token
        private string GenerateJSONWebToken(UserModel userinfo)
        {
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

        public bool kiemtrathoigianlogin(DateTime date)
        {
            int sophut1 = date.Minute;
            int sophut2 = DateTime.Now.Minute;
            if (sophut2 < sophut1)
                sophut2 = sophut2 + 60;
            int ketqua = sophut2 - sophut1;
            if (ketqua < 30) return true;
            return false;

        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, Comment comment)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
        }

        private void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new Comment();
           // data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + id, data);
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
