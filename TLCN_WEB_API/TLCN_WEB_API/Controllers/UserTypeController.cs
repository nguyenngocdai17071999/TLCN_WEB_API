using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using GoogleMaps.LocationServices;
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
    public class UserTypeController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        private IConfiguration _config;
        public UserTypeController(IConfiguration config)
        {
            _config = config;
        }

        private static string key = "TLCN";
        IFirebaseClient client;

        [Authorize]
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll() {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;

                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {

                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV")
                    {
                        client = new FireSharp.FirebaseClient(config);
                        FirebaseResponse response = client.Get("UserType");
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                        var list = new List<UserType>();
                        //danh sách tìm kiếm
                        //var list2 = new List<User>();
                        foreach (var item in data)
                        {
                            list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
                        }
                        return Ok(list);
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch {
                return Ok("Error");
            } 
        }      

        [Authorize]
        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id) {
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV"){
                        client = new FireSharp.FirebaseClient(config);
                        FirebaseResponse response = client.Get("UserType");
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                        var list = new List<UserType>();
                        //danh sách tìm kiếm
                        foreach (var item in data){

                            list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
                        }
                        var list2 = new List<UserType>();
                        foreach (var item in list){
                            if (item.UserTypeID == id)
                                list2.Add(item);
                        }
                        return Ok(list2);
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }            
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] UserType usertype){
            try {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;

                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV")
                    {
                        AddbyidToFireBase(id, usertype);
                        return Ok(new[] { "ok" });
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });                
            }
            catch {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateUserType")]
        public IActionResult RegisterUser([FromBody] UserType userType){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;

                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV")
                    {
                        AddToFireBase(userType);
                        err = "Đăng ký thành công";
                    }
                    else
                    {
                        err = "Bạn Không có quyền";
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });    
            }
            catch{
                err = "Error";
            }
            return Ok(new[] { err });
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, UserType usertype){
            client = new FireSharp.FirebaseClient(config);
            var data = usertype;
            //PushResponse response = client.Push("UserType/", data);
            data.UserTypeID = id;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }

        // thêm dư liệu lên firebase
        private void AddToFireBase(UserType userType){
            client = new FireSharp.FirebaseClient(config);
            var data = userType;
            PushResponse response = client.Push("UserType/", data);
            data.UserTypeID = response.Result.name;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }


        //tim ra ID tự động của user bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("UserType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1){
                int dem = 0;
                foreach (var item in list){
                    if (item.UserTypeID == i.ToString())
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
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
        public string GetIDToken(string Email){
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
                    return item.UserID;
            }
            return "";
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

    }
}
