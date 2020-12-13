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
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLCN_WEB_API.Models;


namespace TLCN_WEB_API.Controllers
{
  //  [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        private IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
        }
        private static string key = "TLCN";
        IFirebaseClient client;


        [Authorize]
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            string Email = claim[1].Value;
            if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true) {
                if (GetRole(Email) == "-MO5VBnzdGsuypsTzHaV")
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
                    foreach (var item in list)
                    {
                        item.Password = Decrypt(item.Password);
                    }
                    return Ok(list);
                }
                return Ok(new[] { "Bạn không có quyền" });
            }
            else return Ok(new[] { "Bạn cần đăng nhập" });
        }

        [Authorize]
        [HttpGet("GetRole")]
        //phương thức get dữ liệu từ firebase
        public IActionResult getRole(){

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            string Email = claim[1].Value;
            if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
            {
                Role a = new Role();
                a.UserTyleID = GetRole(Email);
                return Ok(a);
            }
            else return Ok(new[] { "Bạn cần đăng nhập" });
        }

        [Authorize]
        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(){

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            string Email = claim[1].Value;

            if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
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
                        list2.Add(item);
                }
                foreach (var item in list2)
                {
                    item.Password = Decrypt(item.Password);
                }
                return Ok(list2);
            }
            else return Ok(new[] { "Bạn cần đăng nhập" });
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID( [FromBody] User user){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;

                if (kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    AddbyidToFireBase(GetIDToken(Email), user);
                    return Ok(new[] { "Sửa thành công" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Lỗi rồi" });
            }
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user){
            string err = "";
            try{
                if (kiemtraEmail(user.Email) == false){
                    AddToFireBase(user);
                    err = "Đăng ký thành công";
                }
                else{
                    err = "Email đã tồn tại";
                }
            }
            catch{
                err = "Lỗi rồi";
            }
            return Ok(new[] { err });
        }
        //Hàm login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login userlogin){           
            UserModel login = new UserModel();
            login.EmailAddress = userlogin.Email;
            login.PassWord = userlogin.PassWord;
            IActionResult response = Unauthorized();

            var user = AuthenticationUser(login);
            if (user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });

            }
            return response;
        }

        [HttpPost("ForgetPass")]
        public IActionResult ForgetPass(string Email){///sử dụng thuộc tính Email của models user json chỉ cần Email
            if (kiemtraEmail(Email) == true){
                ForGetCode dai = new ForGetCode();
                Random a = new Random();
                int code = a.Next(100000, 999999);
                DateTime date = DateTime.Now;

                ////Gửi email
                var messenge = new MimeMessage();
                messenge.From.Add(new MailboxAddress("Test Project", "nguyenngocdai17071999@gmail.com"));
                messenge.To.Add(new MailboxAddress("naren", Email));
                messenge.Subject = "hello";
                messenge.Body = new TextPart("plain"){
                    Text = "Code ResetPass cua ban la: " + code + ""
                };

                using (var client = new SmtpClient()){
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("nguyenngocdai17071999@gmail.com", "conyeume");
                    client.Send(messenge);
                    client.Disconnect(true);
                }
                dai.code = code;
                dai.date = date;
                return Ok(dai);
            }
            return Ok(new[] { "Không có Email" });
        }

        //resetpass theo gmail
        [HttpPost("ResetPass")]
        public IActionResult ResetPass(string Email,string Password){//Sử dụng thuốc tính Email và Password
            if (kiemtraEmail(Email) == true){
                User resetPassUser = new User();
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("User");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<User>();
                string err = "";
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list){
                    if (item.Email == Email){
                        resetPassUser = item;
                        break;
                    }
                }
                resetPassUser.Password = Password;
                EditPassBYID(resetPassUser.UserID, resetPassUser);
                return Ok(new[] { "Đổi mật khẩu thành công" });
            }
            return Ok(new[] { "Không có Email" });
        }

        //tim ra ID tự động của user bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1){
                int dem = 0;
                foreach (var item in list){
                    if (item.UserID == i.ToString())
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            PushResponse response = client.Push("User/", data);
            data.UserID = response.Result.name;
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }
        // Edit password by id
        private void EditPassBYID(string id, User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            //  PushResponse response = client.Push("User/", data);
            data.UserID = id;
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, User user){
            if (id == user.UserID) {
                client = new FireSharp.FirebaseClient(config);
                var data = user;
                data.UserID = id;
                data.Password = Encrypt(data.Password);
                SetResponse setResponse = client.Set("User/" + data.UserID, data);
            }
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
        private bool kiemtraEmail(string email){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list){
                if (item.Email == email)
                    return true;
            }
            return false;
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
            foreach(var item in list)
            {
                if (item.Email == login.EmailAddress && item.Password == Encrypt(login.PassWord))
                {
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password) };
                }
            }
            return user;
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

        public bool kiemtrathoigianlogin(DateTime date) {
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
