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
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLCN_WEB_API.Models;


namespace TLCN_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public IActionResult GetAll(){                                          //lấy danh sách tài khoản 
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                         //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                      //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                      //Email của token             
                User infoUser = new User();                                                         //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){         //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                          //Kiểm tra có phải admin không
                        return Ok(infoUser.getAll());
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
        [HttpGet("GetRole")]                                            //lấy role của tài khoản
        public IActionResult getRole(){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                       //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                    //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                    //Email của token             
                User infoUser = new User();                                                       //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){       //kiểm tra thời gian đăng nhập còn không
                    return Ok(infoUser.getRole(Email));                                           
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
        }

        [Authorize]
        [HttpGet("CheckLogin")]                                                                 //kiểm tra còn thời gian đăng nhập không
        public IActionResult CheckLogin()
        {
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không               
                    return Ok("Đang login");                                                    
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpGet("GetByID")]                                                                    //lấy thông tin tài khoản 
        public IActionResult GetByID(string id){                                               //IDUser
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không                                                                                                
                    return Ok(infoUser.getByID(id,Email));
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
        }

        [Authorize]
        [HttpPost("EditByID")]                                                                 //Chỉnh sửa thông tin tài khoản
        public IActionResult EditByID(string id, [FromBody] User user){                        //IDUser
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                    //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                 //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                 //Email của token             
                User infoUser = new User();                                                    //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){    //kiểm tra thời gian đăng nhập còn không
                    infoUser.editByID(id, Email, user);                                        
                    return Ok(new[] { "Sửa thành công" });              
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                                                  //xóa tài khoản
        public IActionResult DeleteByID(string id){                                               //IDUser
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                       //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                    //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                    //Email của token             
                User infoUser = new User();                                                       //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){       //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true ){                                       //Kiểm tra có phải admin không
                        infoUser.Delete(id);                                                      //Delete data
                        return Ok(new[] { "Xóa thành công" });
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [HttpPost("RegisterUser")]                                                  // đăng kí tài khoản user
        public IActionResult RegisterUser([FromBody] User user){
            string err = "";
            User infoUser = new User();
            try{
                if (infoUser.kiemtraEmail(user.Email) == false){                    // kiểm tra có trùng email không
                    infoUser.AddToFireBase(user);
                    err = "Đăng ký thành công";
                }
                else{
                    err = "Email đã tồn tại";
                }
            }
            catch{
                err = "Error";
            }
            return Ok(new[] { err });
        }

        [HttpPost("RegisterOwner")]
        public IActionResult RegisterOwner([FromBody] User user){                     // đăng kí tài khoản owner
            string err = "";
            User infoUser = new User();
            try{
                if (infoUser.kiemtraEmail(user.Email) == false){                      //Kiểm tra có trùng Email không
                    err = infoUser.AddToFireBaseReturn(user);
                }
                else{
                    err = "Email đã tồn tại";
                }
            }
            catch{
                err = "Error";
            }
            return Ok(new[] { err });
        }

        [HttpPost("Login")]                                                         //đăng nhập
        public IActionResult Login([FromBody] Login userlogin){ 
            try{
                User infoUser = new User();                                         //model User
                UserModel login = new UserModel();                                  //model login
                login.EmailAddress = userlogin.Email;
                login.PassWord = userlogin.PassWord;
                IActionResult response = Unauthorized();                            //biến phản hồi

                var user = infoUser.AuthenticationUser(login);                      // kiểm tra thông tin đăng nhập
                if (user != null){
                    if (user.Status == "2")                                         // kiểm tra tài khoản có bị khóa không
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);             // tạo token cho tài khoản
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpPost("LoginFaceBook")]                                                    // đăng nhập bằng Facebook
        public IActionResult LoginFaceBook(string idFacebook, string email){      //Idfacebook, Email
            try{
                User infoUser = new User();                                       //model User
                UserModel login = new UserModel();                                //model login
                login.EmailAddress = email;
                login.idFacebook = idFacebook;
                IActionResult response = Unauthorized();                          //biến phản hồi

                var user = infoUser.AuthenticationUserFaceBook(login);            // kiểm tra thông tin đăng nhập
                if (user != null){
                    if (user.Status == "2")                                       // kiểm tra tài khoản có bị khóa không
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);           // tạo token cho tài khoản
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpPost("LoginGoogle")]                                                 //đăng nhập bằng google
        public IActionResult LoginGoogle(string idGoogle, string email){           //IdGoogle, Email
            try{
                User infoUser = new User();                                        //model User
                UserModel login = new UserModel();                                 //model login
                login.EmailAddress = email;
                login.idGoogle = idGoogle;
                IActionResult response = Unauthorized();                           //biến phản hồi

                var user = infoUser.AuthenticationUserGoogle(login);               // kiểm tra thông tin đăng nhập
                if (user != null){
                    if (user.Status == "2")                                        // kiểm tra tài khoản có bị khóa không
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);            // tạo token cho tài khoản
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch{
                return Ok("Error");
            }
        }
     
        [HttpPost("Logout")]                                              // đăng xuất
        public IActionResult Logout(string id){
            try{
                User infoUser = new User();
                infoUser.blockAccount(id, "1");                           //chuyển về trạng thái off
                return Ok("Thay đổi thành công");
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [Authorize]
        [HttpPost("BlockAccount")]                                               // khóa tài khoản
        public IActionResult BlockAccount(string id, string status){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                        //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                     //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                     //Email của token             
                User infoUser = new User();                                                        //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){        //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true){                                         //Kiểm tra có phải admin không
                        infoUser.blockAccount(id, status);                                         //Block account
                        return Ok("Thay đổi thành công");
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok(new[] { "Error" });
            }
        }

        [HttpPost("ForgetPass")]                                        //quên mật khẩu
        public IActionResult ForgetPass(string Email){
            try{
                User infoUser = new User();
                if (infoUser.kiemtraEmail(Email) == true){               //kiểm tra Email có tồn tại không
                    infoUser.updateCodeForget(Email);                     //lưu lại mã code và gửi mã code đên email
                }
                return Ok(new[] { "Không có Email" });
            }
            catch{
                return Ok("Error");
            }            
        }

        [HttpPost("ResetPass")]                                                         //Đổi mật khẩu
        public IActionResult ResetPass(string Email,string Password, int code){
            try{
                User infoUser = new User();
                if (infoUser.kiemtraEmail(Email) == true){                             //Kiểm tra Email tồn tại không
                    if(infoUser.kiemtraCode(code,Email) == true){                      //Kiểm tra đúng code không
                        infoUser.resetPass(Email, Password);                            //đổi mật khẩu
                        return Ok(new[] { "Đổi mật khẩu thành công" });
                    }    
                    else
                        return Ok(new[] { "Mã code sai" });
                }
                return Ok(new[] { "Không có Email" });
            }
            catch{
                return Ok("Error");
            }            
        }

        [HttpGet("ThongKeNguoiOnline")]                                      //Thống kê số người đang online
        public IActionResult ThongKeNguoiOnline(){
            try{
                User infoUser = new User();
                return Ok(infoUser.ThongKeNguoiOnlie());                      //trả về 1 con số hiển thị số người đang online
            }
            catch{
                return Ok("Error");
            }
        }
    }
}
