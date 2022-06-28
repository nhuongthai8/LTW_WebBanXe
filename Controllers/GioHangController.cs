using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom2_WebsiteBanXe.Models;

namespace Nhom2_WebsiteBanXe.Controllers
{
    public class GioHangController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        //Lấy giỏ hàng
        public List<ShoppingCart> Laygiohang()
        {
            List<ShoppingCart> listGiohang = Session["ShoppingCart"] as List<ShoppingCart>;
            if (listGiohang == null)
            {
                listGiohang = new List<ShoppingCart>();
                Session["ShoppingCart"] = listGiohang;
            }
            return listGiohang;
        }
        //thêm xe vào giỏ hàng
        public ActionResult Themgiohang(int iMaxe, string strURL)
        {
            List<ShoppingCart> listGiohang = Laygiohang();
            ShoppingCart xe = listGiohang.Find(n => n.iMaxe == iMaxe);
            if (xe == null)
            {
                xe = new ShoppingCart(iMaxe);
                listGiohang.Add(xe);
                return Redirect(strURL);
            }
            else
            {
                xe.iSoluong++;
                return Redirect(strURL);
            }
        }
        //tổng số lượng
        private int TongSoLuong()
        {
            int iTSL = 0;
            List<ShoppingCart> listGiohang = Session["ShoppingCart"] as List<ShoppingCart>;
            if (listGiohang != null)
            {
                iTSL = listGiohang.Sum(n => n.iSoluong);
            }
            return iTSL;
        }
        //tính tổng tiền
        private double TongTien()
        {
            double iTT = 0;
            List<ShoppingCart> listGiohang = Session["ShoppingCart"] as List<ShoppingCart>;
            if (listGiohang != null)
            {
                iTT = listGiohang.Sum(n => n.dThanhtien);
            }
            return iTT;
        }

        public ActionResult ConfirmDH()
        {
            return View();
        }

        public ActionResult GioHang()
        {
            List<ShoppingCart> listGiohang = Laygiohang();
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "CarStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(listGiohang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGioHang(int iMaSP)
        {
            List<ShoppingCart> listGiohang = Laygiohang();
            ShoppingCart xe = listGiohang.SingleOrDefault(n => n.iMaxe == iMaSP);
            if (xe != null)
            {
                listGiohang.RemoveAll(n => n.iMaxe == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "CarStore");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult UpdateGioHang(int iMaSP, FormCollection f)
        {
            List<ShoppingCart> listGiohang = Laygiohang();
            ShoppingCart xe = listGiohang.SingleOrDefault(n => n.iMaxe == iMaSP);
            if (xe != null)
            {
                xe.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaAll()
        {
            List<ShoppingCart> listGiohang = Laygiohang();
            listGiohang.Clear();
            return RedirectToAction("Index", "CarStore");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "KhachHang");
            }

            List<ShoppingCart> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(listGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            Order ddh = new Order();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<ShoppingCart> listGiohang = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayTao = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(ngaygiao);
            ddh.TinhTrangThanhToan = false;
            ddh.TinhTrangGH = false;
            data.Orders.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in listGiohang)
            {
                ChiTietOrder ctorder = new ChiTietOrder();
                ctorder.MaOrder = ddh.MaOrder;
                ctorder.idXe = item.iMaxe;
                ctorder.SoLuong = item.iSoluong;
                ctorder.DonGia = (decimal)item.dGiatien;
                data.ChiTietOrders.InsertOnSubmit(ctorder);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("ConfirmDH", "GioHang");
        }
    }
}