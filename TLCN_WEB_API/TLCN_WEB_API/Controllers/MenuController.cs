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
   // [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        

        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                Menu menu = new Menu();
                return Ok(menu.getAll());
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                Menu menu = new Menu();
                return Ok(menu.getByID(id));
            }
            catch {
                return Ok("Error");
            }

        }
        
        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Menu menu){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true){
                        try{
                            Menu menu1 = new Menu();
                            menu1.AddbyidToFireBase(id, menu);
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
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult deleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email) == true || infoUser.checkOwner(Email) == true)
                    {
                        try{
                            Menu menu1 = new Menu();
                            menu1.Delete(id);
                            return Ok(new[] { "Xóa thành công" });
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
        [HttpPost("CreateMenu")]
        public IActionResult RegisterMenu( [FromBody] Menu menu){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email) == true)
                    {
                        string err = "";
                        try{
                            Menu menu1 = new Menu();
                            menu1.AddToFireBase(menu);
                            err = "Đăng ký thành công";
                        }
                        catch{
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }
    }
}
