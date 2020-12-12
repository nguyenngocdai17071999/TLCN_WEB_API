using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLCN_WEB_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace TLCN_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config) {
            _config = config;
        }

        public IActionResult Login(string username, string password) {
            UserModel login = new UserModel();
            login.UserName = username;
            login.PassWord = password;
            IActionResult response = Unauthorized();

            var user = AuthenticationUser(login);
            if (user != null) {
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });

            }
            return response;
        }
        [Authorize]
        [HttpPost("Post")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            DateTime username = DateTime.Parse(claim[0].Value);

            TimeSpan Time = username - DateTime.Now;
            int sophut1 = username.Minute;
            int sophut2 = DateTime.Now.Minute;
            if (sophut2 < sophut1)
                sophut2 = sophut2 + 60;
            int ketqua = sophut2 - sophut1;
            return "welcome to: " +ketqua+" "+username;
        }

        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value1", "Value2", "Value3" };
        }


        private UserModel AuthenticationUser(UserModel login) {
            UserModel user = null;
            if(login.UserName=="dai"&& login.PassWord=="123456" )
            {
               user = new UserModel { UserName = "dai", EmailAddress = "dai", PassWord = "123" };
            }
            return user;
        }
        private string GenerateJSONWebToken(UserModel userinfo) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;

        }
    }
}
