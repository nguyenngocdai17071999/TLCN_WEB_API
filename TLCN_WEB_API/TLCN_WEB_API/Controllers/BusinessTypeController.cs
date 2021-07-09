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
    public class BusinessTypeController : ControllerBase
    {
        [HttpGet("GetAll")]                                     //Lấy tất cả dữ liệu loại hình kinh doanh quán ăn
        public IActionResult GetAll(){
            try{
                BusinessType danhsach = new BusinessType();     //Khai báo biến Model
                return Ok(danhsach.getAll());                   //trả về danh sách dữ liệu
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                    // Lấy dữ liệu loại hình kinh doanh quán ăn theo ID
        public IActionResult GetByID(string id){
            try{
                BusinessType danhsach = new BusinessType();     //Khai báo biến Model
                return Ok(danhsach.getByID(id));                //trả về danh sách dữ liệu
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                              //Chỉnh sửa dữ liệu loại hình kinh doanh quán ăn
        public IActionResult EditByID(string id, [FromBody] BusinessType businessType){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                 //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                              //Danh sách các biến trong identity
                string Email = claim[1].Value;                                              //Email của token             
                User infoUser = new User();                                                 //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){ //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                //Kiểm tra có phải admin không
                        BusinessType businessType1 = new BusinessType();                    //Khai báo biến Model BusinessType 
                        businessType1.AddbyidToFireBase(id, businessType);                  //Update data
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
        [HttpPost("DeleteByID")]                                                             //Xóa dữ liệu loại hình kinh doanh quán ăn
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                  //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                               //Danh sách các biến trong identity
                string Email = claim[1].Value;                                               //Email của token            
                User infoUser = new User();                                                  //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){  //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                 //Kiểm tra có phải admin không
                        BusinessType businessType1 = new BusinessType();                     //Khai báo biến Model BusinessType 
                        businessType1.Delete(id);                                            //delete data
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
        [HttpPost("CreateBusinessType")]                                                    //Thêm dữ liệu loại hình kinh doanh quán ăn
        public IActionResult RegisterBusinessType([FromBody] BusinessType businessType){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                 //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                              //Danh sách các biến trong identity
                string Email = claim[1].Value;                                              //Email của token  
                User infoUser = new User();                                                 //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){ //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true ){                               //Kiểm tra có phải admin không
                        BusinessType businessType1 = new BusinessType();                    //Khai báo biến Model BusinessType 
                        businessType1.AddToFireBase(businessType);                          //Thêm mới một data
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
