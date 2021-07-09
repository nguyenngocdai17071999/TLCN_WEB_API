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
    public class DiscountDishController : Controller
    {
        [HttpGet("GetAll")]                                 //Lấy tất cả dữ liệu khuyến mãi món ăn
        public IActionResult GetAll(){
            try{
                DiscountDish danhsach = new DiscountDish(); //Khai báo model khuyến mãi món ăn
                return Ok(danhsach.getAll());               //Trả về danh sách khuyến mãi món ăn
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                 //lấy dư liệu khuyến mãi theo id loại khuyến mãi món ăn
        public IActionResult GetByID(string id){
            try{
                DiscountDish danhsach = new DiscountDish();  //Khai báo model khuyến mãi món ăn
                return Ok(danhsach.getByID(id));             //Trả về danh sách quán ăn theo loại hình khuyến mãi món ăn
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]                                                                      //Chỉnh sửa khuyến mãi truyền vào id khuyến mãi dish và body model khuyến mãi dish
        public IActionResult EditByID(string id, [FromBody] DiscountDish discountDish){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                         //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                      //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                      //Email của token             
                User infoUser = new User();                                                         //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){         //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true|| infoUser.checkOwner(Email) == true){   //Kiểm tra có phải admin hoặc owener không
                        DiscountDish discountDish1 = new DiscountDish();                            //Khai báo biến Model khuyến mãi món ăn 
                        discountDish1.AddbyidToFireBase(id, discountDish);                          //Update data
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
        [HttpPost("DeleteByID")]                                                                     //Xóa khuyến mãi dish truyền vào id khuyến mãi
        public IActionResult DeleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                          //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                       //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                       //Email của token             
                User infoUser = new User();                                                          //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){          //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true || infoUser.checkOwner(Email) == true){   //Kiểm tra có phải admin hoặc owner không
                        DiscountDish discountDish = new DiscountDish();                              //Khai báo biến Model DiscountDish
                        discountDish.Delete(id);                                                     //Xóa data
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
        [HttpPost("CreateDiscountDish")]
        public IActionResult RegisterDiscountDish([FromBody] DiscountDish discountDish){                 //Tạo khuyến mãi cho món ăn
            string err = "";
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                              //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                           //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                           //Email của token             
                User infoUser = new User();                                                              //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){              //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true || infoUser.checkOwner(Email) == true){       //Kiểm tra có phải admin hoặc owner không
                        DiscountDish discountDish1 = new DiscountDish();                                 //Khai báo biến Model DiscountDish 
                        discountDish1.AddToFireBase(discountDish);                                       //Thêm data
                        err = "Đăng ký thành công";
                    }
                    else
                    {
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
