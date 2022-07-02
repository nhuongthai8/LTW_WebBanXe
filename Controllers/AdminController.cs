using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom2_WebsiteBanXe.Models;
using PagedList;
using PagedList.Mvc;

namespace Nhom2_WebsiteBanXe.Controllers
{
    public class AdminController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Vui lòng điền tài khoản";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Vui lòng nhập mật khẩu";
            }
            else
            {
                Admin admin = data.Admins.SingleOrDefault(a => a.tkAdmin == tendn && a.passAdmin == matkhau);
                if(admin != null)
                {
                    Session["Taikhoanadmin"] = admin;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.ThongBao = "Sai tài khoản hoặc mật khẩu";
            }
            return View();
        }

        //trang quản lý sản phẩm
        public ActionResult Xe(int? page)
        {
            int pageNum = (page?? 1);
            int pageSzie = 7;

            return View(data.Xes.ToList().OrderBy(a=>a.idXe).ToPagedList(pageNum,pageSzie));
        }

        //------------------------------------------------------------------------------------------------------------------------------
        //Chức năng thêm, xóa, sửa, xem chi tiết sản phẩm
        [HttpGet]
        public ActionResult ThemXeMoi()
        {
            ViewBag.idLoaixe = new SelectList(data.LoaiXes.ToList().OrderBy(a => a.TenLoai), "idLoaixe", "TenLoai");
            ViewBag.idHangxe = new SelectList(data.HangXes.ToList().OrderBy(b => b.TenHangXe), "idHangxe", "TenHangXe");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemXeMoi(Xe xe, HttpPostedFileBase fileupload)
        {
            ViewBag.idLoaixe = new SelectList(data.LoaiXes.ToList().OrderBy(a => a.TenLoai), "idLoaixe", "TenLoai");
            ViewBag.idHangxe = new SelectList(data.HangXes.ToList().OrderBy(b => b.TenHangXe), "idHangxe", "TenHangXe");

            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhSP"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    xe.Anhbia = filename;
                    data.Xes.InsertOnSubmit(xe);
                    data.SubmitChanges();
                }
                return RedirectToAction("Xe");
            }          
            
        }

        public ActionResult DetailsXe(int id)
        {
            Xe xe = data.Xes.SingleOrDefault(a => a.idXe == id);
            ViewBag.idXe = xe.idXe;
            if(xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);
        }

        [HttpGet]
        public ActionResult DeleteXe(int id)
        {
            Xe xe = data.Xes.SingleOrDefault(a => a.idXe == id);
            ViewBag.idXe = xe.idXe;
            if(xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);
        }
        [HttpPost,ActionName("DeleteXe")]
        public ActionResult ConfirmDelete(int id)
        {
            Xe xe = data.Xes.SingleOrDefault(a => a.idXe == id);
            ViewBag.idXe = xe.idXe;
            if(xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Xes.DeleteOnSubmit(xe);
            data.SubmitChanges();
            return RedirectToAction("Xe");
        }

        [HttpGet]
        public ActionResult EditXe(int id)
        {
            Xe xe = data.Xes.SingleOrDefault(a => a.idXe == id);
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idLoaixe = new SelectList(data.LoaiXes.ToList().OrderBy(a => a.TenLoai), "idLoaixe", "TenLoai", xe.idLoaiXe);
            ViewBag.idHangxe = new SelectList(data.HangXes.ToList().OrderBy(b => b.TenHangXe), "idHangxe", "TenHangXe", xe.idHangXe);
            return View(xe);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditXe(int id, HttpPostedFileBase fileUpload)
        {
            var sp = data.Xes.FirstOrDefault(a=>a.idXe == id);
            sp.idXe = id;
            ViewBag.idLoaixe = new SelectList(data.LoaiXes.ToList().OrderBy(a => a.TenLoai), "idLoaixe", "TenLoai");
            ViewBag.idHangxe = new SelectList(data.HangXes.ToList().OrderBy(b => b.TenHangXe), "idHangxe", "TenHangXe");
            if(fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View(sp);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhSP"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.ThongBao = "Ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sp.Anhbia = fileName;
                    sp.idXe = id;
                    UpdateModel(sp);
                    data.SubmitChanges();
                    return RedirectToAction("Xe");
                }
                return this.EditXe(id);
            }
            
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý loại xe
        public ActionResult LoaiXe()
        {
            return View(data.LoaiXes.ToList().OrderBy(a=>a.idLoaiXe));
        }
        //Chức năng thêm, xóa, sửa loại xe
        //thêm
        [HttpGet]
        public ActionResult ThemLX()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemLX(LoaiXe lx)
        {
            data.LoaiXes.InsertOnSubmit(lx);
            data.SubmitChanges();
            return RedirectToAction("LoaiXe");
        }
        //sửa
        [HttpGet]
        public ActionResult SuaLX(int id)
        {
            LoaiXe lx = data.LoaiXes.SingleOrDefault(a=>a.idLoaiXe == id);
            //if (lx == null)
            //{
            //    Response.StatusCode = 404;
            //    return null;
            //}
            return View(lx);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaLx(int id)
        {
            var lx = data.LoaiXes.FirstOrDefault(a => a.idLoaiXe == id);
            lx.idLoaiXe = id;
            if (ModelState.IsValid)
            {
                UpdateModel(lx);
                data.SubmitChanges();
                return RedirectToAction("LoaiXe");
            }
            return this.EditXe(id);
        }
        //xóa
        [HttpGet]
        public ActionResult XoaLX(int id)
        {
            LoaiXe lx = data.LoaiXes.SingleOrDefault(a => a.idLoaiXe == id);

            return View(lx);
        }
        [HttpPost,ActionName("XoaLX")]
        public ActionResult ConfirmXoaLX(int id)
        {
            LoaiXe lx = data.LoaiXes.SingleOrDefault(a => a.idLoaiXe == id);
            ViewBag.idLoaiXe = lx.idLoaiXe;
            if (lx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.LoaiXes.DeleteOnSubmit(lx);
            data.SubmitChanges();
            return RedirectToAction("LoaiXe");
        }
        //xem
        public ActionResult ChiTietLX(int id)
        {
            LoaiXe lx = data.LoaiXes.SingleOrDefault(a => a.idLoaiXe == id);
            ViewBag.idLoaiXe = lx.idLoaiXe;
            if (lx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lx);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý hãng xe
        public ActionResult HangXe()
        {
            return View(data.HangXes.ToList().OrderBy(a => a.idHangXe));
        }
        //Chức năng xem, thêm, xóa ,sửa hãng xe
        //thêm
        [HttpGet]
        public ActionResult ThemHX()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemHX(HangXe hx)
        {
            data.HangXes.InsertOnSubmit(hx);
            data.SubmitChanges();
            return RedirectToAction("HangXe");
        }
        //sửa
        [HttpGet]
        public ActionResult SuaHX(int id)
        {
            HangXe hx = data.HangXes.SingleOrDefault(a => a.idHangXe == id);
            return View(hx);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaHx(int id)
        {
            var hx = data.HangXes.FirstOrDefault(a => a.idHangXe == id);
            hx.idHangXe = id;
            if (ModelState.IsValid)
            {
                UpdateModel(hx);
                data.SubmitChanges();
                return RedirectToAction("HangXe");
            }
            return this.SuaHX(id);
        }
        //xóa
        [HttpGet]
        public ActionResult XoaHX(int id)
        {
            HangXe hx = data.HangXes.SingleOrDefault(a => a.idHangXe == id);
            return View(hx);
        }
        [HttpPost,ActionName("XoaHX")]
        public ActionResult ConfirmXoaHX(int id)
        {
            HangXe hx = data.HangXes.SingleOrDefault(a => a.idHangXe == id);
            ViewBag.idHangXe = hx.idHangXe;
            if (hx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.HangXes.DeleteOnSubmit(hx);
            data.SubmitChanges();
            return RedirectToAction("HangXe");
        }
        //xem
        public ActionResult ChiTietHX(int id)
        {
            HangXe hx = data.HangXes.SingleOrDefault(a => a.idHangXe == id);
            ViewBag.idHangXe = hx.idHangXe;
            if (hx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hx);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý thông tin khách hàng
        public ActionResult KhachHang()
        {
            return View(data.KhachHangs.ToList().OrderBy(a=>a.MaKH));
        }
        //sửa
        [HttpGet]
        public ActionResult SuaKH(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(a => a.MaKH == id); 
            return View(kh);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaKh(int id)
        {
            var kh = data.KhachHangs.FirstOrDefault(a => a.MaKH == id);
            kh.MaKH = id;
            if (ModelState.IsValid)
            {
                UpdateModel(kh);
                data.SubmitChanges();
                return RedirectToAction("KhachHang");
            }
            return this.SuaKH(id);
        }
        //chi tiết
        public ActionResult ChiTietKH(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(a => a.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        //xóa
        [HttpGet]
        public ActionResult XoaKH(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(a => a.MaKH == id);
            return View(kh);
        }
        [HttpPost,ActionName("XoaKH")]
        public ActionResult XoaKh(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(a => a.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.KhachHangs.DeleteOnSubmit(kh);
            data.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý đơn hàng
        public ActionResult DonHang(int? page)
        {
            int pageNum = (page?? 1);
            int pageSzie = 7;
            return View(data.ChiTietOrders.ToList().OrderBy(a => a.MaOrder).ToPagedList(pageNum, pageSzie));

            //return View(data.ChiTietOrders.ToList().OrderBy(a => a.MaOrder));
        }
        //chi tiết
        public ActionResult ChiTietDH(int id)
        {
            ChiTietOrder ct = data.ChiTietOrders.FirstOrDefault(a => a.MaOrder == id);
            ViewBag.MaOrder = ct.MaOrder;
            //if (ct == null)
            //{
            //    Response.StatusCode = 404;
            //    return null;
            //}
            return View(ct);
        }
        //sửa
        [HttpGet]
        public ActionResult SuaDH(int id)
        {
            ChiTietOrder ct = data.ChiTietOrders.SingleOrDefault(a => a.MaOrder == id);
            return View(ct);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaDh(int id)
        {
            var ct = data.ChiTietOrders.FirstOrDefault(a => a.MaOrder == id);
            ct.MaOrder = id;
            if (ModelState.IsValid)
            {
                UpdateModel(ct);
                data.SubmitChanges();
                return RedirectToAction("DonHang");
            }
            return this.SuaDH(id);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý thể loại tin tức
        public ActionResult TheLoaiTin()
        {
            return View(data.TheLoaiTins.ToList().OrderBy(a=>a.idLoai));
        }
        //thêm
        [HttpGet]
        public ActionResult ThemTLT()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemTLT(TheLoaiTin tlt)
        {
            data.TheLoaiTins.InsertOnSubmit(tlt);
            data.SubmitChanges();
            return RedirectToAction("TheLoaiTin");
        }
        //xóa
        [HttpGet]
        public ActionResult XoaTLT(int id)
        {
            TheLoaiTin tlt = data.TheLoaiTins.SingleOrDefault(a => a.idLoai == id);
            return View(tlt);
        }
        [HttpPost, ActionName("XoaTLT")]
        public ActionResult XoaTlt(int id)
        {
            TheLoaiTin tlt = data.TheLoaiTins.SingleOrDefault(a => a.idLoai == id);
            ViewBag.idLoai = tlt.idLoai;
            if (tlt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.TheLoaiTins.DeleteOnSubmit(tlt);
            data.SubmitChanges();
            return RedirectToAction("TheLoaiTin");
        }
        //sửa
        [HttpGet]
        public ActionResult SuaTLT(int id)
        {
            TheLoaiTin tlt = data.TheLoaiTins.SingleOrDefault(a => a.idLoai == id);
            return View(tlt);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTlt(int id)
        {
            var tlt = data.TheLoaiTins.FirstOrDefault(a => a.idLoai == id);
            tlt.idLoai = id;
            if (ModelState.IsValid)
            {
                UpdateModel(tlt);
                data.SubmitChanges();
                return RedirectToAction("TheLoaiTin");
            }
            return this.SuaTLT(id);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        //Trang quản lý tin tức
        public ActionResult TinTuc()
        {
            return View(data.TinTucs.ToList().OrderBy(a=>a.idTT));
        }
        //thêm
        [HttpGet]
        public ActionResult ThemTT()
        {
            ViewBag.idLoai = new SelectList(data.TheLoaiTins.ToList().OrderBy(b => b.TenLoai), "idLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemTT(TinTuc tt, HttpPostedFileBase fileupload)
        {
            ViewBag.idLoai = new SelectList(data.TheLoaiTins.ToList().OrderBy(b => b.TenLoai), "idLoai", "TenLoai");

            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhTT"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    tt.Anhbia = filename;
                    data.TinTucs.InsertOnSubmit(tt);
                    data.SubmitChanges();
                }
                return RedirectToAction("TinTuc");
            }

        }
        //xóa
        public ActionResult XoaTT(int id)
        {
            TinTuc tt = data.TinTucs.SingleOrDefault(a => a.idTT == id);
            ViewBag.idTT = tt.idTT;
            if (tt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tt);
        }
        //sửa
        [HttpGet]
        public ActionResult SuaTT(int id)
        {
            TinTuc tt = data.TinTucs.SingleOrDefault(a => a.idTT == id);
            if (tt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idLoai = new SelectList(data.TheLoaiTins.ToList().OrderBy(b => b.TenLoai), "idLoai", "TenLoai");
            return View(tt);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTT(int id, HttpPostedFileBase fileUpload)
        {
            var tt = data.TinTucs.FirstOrDefault(a => a.idTT == id);
            tt.idTT = id;
            ViewBag.idLoai = new SelectList(data.TheLoaiTins.ToList().OrderBy(b => b.TenLoai), "idLoai", "TenLoai");
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View(tt);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhTT"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.ThongBao = "Ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    tt.Anhbia = fileName;
                    tt.idTT = id;
                    UpdateModel(tt);
                    data.SubmitChanges();
                    return RedirectToAction("TinTuc");
                }
                return this.SuaTT(id);
            }

        }
    }
}