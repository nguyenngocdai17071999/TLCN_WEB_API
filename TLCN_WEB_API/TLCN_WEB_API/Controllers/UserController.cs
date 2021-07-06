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
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true){                        
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
        [HttpGet("GetRole")]
        //phương thức get dữ liệu từ firebase
        public IActionResult getRole(){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    return Ok(infoUser.getRole(Email));
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
        }

        [Authorize]
        [HttpGet("CheckLogin")]
        //phương thức get dữ liệu từ firebase
        public IActionResult CheckLogin()
        {
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){                    
                    return Ok("Đang login");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    
                    return Ok(infoUser.getByID(id,Email));
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
        }

        //[HttpGet("GetByIDNotToken")]
        //// phương thức get by id dữ liệu từ firebase 
        //public IActionResult GetByIDnottoken(string id){
        //    try{
        //        User infoUser = new User();
        //        return Ok(infoUser.GetByIDnottoken(id));
        //    }
        //    catch {
        //        return Ok("Error");
        //    }
        //}

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] User user){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
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
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true ){
                        infoUser.Delete(id);
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

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user){
            string err = "";
            User infoUser = new User();
            try
            {
                if (infoUser.kiemtraEmail(user.Email) == false){
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
        public IActionResult RegisterOwner([FromBody] User user)
        {
            string err = "";
            User infoUser = new User();
            try
            {
                if (infoUser.kiemtraEmail(user.Email) == false)
                {
                   
                    err = infoUser.AddToFireBaseReturn(user);
                }
                else
                {
                    err = "Email đã tồn tại";
                }
            }
            catch
            {
                err = "Error";
            }
            return Ok(new[] { err });
        }

        //Hàm login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login userlogin){ 
            try{
                User infoUser = new User();
                UserModel login = new UserModel();
                login.EmailAddress = userlogin.Email;
                login.PassWord = userlogin.PassWord;
                IActionResult response = Unauthorized();

                var user = infoUser.AuthenticationUser(login);
                if (user != null){
                    if (user.Status == "2")
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch{
                return Ok("Error");
            }           
        }



        //Hàm login
        [HttpPost("LoginFaceBook")]
        public IActionResult LoginFaceBook(string idFacebook, string email)
        {
            try
            {
                User infoUser = new User();
                UserModel login = new UserModel();
                login.EmailAddress = email;
                login.idFacebook = idFacebook;
                IActionResult response = Unauthorized();

                var user = infoUser.AuthenticationUserFaceBook(login);
                if (user != null)
                {
                    if (user.Status == "2")
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch
            {
                return Ok("Error");
            }
        }

        //Hàm login
        [HttpPost("LoginGoogle")]
        public IActionResult LoginGoogle(string idGoogle, string email)
        {
            try
            {
                User infoUser = new User();
                UserModel login = new UserModel();
                login.EmailAddress = email;
                login.idGoogle = idGoogle;
                IActionResult response = Unauthorized();

                var user = infoUser.AuthenticationUserGoogle(login);
                if (user != null)
                {
                    if (user.Status == "2")
                        return Ok("Error");
                    var tokenStr = infoUser.GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch
            {
                return Ok("Error");
            }
        }

        //Hàm login       
        [HttpPost("Logout")]
        public IActionResult Logout(string id)
        {
            try
            {
                User infoUser = new User();
                infoUser.blockAccount(id, "1");
                return Ok("Thay đổi thành công");
            }
            catch
            {
                return Ok(new[] { "Error" });
            }
        }

        //Hàm login
        [Authorize]
        [HttpPost("BlockAccount")]
        public IActionResult BlockAccount(string id, string status){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true)
                    {
                        infoUser.blockAccount(id, status);
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

        [HttpPost("ForgetPass")]
        public IActionResult ForgetPass(string Email){///sử dụng thuộc tính Email của models user json chỉ cần Email
            try{
                User infoUser = new User();
                if (infoUser.kiemtraEmail(Email) == true){
                    infoUser.updateCodeForget(Email);
                }
                return Ok(new[] { "Không có Email" });
            }
            catch{
                return Ok("Error");
            }            
        }

        //resetpass theo gmail
        [HttpPost("ResetPass")]
        public IActionResult ResetPass(string Email,string Password, int code){//Sử dụng thuốc tính Email và Password
            try{
                User infoUser = new User();
                if (infoUser.kiemtraEmail(Email) == true){
                    if(infoUser.kiemtraCode(code,Email) == true)
                    {
                        infoUser.resetPass(Email, Password);
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


        [HttpGet("ThongKeNguoiOnline")]
        //phương thức get dữ liệu từ firebase
        public IActionResult ThongKeNguoiOnline()
        {
            try
            {
                User infoUser = new User();
                return Ok(infoUser.ThongKeNguoiOnlie());
            }
            catch
            {
                return Ok("Error");
            }
        }
    }
}
