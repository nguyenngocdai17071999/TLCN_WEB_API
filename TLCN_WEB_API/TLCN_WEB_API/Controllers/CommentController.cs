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
        [HttpGet("GetAll")]                                 //Lấy tất cả dữ liệu comment
        public IActionResult GetAll(){
            try{
                Comment comment = new Comment();            //Khai báo Model Comment
                return Ok(comment.getAll());                //trả về danh sách toàn bộ comment
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                //Lấy tất cả dữ liệu comment của quán truyền vào id quán
        public IActionResult GetByID(string id){
            try{
                Comment comment = new Comment();            //Khai báo biến comment
                return Ok(comment.getbyId(id));             //Trả về danh sách toàn bộ comment của quán 
            }
            catch{
                return Ok("Error");
            }            
        }

        [Authorize]
        [HttpPost("EditByID")]                                                           //Chỉnh sửa thông tin comment
        public IActionResult EditByID(string id, [FromBody] Comment comment){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;              //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                           //Danh sách các biến trong identity
                string Email = claim[1].Value;                                           //Email của token   
                User userinfo = new User();                                              //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true 
                    && userinfo.checkAdmin(Email) == true){                              //kiểm tra thời gian đăng nhập còn không và có phải là admin không
                    Comment comment1 = new Comment();                                    //khai báo model comment
                    comment1.AddbyidToFireBase(id, comment);                             //Update data
                    return Ok(new[] { "sửa thành công" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("EditByIDForUser")]                                                            //Thay đổi comment truyền vào Id của comment và Body comment
        public IActionResult EditByIDForUser(string id, [FromBody] Comment comment){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                      //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                   //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                   //Email của token   
                User userinfo = new User();                                                      //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){      //kiểm tra thời gian đăng nhập còn không
                    Comment comment1 = new Comment();                                            //Khai báo biến model Comment
                    var a = comment1.getbyIdcomment(id);                                         //lấy dữ liệu comment
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
        public IActionResult deleteByID(string idcomment, string idusercomment, string idStore){                                 //Xóa comment truyền vào Id của comment , id người comment và id quán
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                                                       //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                                                    //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                                                    //Email của token   
                User userinfo = new User();                                                                                       //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){                                       //kiểm tra thời gian đăng nhập còn không
                    Comment comment1 = new Comment();                                                                             //Khai báo biến model Comment
                    if (comment1.GetIDUser(Email)==idusercomment|| comment1.GetIDUser(Email) == comment1.idchuquan(idStore)){     //Kiểm tra đúng người người xóa phải người comment không hoặc chủ quán xóa 
                        comment1.Delete(idcomment);                                                                               //Xóa comment
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
        public IActionResult DeleteByIDForUser(string idcomment){                                      //Xóa comment truyền vào Id của comment 
            try{                                                                                                                                                    
                var identity = HttpContext.User.Identity as ClaimsIdentity;                            //khai báo biến danh tính của token                                         
                IList<Claim> claim = identity.Claims.ToList();                                         //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                         //Email của token   
                User userinfo = new User();                                                            //Khai bao biến thông tin người dùng
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){            //kiểm tra thời gian đăng nhập còn không                          
                    Comment comment1 = new Comment();                                                  //Khai báo biến model Comment                                               
                    var a = comment1.getbyIdcomment(idcomment);                                        //Lấy dữ liệu comment
                    if (a.UserID==userinfo.GetIDToken(Email)){                                         //Kiểm tra đúng người comment không
                        comment1.Delete(idcomment);                                                    //Delete comment
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
        [HttpPost("CreateComment")]
        public IActionResult RegisterComment([FromBody] Comment comment){                             //Xóa comment truyền vào body model comment
            string err = "";                                                                                                                                       
            try{                                                                                                                   
                var identity = HttpContext.User.Identity as ClaimsIdentity;                           //khai báo biến danh tính của token     
                IList<Claim> claim = identity.Claims.ToList();                                        //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                        //Email của token   
                User userinfo = new User();                                                           //Khai bao biến thông tin người dùng                          
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){           //kiểm tra thời gian đăng nhập còn không                             
                    Comment comment1 = new Comment();                                                 //Khai báo biến model Comment           
                    string Idcomment = comment1.AddToFireBase(comment);                               //Thêm comment
                    err = Idcomment;                                                                  //Trả về IDcomment
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
