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
    //[EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                Province province = new Province();
                return Ok(province.getAll());
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                Province province = new Province();
                return Ok(province.getByID(id));
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Province province){            
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true){
                        Province province1 = new Province();
                        province1.AddbyidToFireBase(id, province);
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
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true){
                        Province province = new Province();
                        province.Delete(id);
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
        [HttpPost("CreateProvince")]
        public IActionResult RegisterProvince([FromBody] Province province){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email) == true)
                    {
                        Province province1 = new Province();
                        province1.AddToFireBase(province);
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
