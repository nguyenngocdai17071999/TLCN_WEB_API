using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using GoogleMaps.LocationServices;
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
    public class UserTypeController : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]                                                                  //Lấy danh sách loại tài khoản
        public IActionResult GetAll() {
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                  //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                               //Danh sách các biến trong identity
                string Email = claim[1].Value;                                               //Email của token             
                User infoUser = new User();                                                  //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){  //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                   //Kiểm tra có phải admin không
                        UserType infoUserType = new UserType();                              //Khai báo biến Model Usertype
                        return Ok(infoUserType.getAll());                                    //Trả vê danh sách loại tài khoản
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch {
                return Ok("Error");
            } 
        }      

        [Authorize]
        [HttpGet("GetByID")]                                                                    //lấy thông tin loại tài khoản truyền vào IDUserType
        public IActionResult GetByID(string id) {
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                      //Kiểm tra có phải admin không
                        UserType userType = new UserType();                                     //Khai báo biến Model Usertype
                        return Ok(userType.getByID(id));                                        //Trả về thông tin 
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }            
        }

        [Authorize]
        [HttpPost("EditByID")]                                                                   //Chỉnh sửa loại tài khoản
        public IActionResult EditByID(string id, [FromBody] UserType usertype){
            try {
                var identity = HttpContext.User.Identity as ClaimsIdentity;                      //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                   //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                   //Email của token             
                User infoUser = new User();                                                      //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){      //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                       //Kiểm tra có phải admin không
                        UserType userType = new UserType();                                      //Khai báo biến Model Usertype
                        userType.AddbyidToFireBase(id, usertype);                                //Update data
                        return Ok(new[] { "ok" });
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });                
            }
            catch {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                                                 //Xóa loại tài khoản
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                      //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                   //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                   //Email của token             
                User infoUser = new User();                                                      //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){      //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                       //Kiểm tra có phải admin không
                        UserType userType = new UserType();                                      //Khai báo biến Model Usertype
                        userType.Delete(id);                                                     //Xóa data
                        return Ok(new[] { "Xóa thành công" });
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateUserType")]                                                          //Thêm loại tài khoản
        public IActionResult RegisterUser([FromBody] UserType userType){
            string err = "";
            try{                                                                              
                var identity = HttpContext.User.Identity as ClaimsIdentity;                   //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                //Email của token             
                User infoUser = new User();                                                   //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){   //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                    //Kiểm tra có phải admin không
                        UserType userType2 = new UserType();                                  //Khai báo biến Model Usertype
                        userType2.AddToFireBase(userType);                                    //Thêm data
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
