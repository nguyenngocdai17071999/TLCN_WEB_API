﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLCN_WEB_API.Models;

namespace TLCN_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
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
            FirebaseResponse response = client.Get("Menu");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }
        [HttpGet("GetByID/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByID(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Menu");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Menu>();
            foreach (var item in list)
            {
                if (item.MenuID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }
        [HttpGet("GetByIDStore/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByIDStore(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Menu");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Menu>();
            foreach (var item in list)
            {
                if (item.Store_ID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpPost("EditByID/{id:int}")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(int id, [FromBody] Menu menu)
        {

            try
            {
                AddbyidToFireBase(id, menu);
                return Ok(new[] { "sửa thành công" });
            }
            catch
            {
                return Ok(new[] { "Lỗi rồi" });
            }
        }

        [HttpPost("CreateMenu")]
        public IActionResult RegisterMenu([FromBody] Menu menu)
        {
            string err = "";
            try
            {

                AddToFireBase(menu);
                err = "Đăng ký thành công";
            }
            catch
            {
                err = "Lỗi rồi";
            }
            return Ok(new[] { err });

        }



        //tim ra ID tự động bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        private int GetID()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Menu");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Menu>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1)
            {
                int dem = 0;
                foreach (var item in list)
                {
                    if (item.MenuID == i)
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(Menu menu)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = menu;
            data.MenuID = GetID();
            SetResponse setResponse = client.Set("Menu/" + data.MenuID, data);
        }


        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(int id, Menu menu)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = menu;
            data.MenuID = id;
            SetResponse setResponse = client.Set("Menu/" + data.MenuID, data);
        }
    }
}
