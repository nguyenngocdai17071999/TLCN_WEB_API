using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
   // [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private static string key = "TLCN";
        IFirebaseConfig config = new FirebaseConfig{
            AuthSecret = "0ypBJAvuHDxyKu9sDI6xVtKpI6kkp9QEFqHS92dk",
            BasePath = "https://tlcn-1a9cf.firebaseio.com/"
        };

        IFirebaseClient client;

        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Dishes");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();
            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            return Ok(list);
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Dishes");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list){
                if (item.Dish_ID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpGet("GetByIDMenu")]
        // phương thức get by id menu dữ liệu từ firebase 
        public IActionResult GetByIDMenu(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Dishes");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list){
                if (item.Menu_ID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }

        [HttpGet("Search")]
        //phương thức get dữ liệu từ firebase
        public IActionResult Search(string dishname){            
            try{
                if(dishname!=null){
                    dishname = convertToUnSign3(dishname.ToLower());

                    //danh sach store
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse responsestore = client.Get("Store");
                    dynamic datastore = JsonConvert.DeserializeObject<dynamic>(responsestore.Body);
                    var liststore = new List<Store>();

                    //danh sách tìm kiếm
                    foreach (var itemstore in datastore){
                        liststore.Add(JsonConvert.DeserializeObject<Store>(((JProperty)itemstore).Value.ToString()));
                    }

                    //danh sach dish
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse responsedish = client.Get("Dishes");
                    dynamic datadish = JsonConvert.DeserializeObject<dynamic>(responsedish.Body);
                    var listdish = new List<Dish>();

                    //danh sách tìm kiếm
                    foreach (var itemdish in datadish){
                        listdish.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)itemdish).Value.ToString()));
                    }

                    //danh sach menu
                    client = new FireSharp.FirebaseClient(config);
                    FirebaseResponse responseMenu = client.Get("Menu");
                    dynamic datamenu = JsonConvert.DeserializeObject<dynamic>(responseMenu.Body);
                    var listMenu = new List<Menu>();

                    //danh sách tìm kiếm
                    foreach (var itemMenu in datamenu){
                        listMenu.Add(JsonConvert.DeserializeObject<Menu>(((JProperty)itemMenu).Value.ToString()));
                    }

                    var list2 = new List<Store>();
                    var MenuID = new List<string>();

                    foreach (var item in listdish){
                        if ((convertToUnSign3(item.DishName.ToLower())).Contains(dishname)){
                            MenuID.Add(item.Menu_ID);
                        }
                    }
                    var MenuID2 = new List<string>();
                    foreach (var item in MenuID){
                        MenuID2.Add(item);
                        break;
                    }
                    foreach (var item in MenuID){
                        int dem = 0;
                        foreach (var item2 in MenuID2){
                            if (item == item2)
                                dem++;
                        }
                        if (dem == 0) MenuID2.Add(item);
                    }
                    if (MenuID2.Count == 0){
                        var list3 = new List<Store>();
                        foreach (var item in liststore){
                            if ((convertToUnSign3(item.StoreName.ToLower())).Contains(dishname))
                                list3.Add(item);
                        }
                        if (list3.Count == 0){
                            var list4 = new List<Store>();
                            foreach (var item in liststore){
                                if ((convertToUnSign3(item.StoreAddress.ToLower())).Contains(dishname))
                                    list4.Add(item);
                            }
                            return Ok(list4);
                        }
                        return Ok(list3);
                    }
                    else
                    {
                        foreach (var item in liststore){
                            foreach (var item2 in MenuID2){
                                if (item.MenuID == item2) list2.Add(item);
                            }
                        }
                        return Ok(list2);
                    }
                }                
                return Ok("Không có kết quả tìm kiếm");
            }
            catch {
                return Ok("Không có kết quả tìm kiếm");
            }
            
        }

        [HttpGet("GetByIDType")]
        // phương thức get by id menu dữ liệu từ firebase 
        public IActionResult GetByIDType(string id){
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Dishes");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Dish>();

            //danh sách tìm kiếm
            foreach (var item in data){
                list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
            }
            var list2 = new List<Dish>();
            foreach (var item in list){
                if (item.DishType_ID == id)
                    list2.Add(item);
            }
            return Ok(list2);
        }
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id,string token, [FromBody] Dish dish){
            if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
            {
                try{
                    AddbyidToFireBase(id, dish);
                    return Ok(new[] { "sửa thành công" });
                }
                catch{
                    return Ok(new[] { "Lỗi rồi" });
                }
            }
            return Ok(new[] { "Bạn không có quyền" });
        }

        [HttpPost("CreateDish")]
        public IActionResult RegisterDish(string token, [FromBody] Dish dish){
            if (GetRole(token) == "-MO5VBnzdGsuypsTzHaV")
            {
                string err = "";
                try{
                    AddToFireBase(dish);
                    err = "Đăng ký thành công";
                }
                catch{
                    err = "Lỗi rồi";
                }
                return Ok(new[] { err });
            }
            return Ok("Bạn không có quyền");
        }

        //tim ra ID tự động bằng cách tăng dần từ 1 nếu đã có số rồi thì lấy số tiếp theo cho đến hết chuổi thì lấy số cuối cùng.
        // vd 1 2 3 thì get id sẽ ra 4
        // vd 1 3 4 thì get id sẽ ra 2
        //private int GetID(){
        //    client = new FireSharp.FirebaseClient(config);
        //    FirebaseResponse response = client.Get("Dishes");
        //    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
        //    var list = new List<Dish>();
        //    foreach (var item in data){
        //        list.Add(JsonConvert.DeserializeObject<Dish>(((JProperty)item).Value.ToString()));
        //    }
        //    int i = 1;
        //    while (1 == 1){
        //        int dem = 0;
        //        foreach (var item in list){
        //            if (item.Dish_ID == i.ToString())
        //                dem++;
        //        }
        //        if (dem == 0)
        //            return i;
        //        i++;
        //    }
        //    return i;
        //}

        // thêm dư liệu lên firebase
        private void AddToFireBase(Dish dish){
            client = new FireSharp.FirebaseClient(config);
            var data = dish;
            PushResponse response = client.Push("Dishes/", data);
            data.Dish_ID = response.Result.name;
            SetResponse setResponse = client.Set("Dishes/" + data.Dish_ID, data);
        }

        //thêm dữ liệu lên firebase theo id
        private void AddbyidToFireBase(string id, Dish dish){
            client = new FireSharp.FirebaseClient(config);
            var data = dish;

            data.Dish_ID = id;
            SetResponse setResponse = client.Set("Dishes/" + data.Dish_ID, data);
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

        public string GetRole(string token){
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
                if (item.Email.ToString() == Decrypt(token))
                    return item.UserTypeID;
            }
            return "";
        }

        public string convertToUnSign3(string s){
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
