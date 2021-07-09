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
    public class DistrictController : Controller
    {
        [HttpGet("GetAll")]                                    //Lấy tất cả dữ liệu quận
        public IActionResult GetAll(){
            try{
                District danhsach = new District();            //Khai báo model District
                return Ok(danhsach.getAll());                  //Trả về danh sách khuyến mãi món ăn
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                     //Lấy tất cả dữ liệu quận truyền vào IDDistrict
        public IActionResult GetByID(string id){
            try{
                District danhsach = new District();              //Khai báo model District
                return Ok(danhsach.getByID(id));                 //Trả về danh sách District
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDProvince")]                                //Lấy tất cả dữ liệu quận theo Thành phố truyền vào IDProvince
        public IActionResult GetByIDProvince(string id){
            try{
                District danhsach = new District();                 //Khai báo model District
                return Ok(danhsach.getByIDProvince(id));            //Trả về danh sách District
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                               //Chỉnh sửa quận truyền vào IDDistrict
        public IActionResult EditByID(string id, [FromBody] District district){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                  //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                               //Danh sách các biến trong identity
                string Email = claim[1].Value;                                               //Email của token             
                User infoUser = new User();                                                  //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){  //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                 //Kiểm tra có phải admin không
                        District district1 = new District();                                 //Khai báo biến Model District
                        district1.AddbyidToFireBase(id, district);                           //Update data
                        return Ok(new[] { "sửa thành công" });
                    }
                    else{
                        return Ok(new[] { "Bạn Không có quyền" });
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                                             //Xóa quận truyền vào IDDistrict
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                  //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                               //Danh sách các biến trong identity
                string Email = claim[1].Value;                                               //Email của token             
                User infoUser = new User();                                                  //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){  //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                 //Kiểm tra có phải admin không
                        District district = new District();                                  //Khai báo biến Model District 
                        district.Delete(id);                                                 //Delete data
                        return Ok(new[] { "Xóa thành công" });
                    }
                    else{
                        return Ok(new[] { "Bạn Không có quyền" });
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("Create")]                                                                    //Thêm district
        public IActionResult Register([FromBody] District district){
            string err = "";
            try{                                                                                
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                    //Kiểm tra có phải admin không
                        District district1 = new District();                                    //Khai báo biến Model District
                        district1.AddToFireBase(district);                                      //Create data
                        err = "Đăng ký thành công";
                    }
                    else{
                        err = "Bạn Không có quyền";
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                err = "Error";
            }
            return Ok(new[] { err });
        }
    }
}
