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
        // GET: Admin
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

        public ActionResult Xe(int? page)
        {
            int pageNum = (page?? 1);
            int pageSzie = 7;

            return View(data.Xes.ToList().OrderBy(a=>a.idXe).ToPagedList(pageNum,pageSzie));
        }

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

        public ActionResult Details(int id)
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
        public ActionResult Delete(int id)
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
        [HttpPost,ActionName("Delete")]
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

        //phần edit không edit được
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



       



    }
}