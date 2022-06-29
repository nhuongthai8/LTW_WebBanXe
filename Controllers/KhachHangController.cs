using Nhom2_WebsiteBanXe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom2_WebsiteBanXe.Controllers
{
    public class KhachHangController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["HoTenKH"];
            var gioitinh = collection["GioitinhKH"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
            var sdt = collection["SdtKH"];
            var diachi = collection["DiachiKH"];
            var email = collection["eMailKH"];
            var tendangnhap = collection["TendangnhapKH"];
            //var matkhau = collection["MatkhauKH"];
            //mã hóa MD5
            var matkhau = MaHoa.GetMD5(collection["MatkhauKH"]);
            //
            var matkhaunl = collection["Matkhaunl"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(gioitinh))
            {
                ViewData["Loi2"] = "Giới tính không được để trống";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewData["Loi3"] = "Số điện thoại không được để trống";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi4"] = "Địa chỉ không được để trống";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được để trống";
            }
            else if (String.IsNullOrEmpty(tendangnhap))
            {
                ViewData["Loi6"] = "Tên đăng nhập không được để trống";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi7"] = "Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewData["Loi8"] = "Vui lòng nhập lại mật khẩu";
            }
            else
            {
                kh.HoTen = hoten;
                kh.GioiTinh = gioitinh;
                kh.NgaySinh = DateTime.Parse(ngaysinh);
                kh.SDT = sdt;
                kh.DiaChi = diachi;
                kh.eMail = email;
                kh.TaiKhoan = tendangnhap;
                //kh.MatKhau = matkhau;
                kh.MatKhau = MaHoa.GetMD5(matkhau);
                //
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TendangnhapKH"];
            //var mk = collection["MatkhauKH"];
            var mk = MaHoa.GetMD5(collection["MatkhauKH"]);
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Vui lòng nhập Tài khoản";
            }
            else if (String.IsNullOrEmpty(mk))
            {
                ViewData["Loi2"] = "Vui lòng nhập Mật khẩu";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == MaHoa.GetMD5(mk));
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "CarStore");
                }
                else
                    ViewBag.Thongbao = "Sai tài khoản hoặc mật khẩu";
            }
            return View();
        }
    }
}