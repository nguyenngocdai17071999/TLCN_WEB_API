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
        public IActionResult GetAll(double Lat, double Long){
            try{
                Store store = new Store();
                return Ok(store.getAll(Lat,Long));
            }
            catch{
                return Ok("Error");
            }           
        }

        [HttpGet("GetAllManage")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllmanage(double Lat, double Long)
        {
            try
            {
                Store store = new Store();
                return Ok(store.getAll(Lat,Long));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToi")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(double Lat, double Long)
        {
            try{
                Store store = new Store();
                return Ok(store.getAllGanToi(Lat, Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToiProvince")]
        //phương thức get dữ liệu từ firebase
        public IActionResult GetAllGanToi(string id, double Lat, double Long)
        {
            try{
                Store store = new Store();
                return Ok(store.getAllGanToiProvince(id,Lat,Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDOwner")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDOwner(string id, double Lat, double Long)
        {
            try{
                Store store = new Store();
                return Ok(store.getByIDOwner(id,Lat,Long));
            }
            catch{
                return Ok("Error");
            }
        }


        [HttpGet("ThongKeTheoNgay")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoNgay(string id,int ngay, int thang, int nam)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView= view_Store.getByIDStore(id);
                int[] thongkengay = new int[24];
                foreach(var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    TimeSpan Time = a.TimeOfDay;
                    for(int i = 0; i < 24; i++)
                    {
                        if (i == Time.Hours && ngay == a.Day && thang == a.Month && nam == a.Year)
                            thongkengay[i] = thongkengay[i] + 1;
                    }
                }
                return Ok(thongkengay);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoTinh")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoTinh()
        {
            try
            {
                Province province = new Province();
                var danhsachtinh = province.getAll();
                List<BienThongKeTinh> bienThongKeTinhs = new List<BienThongKeTinh>();
                foreach(var item in danhsachtinh)
                {
                    BienThongKeTinh bienThongKes = new BienThongKeTinh();
                    bienThongKes.ProvinceID = item.ProvinceID;
                    bienThongKes.ProvinceName = item.ProvinceName;
                    bienThongKeTinhs.Add(bienThongKes);
                }
                Store store = new Store();
                var danhsachquan = store.getAll(0,0);
                foreach(var item in bienThongKeTinhs)
                {
                    int dem = 0;
                    foreach(var itemquan in danhsachquan)
                    {
                        if (itemquan.ProvinceID == item.ProvinceID) dem++;
                    }
                    item.SoQuan = dem;
                }    
                return Ok(bienThongKeTinhs);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoQuan")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoQuan(string ProvinceID)
        {
            try
            {
                District district = new District();
                var danhsachquan = district.getByIDProvince(ProvinceID);
                List<BienThongKeQuan> bienThongKeQuans = new List<BienThongKeQuan>();
                foreach (var item in danhsachquan)
                {
                    BienThongKeQuan bienThongKes = new BienThongKeQuan();
                    bienThongKes.DistrictID = item.DistrictID;
                    bienThongKes.DistrictName = item.DistrictName;
                    bienThongKeQuans.Add(bienThongKes);
                }
                Store store = new Store();
                var danhsachquanan = store.getAll(0, 0);
                foreach (var item in bienThongKeQuans)
                {
                    int dem = 0;
                    foreach (var itemquan in danhsachquanan)
                    {
                        if (itemquan.DistrictID == item.DistrictID) dem++;
                    }
                    item.SoQuan = dem;
                }
                return Ok(bienThongKeQuans);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoBusinessType")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoBusinessType()
        {
            try
            {
                BusinessType businessType = new BusinessType();
                var danhsachbusinessType = businessType.getAll();
                List<BienThongKeBusinessType> bienThongKeQuans = new List<BienThongKeBusinessType>();
                foreach (var item in danhsachbusinessType)
                {
                    BienThongKeBusinessType bienThongKes = new BienThongKeBusinessType();
                    bienThongKes.BusinessTypeID = item.BusinessTypeID;
                    bienThongKes.BusinessTypetName = item.BusinessTypeName;
                    bienThongKeQuans.Add(bienThongKes);
                }
                Store store = new Store();
                var danhsachquanan = store.getAll(0, 0);
                foreach (var item in bienThongKeQuans)
                {
                    int dem = 0;
                    foreach (var itemquan in danhsachquanan)
                    {
                        if (itemquan.BusinessTypeID == item.BusinessTypeID) dem++;
                    }
                    item.SoQuan = dem;
                }
                return Ok(bienThongKeQuans);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoNgayAll")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoNgayAll(int ngay, int thang, int nam)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int[] thongkengay = new int[24];
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    TimeSpan Time = a.TimeOfDay;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i == a.Hour && ngay == a.Day && thang == a.Month && nam == a.Year)
                            thongkengay[i] = thongkengay[i] + 1;
                    }
                }
                return Ok(thongkengay);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeView")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeView()
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();                
                return Ok(danhsachView.Count);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("Nam")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult Nam()
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                List<int> danhsachnam = new List<int>();
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    int dem = 0;
                    for (int i=0;i<danhsachnam.Count; i++)
                    {                        
                        if (danhsachnam[i] == a.Year)
                        {
                            dem++;                            
                        }                        
                    }
                    if (dem == 0) danhsachnam.Add(a.Year);
                    if (danhsachnam.Count==0) danhsachnam.Add(a.Year);
                }
                return Ok(danhsachnam);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoThang")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoThang(string id,int thang, int nam,double Lat, double Long)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getByIDStore(id);
                int songay = view_Store.fun(thang, nam);
                int[] thongkethang = new int[songay];
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < songay; i++)
                    {
                        if (i == a.Day-1&&thang==a.Month&&nam==a.Year)
                            thongkethang[i] = thongkethang[i] + 1;
                    }
                }
                return Ok(thongkethang);
            }
            catch
            {
                return Ok("Error");
            }
        }

        
        [HttpGet("ThongKeTheoThangAll")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoThangAll(int thang, int nam)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int songay = view_Store.fun(thang, nam);
                int[] thongkethang = new int[songay];
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < songay; i++)
                    {
                        if (i == a.Day-1 && thang == a.Month && nam == a.Year)
                            thongkethang[i] = thongkethang[i] + 1;
                    }
                }
                return Ok(thongkethang);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoNam")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoNam(string id, int nam, double Lat, double Long)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getByIDStore(id);
                int[] thongkenam = new int[12];
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < 12; i++)
                    {
                        if (i == a.Month-1&&nam==a.Year)
                            thongkenam[i] = thongkenam[i] + 1;
                    }
                }
                return Ok(thongkenam);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoNamAll")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult ThongKeTheoNamAll(int nam)
        {
            try
            {
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int[] thongkenam = new int[12];
                foreach (var item in danhsachView)
                {
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < 12; i++)
                    {
                        if (i == a.Month - 1 && nam == a.Year)
                            thongkenam[i] = thongkenam[i] + 1;
                    }
                }
                return Ok(thongkenam);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDBusinessType")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDBusinessType(string id, double Lat, double Long)
        {
            try
            {
                Store store = new Store();
                return Ok(store.getByIDBusinessType(id,Lat,Long));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDDistrict")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDDistrict(string id, double Lat, double Long)
        {
            try
            {
                Store store = new Store();
                return Ok(store.getByIDDistrict(id, Lat, Long));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByID(string id, double Lat, double Long)
        {
            try{
                Store store = new Store();
                return Ok(store.getByID(id, Lat, Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDManage")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDManage(string id, double Lat, double Long)
        {
            try
            {
                Store store = new Store();
                return Ok(store.getByIDManage(id, Lat, Long));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDProvince")]
        // phương thức get by id dữ liệu từ firebase 
        public IActionResult GetByIDProvince(string id,string id2, double Lat, double Long)
        {
            try{
                Store store = new Store();
                return Ok(store.getByIDProvince(id,id2, Lat, Long));
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
                Store store2 = new Store();
                store2.updateRatePoint(id, RatePoint);
                return Ok("Cập nhật thành công");
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


        [Authorize]
        [HttpPost("CreateStore")]
        public IActionResult RegisterStore( [FromBody] Store store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (infoUser.checkAdmin(Email) == true)
                    {
                        string err = "";
                        try
                        {
                            Store store2 = new Store();
                            store2.AddToFireBase(store);
                            err = "Đăng ký thành công";
                        }
                        catch
                        {
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
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



        [HttpGet("getAllCheck")]
        public IActionResult getAllCheck(double lat, double log)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                string Email = claim[1].Value;
                User infoUser = new User();
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true)
                {
                    if (infoUser.checkAdmin(Email) == true)
                    {
                        string err = "";
                        try
                        {
                            Store store2 = new Store();
                            return Ok(store2.getAllCheck(lat,log));
                        }
                        catch
                        {
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
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

        [HttpPost("CreateStoreOwner")]
        public IActionResult RegisterStoreOwner([FromBody] Store store)
        {
            try
            {
                string err;
                  try
                  {
                        Store store2 = new Store();
                        store2.AddToFireBase(store);
                        err = "Đăng ký thành công";
                  }
                  catch
                  {
                        err = "Lỗi rồi";
                  }
                  return Ok(new[] { err });                 
            }
            catch
            {
                return Ok("Error");
            }

        }

    }
}
