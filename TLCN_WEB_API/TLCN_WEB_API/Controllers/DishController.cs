using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    public class DishController : ControllerBase
    {
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll()
        {
            try
            {
                Dish dish = new Dish();
                return Ok(dish.getAll());
            }
            catch
            {
                return Ok("Error");
            }

        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id)
        {
            try
            {
                Dish dish = new Dish();
                return Ok(dish.getByID(id));
            }
            catch
            {
                return Ok("Error");
            }

        }

        [HttpGet("GetByIDMenu")]
        // phương thức get by id menu dữ liệu từ firebase 
        public IActionResult GetByIDMenu(string id)
        {
            try
            {
                Dish dish = new Dish();
                return Ok(dish.getByIDMenu(id));
            }
            catch
            {
                return Ok("Error");
            }

        }

        [HttpGet("Search")]
        //phương thức get dữ liệu từ firebase
        public IActionResult Search(string dishname)
        {
            try
            {
                if (dishname != null)
                {
                    Dish dish = new Dish();
                    return Ok(dish.search(dishname));
                }
                return Ok("Không có kết quả tìm kiếm");
            }
            catch
            {
                return Ok("Error");
            }

        }

        [HttpGet("GetByIDType")]
        // phương thức get by id menu dữ liệu từ firebase 
        public IActionResult GetByIDType(string id)
        {
            try
            {
                Dish dish = new Dish();
                return Ok(dish.getbyIdType(id));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Dish dish)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (userinfo.checkAdmin(Email) == true)
                    {
                        try
                        {
                            Dish dish1 = new Dish();
                            dish1.AddbyidToFireBase(id, dish);
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch
                        {
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult DeleteByID(string id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (userinfo.checkAdmin(Email)==true || userinfo.checkOwner(Email) == true)
                    {
                        try
                        {
                            Dish dish = new Dish();
                            dish.Delete(id);
                            return Ok(new[] { "Xóa thành công" });
                        }
                        catch
                        {
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok("Error");
            }
        }


        [Authorize]
        [HttpPost("CreateDish")]
        public IActionResult RegisterDish([FromBody] Dish dish)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User userinfo = new User();
                if (userinfo.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (userinfo.checkAdmin(Email)==true)
                    {
                        string err = "";
                        try
                        {
                            Dish dish1 = new Dish();
                            dish1.AddToFireBase(dish);
                            err = "Đăng ký thành công";
                        }
                        catch
                        {
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok("Bạn không có quyền");
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok("Error");
            }
        }
    }
}
