﻿using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string UserTypeID { get; set; }
        public string Status { get; set; }
        private static string key = "TLCN";
        string columnName = "User";
        public string nameEmailSend = "nguyenngocdai17071999@gmail.com";
        public string passEmailSend =  "conyeume";
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        public IConfiguration _config;
        public User(IConfiguration config)
        {
            _config = config;
        }

        public User()
        {
        }

        public List<User> getAll(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
            return list;
        }
        public List<User> getByID(string id, string email)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            if (id != null)
            {
                foreach (var item in list)
                {
                    if (item.UserID.ToString() == id)
                        list2.Add(item);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    if (item.Email.ToString() == email)
                        list2.Add(item);
                }
            }
            return list2;
        }  
        public List<UserCommentInfo> GetByIDnottoken(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            List<User> list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            List<User> list2 = new List<User>();

            foreach (var item in list)
            {
                if (item.UserID.ToString() == id)
                    list2.Add(item);
            }

            List<UserCommentInfo> list3 = new List<UserCommentInfo>();
            foreach (var item in list2)
            {
                UserCommentInfo a = new UserCommentInfo();
                a.Email = item.Email;
                a.Picture = item.Picture;
                a.UserName = item.UserName;
                list3.Add(a);
            }
            return list3;
        }
        public void editByID(string id,string email, User user) {
            if (id != null)
            {
                AddbyidToFireBase(id, user);
            }
            else
            {
                AddbyidToFireBase(GetIDToken(email), user);
            }
        }
        public void blockAccount(string id, string status)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
            foreach (var item in list2)
            {
                item.Status = status;
                AddbyidToFireBase(item.UserID, item);
            }
        }

        public void resetPass(string email, string Password) {
            User resetPassUser = new User();
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list)
            {
                if (item.Email == email)
                {
                    resetPassUser = item;
                    break;
                }
            }
            resetPassUser.Password = Password;
            EditPassBYID(resetPassUser.UserID, resetPassUser);
        }

        public string getRole(string Email)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
        public string GetIDToken(string Email)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, User user)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
                if (item.UserID.ToString() == id)
                    list2.Add(item);
            }

            foreach (var item in list2)
            {
                if (user.UserID == null) user.UserID = item.UserID;
                if (user.UserName == null) user.UserName = item.UserName;
                if (user.Phone == null) user.Phone = item.Phone;
                if (user.Address == null) user.Address = item.Address;
                if (user.Password == null) user.Password = item.Password;
                if (user.Email == null) user.Email = item.Email;
                if (user.Picture == null) user.Picture = item.Picture;
                if (user.Sex == null) user.Sex = item.Sex;
                if (user.Birthday == null) user.Birthday = item.Birthday;
                if (user.UserTypeID == null) user.UserTypeID = item.UserTypeID;
            }

            client = new FireSharp.FirebaseClient(config);
            var data2 = user;
            data2.UserID = id;
            //data2.Password = Encrypt(data2.Password);
            SetResponse setResponse = client.Set("User/" + data2.UserID, data2);
        }

        
        public bool checkAdmin(string email) {
            if (getRole(email) == "-MO5VBnzdGsuypsTzHaV") return true;
            return false;
        }
        public bool checkUser(string email)
        {
            if (getRole(email) == "-MO5VYNnWIjXtvJO4AXi") return true;
            return false;
        }
        public bool checkOwner(string email)
        {
            if (getRole(email) == "-MO5VWchsca2XwktyNAw") return true;
            return false;
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
        public void AddToFireBase(User user)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            PushResponse response = client.Push("User/", data);
            data.UserID = response.Result.name;
            data.Password = Encrypt(data.Password);
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


        public bool kiemtraEmail(string email)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list)
            {
                if (item.Email == email)
                    return true;
            }
            return false;
        }
        public void Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            var data2 = new User();
            //data2.UserID = id;
            //data2.Password = Encrypt(data2.Password);
            SetResponse setResponse = client.Set("User/" + id, data2);
        }
        // Edit password by id
        public void EditPassBYID(string id, User user)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            //  PushResponse response = client.Push("User/", data);
            data.UserID = id;
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }




        public UserModel AuthenticationUser(UserModel login)
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
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                }
            }
            return user;
        }


        //thuc hien tao token
        public string GenerateJSONWebToken(UserModel userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AshProgHelpSecretKey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "ashproghelp.com",
                audience: "ashproghelp.com",
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }
    }


}
