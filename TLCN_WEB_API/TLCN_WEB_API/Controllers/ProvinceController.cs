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
    public class ProvinceController : ControllerBase
    {        
        [HttpGet("GetAll")]                                //Lấy tất cả dữ liệu thành phố
        public IActionResult GetAll(){
            try{
                Province province = new Province();        //Khai báo model Province
                return Ok(province.getAll());              //Trả về danh sách thành phố
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetByID")]                               //Lấy tất cả dữ liệu thành phố truyền vào IDProvince
        public IActionResult GetByID(string id){
            try{
                Province province = new Province();        //Khai báo model Province
                return Ok(province.getByID(id));           //Trả về danh sách thành phố
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                               //Chỉnh sửa thông tin thành phố
        public IActionResult EditByID(string id, [FromBody] Province province){            
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                  //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                               //Danh sách các biến trong identity
                string Email = claim[1].Value;                                               //Email của token             
                User infoUser = new User();                                                  //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){  //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                   //Kiểm tra có phải admin không
                        Province province1 = new Province();                                 //Khai báo biến Model Province 
                        province1.AddbyidToFireBase(id, province);                           //Update data
                        return Ok(new[] { "sửa thành công" });
                    }
                    else{
                        return Ok("Bạn Không có quyền");
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });               
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                                                   //Xóa thông tin thành phố
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                        //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                     //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                     //Email của token             
                User infoUser = new User();                                                        //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){        //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                         //Kiểm tra có phải admin không
                        Province province = new Province();                                        //Khai báo biến Model Province 
                        province.Delete(id);                                                       //delete data
                        return Ok(new[] { "Xóa thành công" });
                    }
                    else{
                        return Ok("Bạn Không có quyền");
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateProvince")]                                                           //thêm thông tin thành phố
        public IActionResult RegisterProvince([FromBody] Province province){
            string err = "";
            try{                                                                              
                var identity = HttpContext.User.Identity as ClaimsIdentity;                    //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                 //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                 //Email của token             
                User infoUser = new User();                                                    //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){    //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                   //Kiểm tra có phải admin không
                        Province province1 = new Province();                                   //Khai báo biến Model Province 
                        province1.AddToFireBase(province);                                     //Create data
                        return Ok(new[] { "Đăng ký thành công" });
                    }
                    else{                        
                        return Ok(new[] { "Bạn Không có quyền" });
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
