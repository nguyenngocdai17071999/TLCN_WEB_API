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
    public class DiscountController : Controller
    {
        [HttpGet("GetAll")]                           //Lấy tất cả dữ liệu khuyến mãi
        public IActionResult GetAll(){
            try{
                Discount danhsach = new Discount();   //Khai báo model khuyến mãi
                return Ok(danhsach.getAll());         //Trả về danh sách khuyến mãi
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDDiscountType")]                         //lấy dư liệu khuyến mãi theo id loại khuyến mãi
        public IActionResult GetByIDDiscountType(string id){
            try{
                Discount danhsach = new Discount();              //Khai báo model khuyến mãi
                return Ok(danhsach.getByidDiscountType(id));     //Trả về danh sách quán ăn theo loại hình khuyến mãi
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDStore")]                           //Lấy dữ liệu khuyến mãi theo id quán
        public IActionResult GetByIDStore(string id){
            try{
                Discount danhsach = new Discount();         //Khai báo model khuyến mãi
                return Ok(danhsach.getByidStore(id));       //Trả về danh sách khuyến mãi theo id quán
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                              //Chỉnh sửa khuyến mãi truyền vào id khuyến mãi và body model khuyến mãi
        public IActionResult EditByID(string id, [FromBody] Discount discount){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                 //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                              //Danh sách các biến trong identity
                string Email = claim[1].Value;                                              //Email của token             
                User infoUser = new User();                                                 //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){ //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                //Kiểm tra có phải admin không
                        Discount discount1 = new Discount();                                //Khai báo biến Model Discount 
                        discount1.AddbyidToFireBase(id, discount);                          //Update data
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
        [HttpPost("DeleteByID")]                                                                //Xóa khuyến mãi truyền vào id quán id loại khuyến mãi
        public IActionResult DeleteByID(string idstore, string iddiscounttype){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true||infoUser.checkOwner(Email)==true){  //Kiểm tra có phải admin hoặc owner không
                        Discount discount = new Discount();                                     //Khai báo biến Model Discount 
                        discount.Delete(discount.getiddiscount(idstore,iddiscounttype));        //Xóa data
                        return Ok(new[] { "Xóa thành công" });
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
        [HttpPost("CreateDiscount")]
        public IActionResult RegisterDiscount([FromBody] Discount discount){                     //Tạo khuyến mãi cho quán
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                      //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                   //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                   //Email của token             
                User infoUser = new User();                                                      //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){      //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true||infoUser.checkOwner(Email)==true){   //Kiểm tra có phải admin hoặc owner không
                        Discount discount1 = new Discount();                                     //Khai báo biến Model Discount 
                        discount1.AddToFireBase(discount);                                       //Thêm data
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
    }
}
