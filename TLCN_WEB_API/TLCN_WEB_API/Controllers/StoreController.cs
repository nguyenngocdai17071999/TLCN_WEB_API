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
    public class StoreController : ControllerBase
    {       
        [HttpGet("GetAll")]                                      //lấy dữ liệu danh sách các quán ăn có kiểm tra live
        public IActionResult GetAll(double Lat, double Long){    
            try{
                Store store = new Store();
                return Ok(store.getAll(Lat,Long));
            }
            catch{
                return Ok("Error");
            }           
        }
        
        [HttpGet("TangViewWebsite")]                                      //tăng view Website
        public IActionResult TangViewWebsite(double Lat, double Long){    
            try{
                Store store = new Store();
                store.ViewWebsite();
                return Ok("OK");
            }
            catch{
                return Ok("Error");
            }           
        } 

        [HttpGet("GetAllManage")]                                        //Lấy dữ liệu danh sách tất cả quán ăn k kiểm tra live
        public IActionResult GetAllmanage(double Lat, double Long){
            try{
                Store store = new Store();
                return Ok(store.getAll(Lat,Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetAllGanToi")]                                          //Danh sách quán ăn được sắp xếp gần Uơ xa
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

        [HttpGet("GetAllGanToiProvince")]                                       //Danh sách quán ăn gần tôi theo Thành phố
        public IActionResult GetAllGanToi(string id, double Lat, double Long)    //IdProvince
        {
            try{
                Store store = new Store();
                return Ok(store.getAllGanToiProvince(id,Lat,Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDOwner")]                                                  //danh sách quán ăn của chủ quán
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


        [HttpGet("ThongKeTheoNgay")]                                                      //thống kế lượt view của quán theo từng ngày
        public IActionResult ThongKeTheoNgay(string id,int ngay, int thang, int nam)      //IdStore
        {
            try{
                View_Store view_Store = new View_Store();                                 //Model View_Store
                var danhsachView= view_Store.getByIDStore(id);                            //Danh sách lượt xem
                int[] thongkengay = new int[24];                                          //Mãng 24 giờ trong 1 ngày
                foreach(var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);                               //thời gian của view
                    TimeSpan Time = a.TimeOfDay;                                          //Biến timespan giúp phân chia hours, day, month, year
                    for(int i = 0; i < 24; i++){
                        if (i == Time.Hours && ngay == a.Day && thang == a.Month && nam == a.Year) //kiểm tra đúng ngày đúng giờ thêm view
                            thongkengay[i] = thongkengay[i] + 1;
                    }
                }
                return Ok(thongkengay);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoTinh")]                                                         //thống kê số quán của tỉnh
        public IActionResult ThongKeTheoTinh(){
            try{
                Province province = new Province();
                var danhsachtinh = province.getAll();                                          //Danh sách tỉnh
                List<BienThongKeTinh> bienThongKeTinhs = new List<BienThongKeTinh>();          //Danh sách thống kê số quán
                foreach(var item in danhsachtinh){
                    BienThongKeTinh bienThongKes = new BienThongKeTinh();
                    bienThongKes.ProvinceID = item.ProvinceID;
                    bienThongKes.ProvinceName = item.ProvinceName;
                    bienThongKeTinhs.Add(bienThongKes);                                         // thêm dữ liệu vào danh sách
                }
                Store store = new Store();
                var danhsachquan = store.getAll(0,0);                                           //danh sách quán ăn
                foreach(var item in bienThongKeTinhs){
                    int dem = 0;
                    foreach(var itemquan in danhsachquan){
                        if (itemquan.ProvinceID == item.ProvinceID) dem++;                      //nếu quán có IDprovince trùng thì đếm số quán
                    }
                    item.SoQuan = dem;
                }    
                return Ok(bienThongKeTinhs);                                                    //Danh sách thống kê
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoQuan")]                                                           //thống kê quán theo từng quận
        public IActionResult ThongKeTheoQuan(string ProvinceID){
            try{
                District district = new District();                                               //danh sách quận
                var danhsachquan = district.getByIDProvince(ProvinceID);
                List<BienThongKeQuan> bienThongKeQuans = new List<BienThongKeQuan>();             //Danh sách thống kê số quán của quận
                foreach (var item in danhsachquan){
                    BienThongKeQuan bienThongKes = new BienThongKeQuan();
                    bienThongKes.DistrictID = item.DistrictID;
                    bienThongKes.DistrictName = item.DistrictName;
                    bienThongKeQuans.Add(bienThongKes);                                           //Thêm thông tin quận vào danh sách thống kế
                }
                Store store = new Store();
                var danhsachquanan = store.getAll(0, 0);
                foreach (var item in bienThongKeQuans){
                    int dem = 0;
                    foreach (var itemquan in danhsachquanan){
                        if (itemquan.DistrictID == item.DistrictID) dem++;                         //Nếu trùng IDDistrict tăng biến đếm số quán trong quận
                    }
                    item.SoQuan = dem;
                }
                return Ok(bienThongKeQuans);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoBusinessType")]                                                        //Thống kê số quán theo từng loại mô hình quán
        public IActionResult ThongKeTheoBusinessType(){
            try{
                BusinessType businessType = new BusinessType();                                     //Danh sách loại kinh doanh
                var danhsachbusinessType = businessType.getAll();
                List<BienThongKeBusinessType> bienThongKeQuans = new List<BienThongKeBusinessType>();
                foreach (var item in danhsachbusinessType){
                    BienThongKeBusinessType bienThongKes = new BienThongKeBusinessType();
                    bienThongKes.BusinessTypeID = item.BusinessTypeID;
                    bienThongKes.BusinessTypetName = item.BusinessTypeName;
                    bienThongKeQuans.Add(bienThongKes);                                             // Thêm thông tin loại kinh doanh vào danh sách thống kê
                }
                Store store = new Store();
                var danhsachquanan = store.getAll(0, 0);
                foreach (var item in bienThongKeQuans){
                    int dem = 0;
                    foreach (var itemquan in danhsachquanan){
                        if (itemquan.BusinessTypeID == item.BusinessTypeID) dem++;                 // Nếu BusinessTypeID trùng biến đếm tăng 
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

        [HttpGet("ThongKeTheoNgayAll")]                                           //thống kê view theo ngày của website
        public IActionResult ThongKeTheoNgayAll(int ngay, int thang, int nam){
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int[] thongkengay = new int[24];                                   //mảng 24 giờ của 1 ngày
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    TimeSpan Time = a.TimeOfDay;
                    for (int i = 0; i < 24; i++){
                        if (i == a.Hour && ngay == a.Day && thang == a.Month && nam == a.Year)   //Nếu trùng giờ thì số view sẽ tăng lên
                            thongkengay[i] = thongkengay[i] + 1;
                    }
                }
                return Ok(thongkengay);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeView")]                                       //Thống kê số view của website
        public IActionResult ThongKeView(){
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();                
                return Ok(danhsachView.Count);                         //trả về 1 con số hiển thị số view của website
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("Nam")]                                              // danh sách năm của view website để hiển thị chọn báo cáo
        public IActionResult Nam(){
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();                 //danh sách view
                List<int> danhsachnam = new List<int>();
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    int dem = 0;
                    for (int i=0;i<danhsachnam.Count; i++){                        
                        if (danhsachnam[i] == a.Year){
                            dem++;                            
                        }                        
                    }
                    if (dem == 0) danhsachnam.Add(a.Year);                  //nếu năm này chưa thêm vào danh sách thì add
                    if (danhsachnam.Count==0) danhsachnam.Add(a.Year);       // nếu danh sách năm chưa có thêm năm đầu tiên cho phần tử số 0
                }
                return Ok(danhsachnam);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoThang")]                                      //thống kê lượt view theo tháng
        public IActionResult ThongKeTheoThang(string id,int thang, int nam,double Lat, double Long){    //IDStore
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getByIDStore(id);               //Danh sach quán ăn
                int songay = view_Store.fun(thang, nam);                      //số ngày của tháng
                int[] thongkethang = new int[songay];                         //mang số ngày trong tháng
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < songay; i++){
                        if (i == a.Day-1 && thang == a.Month && nam == a.Year) //nếu trùng thời gian thì tăng lượt đếm view
                            thongkethang[i] = thongkethang[i] + 1;             //(i=a.day-1 vì i bắt đầu = 0 ngày bắt đầu bằng 1)
                    }
                }
                return Ok(thongkethang);
            }
            catch{
                return Ok("Error");
            }
        }
        
        [HttpGet("ThongKeTheoThangAll")]                                    //thống kê lượt view theo tháng website
        public IActionResult ThongKeTheoThangAll(int thang, int nam){
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int songay = view_Store.fun(thang, nam);                      //Số ngày của tháng
                int[] thongkethang = new int[songay];
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < songay; i++){
                        if (i == a.Day-1 && thang == a.Month && nam == a.Year)    
                            thongkethang[i] = thongkethang[i] + 1;
                    }
                }
                return Ok(thongkethang);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoNam")]                                             //Thống kê lượt view theo năm của quán
        public IActionResult ThongKeTheoNam(string id, int nam, double Lat, double Long){        //IDStore
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getByIDStore(id);
                int[] thongkenam = new int[12];                        //1 năm 12 tháng, mảng tháng trong năm
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < 12; i++){
                        if (i == a.Month-1&&nam==a.Year)
                            thongkenam[i] = thongkenam[i] + 1;
                    }
                }
                return Ok(thongkenam);
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("ThongKeTheoNamAll")]                                   //Thống kê lượt view của website
        public IActionResult ThongKeTheoNamAll(int nam){
            try{
                View_Store view_Store = new View_Store();
                var danhsachView = view_Store.getAll();
                int[] thongkenam = new int[12];                          //1 năm 12 tháng, mảng tháng trong năm
                foreach (var item in danhsachView){
                    DateTime a = DateTime.Parse(item.Date);
                    for (int i = 0; i < 12; i++){
                        if (i == a.Month - 1 && nam == a.Year)
                            thongkenam[i] = thongkenam[i] + 1;
                    }
                }
                return Ok(thongkenam);                                  
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDBusinessType")]                                       // Lấy danh sách quán ăn theo mô thình kinh doanh
        public IActionResult GetByIDBusinessType(string id, double Lat, double Long){//IDBusinessType
            try{
                Store store = new Store();
                return Ok(store.getByIDBusinessType(id,Lat,Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDDistrict")]                                              //Lấy danh sách quán theo quận 
        public IActionResult GetByIDDistrict(string id, double Lat, double Long){    //IDDistrict
            try{
                Store store = new Store();
                return Ok(store.getByIDDistrict(id, Lat, Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByID")]                                                //lấy thông tin của quán 
        public IActionResult GetByID(string id, double Lat, double Long){    //IDStore có check live
            try{
                Store store = new Store();
                return Ok(store.getByID(id, Lat, Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDManage")]                                                 //lấy thông tin của quán cho admin
        public IActionResult GetByIDManage(string id, double Lat, double Long){     //IDStore không check live
            try{
                Store store = new Store();
                return Ok(store.getByIDManage(id, Lat, Long));
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpGet("GetByIDProvince")]                                           //Lấy danh sách quán ăn theo tỉnh
        public IActionResult GetByIDProvince(string id,string id2, double Lat, double Long){   //IDProvince
            try{
                Store store = new Store();
                return Ok(store.getByIDProvince(id,id2, Lat, Long));
            }
            catch{
                return Ok("Error");
            }            
        }

        [Authorize]
        [HttpPost("EditByID")]                                                       //Chỉnh sửa thông tin quán ăn
        public IActionResult EditByID(string id, [FromBody] Store store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                         //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                      //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                      //Email của token             
                User infoUser = new User();                                                         //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){         //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true || infoUser.checkOwner(Email) == true){    //Kiểm tra có phải admin, owner không
                        try{
                            Store store2 = new Store();
                            store2.AddbyidToFireBase(id, store);                                    //Update data
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

        [HttpGet("TongViewQuan")]                                       // cập nhật điểm đánh giá cho quán
        public IActionResult TongViewQuan(string id)
        {  //IDStore, Ratepoint
            try
            {
                View_Store view_Store = new View_Store();
                var tongview = view_Store.getByIDStore(id);
                return Ok(tongview.Count);
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpGet("ListRate")]                                       // cập nhật điểm đánh giá cho quán
        public IActionResult Danhsachrate(string id)
        {  //IDStore, Ratepoint
            try
            {
                Store store2 = new Store();
                return Ok(store2.danhsachrate(id));
            }
            catch
            {
                return Ok("Error");
            }
        }

        [HttpPost("UpdateRatePoint")]                                       // cập nhật điểm đánh giá cho quán
        public IActionResult UpdateRatePoint(string id){  //IDStore, Ratepoint
            try{
                Store store2 = new Store();
                double a = store2.updateRatePoint(id);
                return Ok(a);
            }
            catch{
                return Ok("Error");
            }
        }

        [Authorize]
        [HttpPost("DeleteByID")]                                           // Xóa quán ăn
        public IActionResult deleteByID(string id){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                             //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                          //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                          //Email của token             
                User infoUser = new User();                                                             //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){             //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email)==true || infoUser.checkOwner(Email)==true){          //Kiểm tra có phải admin, owner không
                        try{
                            Store store2 = new Store();
                            store2.Delete(id);                                                          //Update data
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
        [HttpPost("ChangeStatusStore")]                                      // thay đổi trạng thái của quán ăn
        public IActionResult BlockStore(string id, string status){
            try{                                                                                    //khai báo biến danh tính của token
                var identity = HttpContext.User.Identity as ClaimsIdentity;                         //Danh sách các biến trong identity
                IList<Claim> claim = identity.Claims.ToList();                                      //Email của token             
                string Email = claim[1].Value;                                                      //Khai bao biến thông tin người dùng
                User infoUser = new User();                                                         //kiểm tra thời gian đăng nhập còn không
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){         //Kiểm tra có phải admin không
                    if (infoUser.checkAdmin(Email)==true){
                        try{
                            Store store2 = new Store();                                             
                            store2.blockStore(id, status);                                          //Update data
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
        [HttpPost("CreateStore")]                                                    // Thêm quán ăn
        public IActionResult RegisterStore( [FromBody] Store store){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                      //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                   //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                   //Email của token             
                User infoUser = new User();                                                      //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){      //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                     //Kiểm tra có phải admin không
                        string err = "";
                        try{
                            Store store2 = new Store();
                            store2.AddToFireBase(store);                                         //Create data
                            err = "Đăng ký thành công";
                        }
                        catch{
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }           
            
        }

        [HttpGet("getAllCheck")]                                                                //Lấy danh sách quán cần xác nhận lên live
        public IActionResult getAllCheck(double lat, double log){
            try{
                var identity = HttpContext.User.Identity as ClaimsIdentity;                     //khai báo biến danh tính của token
                IList<Claim> claim = identity.Claims.ToList();                                  //Danh sách các biến trong identity
                string Email = claim[1].Value;                                                  //Email của token             
                User infoUser = new User();                                                     //Khai bao biến thông tin người dùng
                if (infoUser.kiemtrathoigianlogin(DateTime.Parse(claim[0].Value)) == true){     //kiểm tra thời gian đăng nhập còn không
                    if (infoUser.checkAdmin(Email) == true){                                    //Kiểm tra có phải admin không
                        string err = "";
                        try{
                            Store store2 = new Store();
                            return Ok(store2.getAllCheck(lat,log));
                        }
                        catch{
                            err = "Lỗi rồi";
                        }
                        return Ok(new[] { err });
                    }
                    return Ok(new[] { "Bạn không có quyền" });
                }
                else return Ok(new[] { "Bạn cần đăng nhập" });
            }
            catch{
                return Ok("Error");
            }
        }

        [HttpPost("CreateStoreOwner")]                                              //đăng kí quán ăn mới chưa được lên live
        public IActionResult RegisterStoreOwner([FromBody] Store store){
            try{
                string err;
                  try{
                        Store store2 = new Store();
                        store2.AddToFireBase(store);
                        err = "Đăng ký thành công";
                  }
                  catch{
                        err = "Lỗi rồi";
                  }
                  return Ok(new[] { err });                 
            }
            catch{
                return Ok("Error");
            }
        }
    }
}
