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

namespace TLCN_WEB_API.Models
{
    public class Comment
    {
        public string CommentID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
        public string UserID { get; set; }
        public string StoreID { get; set; }
        public string ParentComment_ID { get; set; }
        public string UserName { get; set; }
        public string UserPicture { get; set; }
        public string RatePoint { get; set; }



        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };
        string columnName = "Comment";                  //tên bảng
        IFirebaseClient client;                        //biến client kết nối firebase

        public Comment()
        {
            CommentID = "";
            Content = "";
            Date = "";
            Image = "";
            UserID = "";
            StoreID = "";
            ParentComment_ID = "";
            UserName = "";
            UserPicture = "";
            RatePoint = "";
        }

        //Update data
        public void AddbyidToFireBase(string id, Comment comment){     //IDComment
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
        }

        public List<Comment> getAll() {                                //Lấy tất cả dữ liệu comment
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Comment>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
            }
            return list;
        }
        public List<Comment> getbyId(string id) {                             //Lấy dữ liệu comment của quán IDStore
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Comment>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Comment>();
            foreach (var item in list){
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return list2;
        }
        public Comment getbyIdcomment(string id)                                   //Lấy dữ liệu comment truyền vào IDcomment
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get(columnName);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Comment>();
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Comment>(((JProperty)item).Value.ToString()));
            }
            Comment a = new Comment();          
            foreach (var item in list){
                if (item.CommentID == id){
                    a = item;
                }    
            }
            return a;
        }

        public void Delete(string id){                                          //Xóa comment
            client = new FireSharp.FirebaseClient(config);
            var data = new Comment();
            // data.CommentID = id;
            SetResponse setResponse = client.Set("Comment/" + id, Nulldata(data));
        }

        public Comment Nulldata(Comment comment){                     //Xét null các giá trị để xóa trên firebase
            comment.CommentID = null;
            comment.Content = null;
            comment.Date = null;
            comment.Image = null;
            comment.UserID = null;
            comment.StoreID = null;
            comment.ParentComment_ID = null;
            comment.UserName = null;
            comment.UserPicture = null;
            comment.RatePoint = null;
            return comment;
        }

        public string GetIDUser(string Email){                                    //lấy thông tin User bằng EMail
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

        public string idchuquan(string idstore)                                              //Lấy ID chủ quán
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Stores");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list){
                if (item.StoreID.ToString() == idstore)
                    return item.UserID;
            }
            return "";
        }

        // thêm dư liệu lên firebase
        public string AddToFireBase(Comment comment){
            client = new FireSharp.FirebaseClient(config);
            var data = comment;
            PushResponse response = client.Push("Comment/", data);
            data.CommentID = response.Result.name;
            SetResponse setResponse = client.Set("Comment/" + data.CommentID, data);
            return data.CommentID;
        }

    }
}
