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
    //[EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {       
        [HttpGet("GetAll")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAll(){
            try{
                Store store = new Store();
                return Ok(store.getAll());
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetAllManage")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllmanage()
        {
            try
            {
                Store store = new Store();
                return Ok(store.getAll());
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToi")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(){
            try{
                Store store = new Store();
                return Ok(store.getAllGanToi());
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToiProvince")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(string id){
            try{
                Store store = new Store();
                return Ok(store.getAllGanToiProvince(id));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDOwner")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDOwner(string id){
            try{
                Store store = new Store();
                return Ok(store.getByIDOwner(id));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id){
            try{
                Store store = new Store();
                return Ok(store.getByID(id));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDManage")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDManage(string id)
        {
            try
            {
                Store store = new Store();
                return Ok(store.getByIDManage(id));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDProvince")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDProvince(string id,string id2){
            try{
                Store store = new Store();
                return Ok(store.getByIDProvince(id,id2));
            }
            catch{
                return Ok("Error");
            }            
        }

        [HttpGet("GetByIDBusinessType")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDBusinessType(string id){
            try{
                Store store = new Store();
                return Ok(store.getByIDBusinessType(id));
            }
            catch{
                return Ok("Error");
            }           
        }

        [Authorize]
        [HttpPost("EditByID")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult EditByID(string id, [FromBody] Store store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true || infoUser.checkOwner(Email) == true)
                    {
                        try{
                            Store store2 = new Store();
                            store2.AddbyidToFireBase(id, store);
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }                 
        }


        [HttpPost("UpdateRatePoint")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult UpdateRatePoint(string id, string RatePoint)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    Store store2 = new Store();
                    store2.updateRatePoint(id, RatePoint);
                    return Ok("Cập nhật thành công");
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
        public IActionResult deleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true || infoUser.checkOwner(Email)==true){
                        try{
                            Store store2 = new Store();
                            store2.Delete(id);
                            return Ok(new[] { "Xóa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
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
        [HttpPost("ChangeStatusStore")]
        //thay đổi thông tin đã có trên firebase theo id
        public IActionResult BlockStore(string id, string status){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                    if (infoUser.checkAdmin(Email)==true)
                    {
                        try{
                            Store store2 = new Store();
                            store2.blockStore(id, status);
                            return Ok(new[] { "sửa thành công" });
                        }
                        catch{
                            return Ok(new[] { "Lỗi rồi" });
                        }
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }


        //[Authorize]
        [HttpPost("CreateStore")]
        public IActionResult RegisterStore( [FromBody] Store store){
            try{
                Store store2 = new Store();
                //store.Status = "3";
                store2.AddToFireBase(store);
                return Ok("oke");
                //var identity = HttpContext.User.Identity as ClaimsIdentity;
                //IList<Claim> claim = identity.Claims.ToList();
                //string Email = claim[1].Value;
                //User infoUser = new User();
                //if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){
                //    if (infoUser.checkAdmin(Email)==true)
                //    {
                //        string err = "";
                //        try{
                //            Store store2 = new Store();
                //            //store.Status = "3";
                //            store2.AddToFireBase(store);
                //            err = "Đăng ký thành công";
                //        }
                //        catch{
                //            err = "Lỗi rồi";
                //        }
                //        return Ok(new[] { err });
                //    }
                //    return Ok(new[] { "Bạn không có quyền" });
                //}
                //else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch
            {
                return Ok("Error");
            }           
            
        } 
    }
}
