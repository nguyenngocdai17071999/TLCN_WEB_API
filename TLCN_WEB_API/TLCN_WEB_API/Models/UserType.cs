using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace TLCN_WEB_API.Models
{
    public class UserType
    {
        public string UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        string columnName = "UserType";
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        public UserType(){
            UserTypeID = "";
            UserTypeName = "";
        }
        IFirebaseClient client;
        public List<UserType> getAll() {                              //lấy dứ liệu loại tài khoản
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            return list;
        }

        public void sendMail(string Email, string text)
        {

            MailMessage mail = new MailMessage();
            SmtpClient server = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "nguyenngocdai1707@gmail.com",
                    Password = "apruxaenokwtefyg"
                }
            };
            mail.From = new MailAddress("nguyenngocdai1707@gmail.com");
            mail.To.Add(Email);
            mail.Subject = "Xác nhận quán thành công";
            mail.Body = "Quán " + text + " đã được xác nhận";
            server.Send(mail);

        }

        public string updateCodeForget(string email, string errlive)
        {
            try
            {
                User infoUser = new User();
                UserModel user = null;
                ForGetCode dai = new ForGetCode();
                Random a = new Random();
                int code = a.Next(100000, 999999);             //random code
                DateTime date = DateTime.Now;                  //thời gian tạo code

                //MailMessage mail = new MailMessage();
                //SmtpClient server = new SmtpClient("");
                //mail.From = new MailAddress("nguyenngocdai1707@gmail.com");
                //mail.To.Add(email);
                //mail.Subject = "Mã xác nhận thay đổi mật khẩu";
                //mail.Body = "Mã code:" + code + "";
                //server.Port = 587;
                //server.UseDefaultCredentials = false;
                //server.Credentials = new NetworkCredential("nguyenngocdai1707@gmail.com", "conyeumenhieulam");
                //server.EnableSsl = true;

                //server.Send(mail);

                SmtpClient server = new SmtpClient() {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "nguyenngocdai1707@gmail.com",
                        Password = "apruxaenokwtefyg"
                    }
                };
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nguyenngocdai1707@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Mã xác nhận thay đổi mật khẩu";
                mail.Body = "Mã code:" + code + "";
                server.Send(mail);

                dai.code = code;
                dai.date = date;
                // get list user
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("User");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<User>();
                string err = "";

                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list)
                {                              //cập nhật lại thông tin để xác nhận code và ngày tạo code
                    if (item.Email == email)
                    {
                        item.numberVerify = code.ToString();
                        item.dateCodeVerify = date.ToString();
                        User user1 = new User();
                        user1.UpdateToFireBase(item);
                    }
                }
                errlive = "OK";
                return errlive;
            }
            catch(Exception loi)
            {
                return errlive = loi.Message;
            }    
        }

        public List<UserType> getByID(string id){                         //xem thông tin loại tài khoản truyền vào IDUserType
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
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
            return list2;
        }

        //Update dữ liệu lên firebase theo id
        public void AddbyidToFireBase(string id, UserType usertype){
            client = new FireSharp.FirebaseClient(config);
            var data = usertype;
            data.UserTypeID = id;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }
        //Xóa data
        public void Delete(string id){
            client = new FireSharp.FirebaseClient(config);
            var data = new UserType();
            data.UserTypeID = null;
            data.UserTypeName = null;
            SetResponse setResponse = client.Set("UserType/" + id, data);
        }

        // thêm dư liệu lên firebase
        public void AddToFireBase(UserType userType){
            client = new FireSharp.FirebaseClient(config);
            var data = userType;
            PushResponse response = client.Push("UserType/", data);
            data.UserTypeID = response.Result.name;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }
    }
}
