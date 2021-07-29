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
    public class DiscountTypeController : Controller
    {
        [HttpGet("GetAll")]                                     //Lấy tất cả dữ liệu loại khuyến mãi
        public IActionResult GetAll(){
            try{
                DiscountType danhsach = new DiscountType();     //Khai báo model loại khuyến mãi
                return Ok(danhsach.getAll());                   //Trả về danh sách loại khuyến mãi
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetAllAdmin")]                                     //Lấy tất cả dữ liệu loại khuyến mãi
        public IActionResult GetAllAdmin()
        {
            try
            {
                DiscountType danhsach = new DiscountType();     //Khai báo model loại khuyến mãi
                return Ok(danhsach.getAllAmin());                   //Trả về danh sách loại khuyến mãi
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetAllOwner")]                                     //Lấy tất cả dữ liệu loại khuyến mãi
        public IActionResult GetAllOwner(string IDStore)
        {
            try
            {
                DiscountType danhsach = new DiscountType();     //Khai báo model loại khuyến mãi
                return Ok(danhsach.getAllOwner(IDStore));                   //Trả về danh sách loại khuyến mãi
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                                  //Lấy tất cả dữ liệu loại khuyến mãi
        public IActionResult GetByID(string id){
            try{
                DiscountType danhsach = new DiscountType();                   //Khai báo model loại khuyến mãi
                return Ok(danhsach.getByID(id));                              //Trả về danh sách loại khuyến mãi
            }
            catch
            {
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                                //Chỉnh sửa loại khuyến mãi truyền vào id loại khuyến mãi  và body model loại khuyến mãi
        public IActionResult EditByID(string id, [FromBody] DiscountType discountType){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                   //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                //Email của token             
                User infoUser = new User();                                                   //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){   //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                  //Kiểm tra có phải admin không
                        DiscountType discountType1 = new DiscountType();                      //Khai báo biến Model loại khuyến mãi
                        discountType1.AddbyidToFireBase(id, discountType);                    //Update data
                        return Ok(new[] { "sửa thành công" });
                    }
                    else
                    {
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
        [HttpPost("DeleteByID")]                                                               //Xóa loại khuyến mãi truyền vào id loại khuyến mãi
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                    //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                 //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                 //Email của token             
                User infoUser = new User();                                                    //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){    //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true || infoUser.checkOwner(Email) == true)
                    {                                   //Kiểm tra có phải admin không
                        DiscountType discountType = new DiscountType();                        //Khai báo biến Model DiscountType
                        discountType.Delete(id);                                               //Xóa data
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
        [HttpPost("Create")]
        public IActionResult Register([FromBody] DiscountType discountType){                    //Tạo loại khuyến mãi
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true ){                                    //Kiểm tra có phải admin không
                        DiscountType discountType1 = new DiscountType();                        //Khai báo biến Model DiscountType
                        discountType.DiscountRule = "Admin";
                        discountType1.AddToFireBase(discountType);                              //Thêm data
                        err = "Đăng ký thành công";
                    }
                    else if (infoUser.checkOwner(Email) == true){                                    //Kiểm tra có phải admin không
                            DiscountType discountType1 = new DiscountType();                        //Khai báo biến Model DiscountType
                            discountType.DiscountRule = "Owner";
                            discountType1.AddToFireBase(discountType);                              //Thêm data
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

        [Authorize]
        [HttpPost("CreateDiscountTypeOwner")]
        public IActionResult RegisterOwner(string IDStore, [FromBody] DiscountType discountType)
        {                    //Tạo loại khuyến mãi
            string err = "";
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {     //kiểm tra thời gian đăng nhập còn không
                     if (infoUser.checkOwner(Email) == true)
                    {                                    //Kiểm tra có phải admin không
                        DiscountType discountType1 = new DiscountType();                        //Khai báo biến Model DiscountType
                        discountType.DiscountRule = "Owner";
                        discountType.IDStore = IDStore;
                        discountType1.AddToFireBaseOwner(IDStore,discountType);                              //Thêm data
                        err = "Đăng ký thành công";
                    }
                    else
                    {
                        err = "Bạn Không có quyền";
                    }
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                err = "Error";
            }
            return Ok(new[] { err });
        }
    }
}
