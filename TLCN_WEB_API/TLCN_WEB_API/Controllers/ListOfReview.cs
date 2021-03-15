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
    public class ListOfReview : ControllerBase
    {
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                listOfReviews danhsach = new listOfReviews();
                return Ok(danhsach.getAll());
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetByID")]
        // phương thức get by id store dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                listOfReviews danhsach = new listOfReviews();
                return Ok(danhsach.getByID(id));
            }
            catch{
                return Ok("Error");
            }

        }

        [HttpGet("GetByIDComment")]
        // phương thức get by id store dữ liệu từ firebase 
        public IActionResult GetByIDcomment(string id)
        {
            try
            {
                listOfReviews danhsach = new listOfReviews();
                return Ok(danhsach.getByIDComment(id));
            }
            catch
            {
                return Ok("Error");
            }

        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] listOfReviews listreview){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true)
                    {
                        listOfReviews danhsach = new listOfReviews();
                        danhsach.AddbyidToFireBase(id, listreview);
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
                        listOfReviews danhsach = new listOfReviews();
                        danhsach.Delete(id);
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
        [HttpPost("CreateListOfReviews")]
        public IActionResult RegisterBusinessType( [FromBody] listOfReviews list){
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    listOfReviews danhsach = new listOfReviews();
                    danhsach.AddToFireBase(list);
                    err = "Đăng ký thành công";                    
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
