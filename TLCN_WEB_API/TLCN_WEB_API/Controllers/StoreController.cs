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
    public class StoreController : ControllerBase
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
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }
        [HttpGet("GetByID/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByID(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.StoreID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }
        [HttpGet("GetByIDProvince/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByIDProvince(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.ProvinceID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }
        [HttpGet("GetByIDBusinessType/{id:int}")]
        // phương thức get by id dữ liệu từ firebase 
        public async Task<IActionResult> GetByIDBusinessType(int id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            //danh sách tìm kiếm

            foreach (var item in data)
            {

                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Store>();
            foreach (var item in list)
            {
                if (item.BusinessTypeID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpPost("EditByID/{id:int}")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(int id, [FromBody] Store store)
        {

            try
            {
                AddbyidToFireBase(id, store);
                return Ok(new[] { "sửa thành công" });
            }
            catch
            {
                return Ok(new[] { "Lỗi rồi" });
            }
        }

        [HttpPost("CreateStore")]
        public IActionResult RegisterStore([FromBody] Store store)
        {
            string err = "";
            try
            {
                
                 AddToFireBase(store);
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
            FirebaseResponse response = client.Get("Store");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Store>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Store>(((JProperty)item).Value.ToString()));
            }
            int i = 1;
            while (1 == 1)
            {
                int dem = 0;
                foreach (var item in list)
                {
                    if (item.StoreID == i)
                        dem++;
                }
                if (dem == 0)
                    return i;
                i++;
            }
            return i;
        }
        // thêm dư liệu lên firebase
        private void AddToFireBase(Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = GetID();
            SetResponse setResponse = client.Set("Store/" + data.StoreID, data);
        }


        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(int id, Store store)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = store;
            data.StoreID = id;
            SetResponse setResponse = client.Set("Store/" + data.StoreID, data);
        }
    }
}