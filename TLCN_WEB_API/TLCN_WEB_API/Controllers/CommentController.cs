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
    public class CommentController : ControllerBase
    {
       
        [HttpGet("GetAll")]

        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                Comment comment = new Comment();
                return Ok(comment.getAll());
            }
            catch{
                return Ok("Error");
            }

        }

        [HttpGet("GetByID")]
        // phương thức get by id store dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                Comment comment = new Comment();
                return Ok(comment.getbyId(id));
            }
            catch{
                return Ok("Error");
            }
            finally
            {

            }
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Comment comment){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    Comment comment1 = new Comment();
                    comment1.AddbyidToFireBase(id, comment);
                    return Ok(new[] { "sửa thành công" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("EditByIDForUser")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByIDForUser(string id, [FromBody] Comment comment)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    Comment comment1 = new Comment();
                    var a = comment1.getbyIdcomment(id);
                    if (a.UserID == userinfo.GetIDToken(Email))
                    {
                        comment1.AddbyidToFireBase(id, comment);
                        return Ok(new[] { "sửa thành công" });
                    }
                    return Ok(new[] { "Không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult deleteByID(string idcomment, string idusercomment, string idStore){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    Comment comment1 = new Comment();
                    if (comment1.GetIDUser(Email)==idusercomment|| comment1.GetIDUser(Email) == comment1.idchuquan(idStore)){
                        comment1.Delete(idcomment);
                        return Ok(new[] { "Xóa thành công" });
                    }
                    return Ok(new[] { "Bạn không được xóa" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });

            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByIDForUser")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult DeleteByIDForUser(string idcomment)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    Comment comment1 = new Comment();
                    var a = comment1.getbyIdcomment(idcomment);
                    if (a.UserID==userinfo.GetIDToken(Email))
                    {
                        comment1.Delete(idcomment);
                        return Ok(new[] { "Xóa thành công" });
                    }
                    return Ok(new[] { "Bạn không được xóa" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });

            }
            catch
            {
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("CreateComment")]
        public IActionResult RegisterComment([FromBody] Comment comment){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    Comment comment1 = new Comment();
                    string Idcomment = comment1.AddToFireBase(comment);
                    err = Idcomment;
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
