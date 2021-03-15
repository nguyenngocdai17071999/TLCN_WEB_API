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
    public class DishTypeController : ControllerBase
    {     
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                DishType dishType = new DishType();
                return Ok(dishType.getAll());
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                DishType dishType = new DishType();
                return Ok(dishType.getById(id));
            }
            catch{
                return Ok("Error");
            }

        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] DishType dishType){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userInfo = new User();
                if (userInfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (userInfo.checkAdmin(Email)==true){
                        DishType dishType1 = new DishType();
                        dishType1.AddbyidToFireBase(id, dishType);
                        return Ok(new[] { "sửa thành công" });
                    }
                    else
                    {
                        return Ok("Bạn không có quyền");
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
        public IActionResult deleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true) {
                    if (userinfo.checkAdmin(Email)==true)
                    {
                        userinfo.Delete(id);
                        return Ok(new[] { "Xóa thành công" });
                    }
                    else{
                        return Ok("Bạn không có quyền");
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateDishType")]
        public IActionResult RegisterDishType([FromBody] DishType dishType){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (userinfo.checkAdmin(Email)== true)
                    {
                        DishType dishType1 = new DishType();
                        dishType1.AddToFireBase(dishType);
                        err = "Đăng ký thành công";
                    }
                    else{
                        err = "Bạn không có quyền";
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
