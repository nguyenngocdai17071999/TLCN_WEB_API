using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
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
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        [HttpGet("GetAll")]                  //Lấy tất cả dữ liệu món ăn
        public IActionResult GetAll(){
            try{
                Dish dish = new Dish();      //Khai báo model Dish
                return Ok(dish.getAll());    //Trả về danh sách món ăn
            }
            catch{
                return Ok("Error");
            }
        }


        [HttpGet("GetByIDStore")]                            //Lấy tất cả dữ liệumón ăn theo quán truyền vào IDStore
        public IActionResult GetByIDStrore(string id){
            try{
                Dish dish = new Dish();                      //Khai báo model Dish
                return Ok(dish.getByIDStore(id));            //Trả về danh sách món ăn của quán
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                  //Lấy tất cả dữ liệu món ăn truyền vào IDDish
        public IActionResult GetByID(string id){
            try{
                Dish dish = new Dish();                       //Khai báo model Dish
                return Ok(dish.getByID(id));                  //Trả về dữ liệu món ăn
            }
            catch{
                return Ok("Error");
            }
        }


        [HttpGet("Search")]                                                          //Api tìm kiếm truyền vào KeyWord là tên món ăn hoặc bất kì, lat, long nếu thay đổi location
        public IActionResult Search(string dishname,double Lat, double Long){  

                if (dishname != null){                                               //Kiểm tra nếu không có nhập gì thì trả về không có kết quả
                    Dish dish = new Dish();                                          //Khai báo model dish
                    return Ok(dish.search(dishname, Lat, Long));                     //Trả về danh sách quán ăn theo keyword
                }
                return Ok("Không có kết quả tìm kiếm");

        }

        [Authorize]
        [HttpPost("EditByID")]                                                                  //Chỉnh sửa món ăn truyền vào IDDish và nội dung món ăn
        public IActionResult EditByID(string id, [FromBody] Dish dish){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User userinfo = new User();                                                     //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (userinfo.checkAdmin(Email) == true || userinfo.checkOwner(Email)==true){//Kiểm tra có phải admin,owner không
                        try{                                                                    
                            Dish dish1 = new Dish();                                            //Khai báo biến Model Dish
                            dish1.AddbyidToFireBase(id, dish);                                  //Update data
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                                                     //Xóa món ăn truyền vào idDish
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                          //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                       //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                       //Email của token             
                User userinfo = new User();                                                          //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){          //kiểm tra thời gian đăng nhập còn không
                    if (userinfo.checkAdmin(Email)==true || userinfo.checkOwner(Email) == true){     //Kiểm tra có phải admin hoặc owner không
                        try{                                                                         
                            Dish dish = new Dish();                                                  //Khai báo biến Model DiscountDish
                            dish.Delete(id);                                                         //Xóa data
                            return Ok(new[] { "Xóa thành công" });
                        }
                        catch
                        {
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("CreateDish")]                                                                      //Tạo món ăn
        public IActionResult RegisterDish([FromBody] Dish dish){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                           //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                        //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                        //Email của token             
                User userinfo = new User();                                                           //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){           //kiểm tra thời gian đăng nhập còn không
                    if (userinfo.checkAdmin(Email) == true || userinfo.checkOwner(Email) == true){    //Kiểm tra có phải admin hoặc owner không
                        string err = "";                                                              
                        try{                                                                          
                            Dish dish1 = new Dish();                                                  //Khai báo biến Dish 
                            dish1.AddToFireBase(dish);                                                //Thêm data
                            err = "Đăng ký thành công";
                        }
                        catch{
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }
    }
}
