using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
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
using System.Net;

namespace TLCN_WEB_API.Models
{
    public class User{
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
        public string idFacebook { get; set; }
        public string idGoogle { get; set; }
        public string numberVerify { get; set; }
        public string dateCodeVerify { get; set; }
        public string DateLogin { get; set; }


        private static string key = "TLCN";
        string columnName = "User";

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        //public IConfiguration _config;
        //public User(IConfiguration config)
        //{
        //    _config = config;
        //}

        public User(){
            UserID = "";
            UserName = "";
            Phone = "";
            Address = "";
            Password = "";
            Email = "";
            Picture = "";
            Sex = "";
            Birthday = "";
            UserTypeID = "";
            Status = "";
            idFacebook = "";
            idGoogle = "";
            numberVerify = "";
            dateCodeVerify = "";
            DateLogin = "";
        }

        public List<User> getAll(){                                            //Lấy danh sách tài khoản 
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public bool kiemtracononline(string datetimelogin) {
            TimeSpan Time = DateTime.Now - DateTime.Parse(datetimelogin);
            if (Time.Days < 1  && Time.Hours < 1) return true;
            return false;
        }

        public int ThongKeNguoiOnlie(){                                      //thống kê số người đang online 
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            int dem = 0;
            foreach (var item in list){
                if (item.DateLogin != "" && item.DateLogin != null)
                    if (kiemtracononline(item.DateLogin) == true)
                        dem++;
            }
            return dem;
        }

        public void logout(string IDUser) {
            var danhsachUser = getAll();
            foreach(var item in danhsachUser) {
                if (item.UserID == IDUser) {
                    item.DateLogin = "";
                    item.AddbyidToFireBase(item.UserID, item);
                }
                    
            }
        }

        public List<User> getByID(string id, string email){                   //xem thông tin chi tiết tài khoản IDUser, Email
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            if (id != null){
                foreach (var item in list){
                    if (item.UserID.ToString() == id)
                        list2.Add(item);
                }
            }
            else{
                foreach (var item in list){
                    if (item.Email.ToString() == email)
                        list2.Add(item);
                }
            }
            return list2;
        }  

        //public List<UserCommentInfo> GetByIDnottoken(string id)
        //{
        //    client = new FireSharp.FirebaseClient(config);
        //    FirebaseResponse response = client.Get(columnName);
        //    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
        //    List<User> list = new List<User>();
        //    //danh sách tìm kiếm
        //    foreach (var item in data)
        //    {
        //        list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
        //    }
        //    List<User> list2 = new List<User>();

        //    foreach (var item in list)
        //    {
        //        if (item.UserID.ToString() == id)
        //            list2.Add(item);
        //    }

        //    List<UserCommentInfo> list3 = new List<UserCommentInfo>();
        //    foreach (var item in list2)
        //    {
        //        UserCommentInfo a = new UserCommentInfo();
        //        a.Email = item.Email;
        //        a.Picture = item.Picture;
        //        a.UserName = item.UserName;
        //        list3.Add(a);
        //    }
        //    return list3;
        //}

        public void editByID(string id,string email, User user) { //Update data
            if (id != null){
                AddbyidToFireBase(id, user);
            }
            else{
                AddbyidToFireBase(GetIDToken(email), user);
            }
        }
        public void blockAccount(string id, string status){                        //thay đổi status của tài khoản
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();
            foreach (var item in list){
                if (item.UserID == id)                                    //tìm tài khoản
                    list2.Add(item);
            }
            foreach (var item in list2){
                item.Status = status;                                    //cập nhật lại Status
                AddbyidToFireBase(item.UserID, item);
            }
        }

        public void resetPass(string email, string Password) {                   //thay đổi mật khẩu tài khoản
            User resetPassUser = new User();
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list){
                if (item.Email == email){                                  //cập nhật lại mật khẩu
                    resetPassUser = item;
                    break;
                }
            }
            resetPassUser.Password = Password;
            EditPassBYID(resetPassUser.UserID, resetPassUser);              //cập nhật lại tài khoản
        }

        public string getRole(string Email){                                            //truy xuất quyền của tài khoản
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
        public string GetIDToken(string Email){                    //Lấy thông tin tài khoản dựa vào token
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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

        //Update dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, User user){                  
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<User>();

            foreach (var item in list){
                if (item.UserID.ToString() == id)                                     //tìm kiếm tài khoản user
                    list2.Add(item);
            }

            foreach (var item in list2){
                if (user.UserID == null) user.UserID = item.UserID;                           //cập nhật các thông tin khác của user
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

            client = new FireSharp.FirebaseClient(config);                                //lưu lại data
            var data2 = user;
            data2.UserID = id;
            //data2.Password = Encrypt(data2.Password);
            SetResponse setResponse = client.Set("User/" + data2.UserID, data2);
        }

        
        public bool checkAdmin(string email) {                                //kiểm tra có phải admin không
            if (getRole(email) == "-MO5VBnzdGsuypsTzHaV") return true;
            return false;
        }
        public bool checkUser(string email){
            if (getRole(email) == "-MO5VYNnWIjXtvJO4AXi") return true;        //kiểm tra có phải user không
            return false;
        }
        public bool checkOwner(string email){
            if (getRole(email) == "-MO5VWchsca2XwktyNAw") return true;       //kiểm tra có phải owner không
            return false;
        }
        public bool kiemtrathoigianlogin(DateTime date){                    //kiểm tra thời gian đăng nhập
            TimeSpan Time = DateTime.Now - date;
            if (Time.Days > 1) return false;
            if (Time.Minutes > 20) return false;
            return true;
        }

        public bool kiemtrathoigianforget(DateTime date){                     //kiểm tra thời gian đổi mật khẩu chỉ có hiệu lực trong 5p
            TimeSpan Time = DateTime.Now - date;
            if (Time.Days != 0) return false;
            if (Time.Minutes > 5) return false;
            return true;
        }

        public static string Decrypt(string toDecrypt){
            try{
                bool useHashing = true;                 //biến sử dụng băm
                byte[] keyArray;                        //mãng key
                //Chuyển đổi chuỗi được chỉ định, mã hóa dữ liệu nhị phân dưới dạng cơ số 64 chữ số, thành một mảng số nguyên không dấu 8 bit tương đương.
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);            //mãng để mã hóa
               
                if (useHashing){         //xác nhận băm để khởi tạo
                    //Tính toán giá trị băm MD5 cho dữ liệu đầu vào bằng cách sử dụng triển khai do nhà cung cấp dịch vụ mật mã (CSP) cung cấp. 
                    //Lớp này không thể được thừa kế.
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    //Tính toán giá trị băm cho dữ liệu đầu vào
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                }
                else                    
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                //Xác định đối tượng trình bao bọc để truy cập phiên bản nhà cung cấp dịch vụ mật mã (CSP) của thuật toán TripleDES. Lớp này không thể được thừa kế.
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;           //Chỉ định chế độ mật mã khối để sử dụng cho mã hóa.
                //The Electronic Codebook (ECB)Bất kỳ khối văn bản thuần túy nào giống hệt nhau và trong cùng một thông điệp, 
                //hoặc trong một thông điệp khác được mã hóa bằng cùng một khóa, sẽ được chuyển đổi thành các khối văn bản mật mã giống hệt nhau
                //Chỉ định loại đệm sẽ áp dụng khi khối dữ liệu thông báo ngắn hơn số byte đầy đủ cần thiết cho một hoạt động mật mã
                //Chuỗi đệm PKCS # 7 bao gồm một chuỗi các byte, mỗi byte bằng tổng số byte đệm được thêm vào.
                tdes.Padding = PaddingMode.PKCS7;

                //Tạo một đối tượng giải mã đối xứng.
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                //Biến đổi vùng được chỉ định của mảng byte được chỉ định. từ vị trí 0  đến cuối
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch{
                return "Loi roi";
            }
        }

        // mã hóa dữ liệu MD5
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;              //biến sử dụng băm
            byte[] keyArray;                     //mãng key
            //trả về mảng byte của chuỗi
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing){        //xác nhận băm để khởi tạo
                //Tính toán giá trị băm MD5 cho dữ liệu đầu vào bằng cách sử dụng triển khai do nhà cung cấp dịch vụ mật mã (CSP) cung cấp. 
                //Lớp này không thể được thừa kế.
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                //Tính toán giá trị băm cho dữ liệu đầu vào
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            //Xác định đối tượng trình bao bọc để truy cập phiên bản nhà cung cấp dịch vụ mật mã (CSP) của thuật toán TripleDES. Lớp này không thể được thừa kế.
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;            //Chỉ định chế độ mật mã khối để sử dụng cho mã hóa.
            //The Electronic Codebook (ECB)Bất kỳ khối văn bản thuần túy nào giống hệt nhau và trong cùng một thông điệp, 
            //hoặc trong một thông điệp khác được mã hóa bằng cùng một khóa, sẽ được chuyển đổi thành các khối văn bản mật mã giống hệt nhau
            //Chỉ định loại đệm sẽ áp dụng khi khối dữ liệu thông báo ngắn hơn số byte đầy đủ cần thiết cho một hoạt động mật mã
            //Chuỗi đệm PKCS # 7 bao gồm một chuỗi các byte, mỗi byte bằng tổng số byte đệm được thêm vào.
            tdes.Padding = PaddingMode.PKCS7;
            //Tạo một đối tượng giải mã đối xứng.
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //Biến đổi vùng được chỉ định của mảng byte được chỉ định. từ vị trí 0  đến cuối
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        //tạo mới dữ liệu
        public void AddToFireBase(User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            PushResponse response = client.Push("User/", data);
            data.UserID = response.Result.name;
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }
        //cập nhật dữ liệu
        public string AddToFireBaseReturn(User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            PushResponse response = client.Push("User/", data);
            data.UserID = response.Result.name;
            data.Password = Encrypt(data.Password);
            data.UserTypeID = "-MO5VWchsca2XwktyNAw";
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
            return data.UserID;
        }
        //xóa dữ liệu
        public void UpdateToFireBase(User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }

        //kiểm tra dữ liệu Email có trùng không
        public bool kiemtraEmail(string email){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            if (data != null){
                foreach (var item in data){
                    list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list){
                    if (item.Email == email)
                        return true;
                }
            }
            return false;
        }

        public bool kiemtraCode(int code, string email){                      //kiểm tra code nhập vào có đúng với code đã lưu không
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list){
                if (item.Email == email)
                    if (item.numberVerify == code.ToString())
                        return true;
            }
            return false;
        }

        public bool kiemtraDateCode(string email){                        //kiểm tra xem ngày gửi code với thời lúc nhập code để resetpass có lâu không 
            client = new FireSharp.FirebaseClient(config);                 //CHỉ hiệu lực trong 5p
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<User>();
            string err = "";
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list){
                if (item.Email == email)
                    if (kiemtraDateCode(item.dateCodeVerify)==true)
                        return true;
            }
            return false;
        }
        //Xóa tài khoản
        public void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data2 = new User();
            //data2.UserID = id;
            //data2.Password = Encrypt(data2.Password);
            SetResponse setResponse = client.Set("User/" + id, data2);
        }

        // Edit password by id
        public void EditPassBYID(string id, User user){
            client = new FireSharp.FirebaseClient(config);
            var data = user;
            //  PushResponse response = client.Push("User/", data);
            data.UserID = id;
            data.Password = Encrypt(data.Password);
            SetResponse setResponse = client.Set("User/" + data.UserID, data);
        }

        //kiểm tra thông tin đăng nhập có đúng không
        public UserModel AuthenticationUser(UserModel login){
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
                if (item.Email == login.EmailAddress && item.Password == Encrypt(login.PassWord)&&item.Status!="2"){
                    item.DateLogin = DateTime.Now.ToString(); //lưu lại ngày đăng nhập
                    item.AddbyidToFireBase(item.UserID, item);
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                }
            }
            return user;
        }

        //kiểm tra thông tin đăng nhập có đúng không khi dùng facebook
        public UserModel AuthenticationUserFaceBook(UserModel login){
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
            foreach (var item in list)                                    
            {
                if (item.Email == login.EmailAddress && item.idFacebook == login.idFacebook){
                    item.DateLogin = DateTime.Now.ToString();                                //lưu lại ngày đăng nhập
                    item.AddbyidToFireBase(item.UserID, item);
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                }
            }
            if(user==null){                                       //nếu trong dữ liệu chưa có tài khoản nào với Email sẽ tạo tài khoản mới            
                foreach (var item in list){
                    if (item.Email == login.EmailAddress){
                        user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                        item.idFacebook = login.idFacebook;
                        UpdateToFireBase(item);            //lưu lại tài khoản 
                    }
                }                
            }                
            return user;
        }

        public UserModel AuthenticationUserGoogle(UserModel login){
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
                if (item.Email == login.EmailAddress && item.idGoogle == login.idGoogle){
                    item.DateLogin = DateTime.Now.ToString();                                 //lưu lại ngày đăng nhập
                    item.AddbyidToFireBase(item.UserID, item);
                    user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                }
            }

            if (user == null){                                   //Nếu tài khoản google chưa được đăng ký trước đó sẻ tạo mới tài khoản
                foreach (var item in list){
                    if (item.Email == login.EmailAddress){
                        user = new UserModel { UserName = item.UserName, EmailAddress = item.Email, PassWord = Decrypt(item.Password), Status = item.Status };
                        item.idGoogle = login.idGoogle;
                        UpdateToFireBase(item);
                    }
                }
            }
            return user;
        }

        //thuc hien tao token
        public string GenerateJSONWebToken(UserModel userinfo){
            //Biểu diễn lớp cơ sở trừu tượng cho tất cả các khóa được tạo bằng thuật toán đối xứng.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AshProgHelpSecretKey"));       //khóa bảo mật
            //   Đại diện cho khóa mật mã và các thuật toán bảo mật được sử dụng để tạo chữ ký điện tử.
            //Chỉ định một URI trỏ đến thuật toán mã hóa HMAC 256-bit để ký kỹ thuật số XML
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);             //Chứng chỉ cấp
            //thành phần token
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,DateTime.Now.ToString()),      //ngày tọa token
                new Claim(JwtRegisteredClaimNames.Email,userinfo.EmailAddress),      //email token
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())     //Trả về biểu diễn chuỗi giá trị của phiên bản này của cấu trúc Guid.
            };

            //SecurityToken được thiết kế để đại diện cho Mã thông báo web JSON(JWT).
            var token = new JwtSecurityToken(
                issuer: "ashproghelp.com",                        //nhà phát hành
                audience: "ashproghelp.com",                      //khán giả
                claims,                                           //các giá trị trong token
                expires: DateTime.Now.AddMinutes(60),             //thời gian token hoạt động
                signingCredentials: credentials);                 //thông tin xác thực
            //tạo token
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        //public void sendMail(string Email, string text){
        //    ////Gửi email
        //    var messenge = new MimeMessage();
        //    messenge.From.Add(new MailboxAddress("Test Project", "nguyenngocdai17071999@gmail.com"));      //gmail gửi 
        //    messenge.To.Add(new MailboxAddress("naren", Email));
        //    messenge.Subject = "Xac Nhan";
        //    messenge.Body = new TextPart("plain"){            //nội dung gửi            
        //        Text = "Quán " + text + " đã được xác nhận"
        //    };

        //    using (var client = new SmtpClient()){                                      //kết nối gmail để đăng nhập gửi            
        //        client.Connect("smtp.gmail.com", 587, false);
        //        client.Authenticate("nguyenngocdai17071999@gmail.com", "conyeumenhieulam1");
        //        client.Send(messenge);
        //        client.Disconnect(true);
        //    }
        //}

        public string GetEmailByID(string ID){           //Lấy thông tin Email của tài khoản truyền vào IDUser
            var danhsach = getAll();
            foreach(var item in danhsach){
                if (item.UserID == ID)
                    return item.Email;
            }
            return "";
        }

        //public void updateCodeForget(string email){
        //    User infoUser = new User();
        //    UserModel user = null;
        //    ForGetCode dai = new ForGetCode();
        //    Random a = new Random();
        //    int code = a.Next(100000, 999999);             //random code
        //    DateTime date = DateTime.Now;                  //thời gian tạo code

        //    //Gửi email
        //    //var messenge = new MimeMessage();
        //    //messenge.From.Add(new MailboxAddress("Test Project", "nguyenductinh1973@gmail.com"));
        //    //messenge.To.Add(new MailboxAddress("naren", Email));
        //    //messenge.Subject = "Xac nhan";
        //    //messenge.Body = new TextPart("plain")
        //    //{
        //    //    Text = "Code ResetPass cua ban la: " + code + ""
        //    //};

        //    //using (var client = new SmtpClient())
        //    //{
        //    //    client.Connect("smtp.gmail.com", 587, false);
        //    //    client.Authenticate("nguyenductinh1973@gmail.com", "21061973");
        //    //    client.Send(messenge);
        //    //    client.Disconnect(true);
        //    //}

        //    dai.code = code;
        //    dai.date = date;
        //    // get list user
        //    client = new FireSharp.FirebaseClient(config);
        //    FirebaseResponse response = client.Get("User");
        //    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
        //    var list = new List<User>();
        //    string err = "";

        //    foreach (var item in data){
        //        list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
        //    }
        //    foreach (var item in list){                              //cập nhật lại thông tin để xác nhận code và ngày tạo code
        //        if (item.Email == email ){
        //            item.numberVerify = code.ToString();
        //            item.dateCodeVerify = date.ToString();
        //            UpdateToFireBase(item);
        //        }
        //    }
        //}
    }
}
