using System;
using System.Collections.Generic;
using System.Linq;
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
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        IFirebaseClient client;

        [HttpGet("GetAll")]

        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll()
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

        [HttpGet("GetByID/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByID(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("UserType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<UserType>();
            foreach (var item in list)
            {
                if (item.UserTypeID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }


        [HttpPost("EditByID/{id:int}")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(int id, [FromBody] UserType usertype)
        {

            try
            {
                AddbyidToFireBase(id, usertype);
                return Ok(new[] { "ok" });
            }
            catch
            {
                return Ok(new[] { "not ok" });
            }
        }


        [HttpPost("CreateUserType")]
        public IActionResult RegisterUser([FromBody] UserType userType)
        {
            string err = "";
            try
            {
                AddToFireBase(userType);
                err = "Đăng ký thành công";

                

            }
            catch
            {
                err = "Lỗi rồi";
            }
            return Ok(new[] { err });

        }




        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(int id, UserType usertype)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = usertype;
            data.UserTypeID = id;
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(UserType userType)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = userType;
            data.UserTypeID = GetID();
            SetResponse setResponse = client.Set("UserType/" + data.UserTypeID, data);
        }
        //tim ra ID tự động của user bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("UserType");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<UserType>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<UserType>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1)
            {
                int dem = 0;
                foreach (var item in list)
                {
                    if (item.UserTypeID == i)
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }
    }
}
