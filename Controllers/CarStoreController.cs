using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom2_WebsiteBanXe.Models;
using PagedList;

namespace Nhom2_WebsiteBanXe.Controllers
{
    public class CarStoreController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();

        private List<Xe> Layxemoi(int count)
        {
            //sắp xếp xe cập nhật theo ngay(Ngaynhap) top 6
            return data.Xes.OrderByDescending(a => a.NgayNhap).Take(count).ToList();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 6; //số xe mới cập nhật hiện lên trang index
            int pageNum = (page ?? 1); //tạo biến số trang

            var xemoi = Layxemoi(12); //tổng số xe hiện lên trong phần chia trang
            return View(xemoi.ToPagedList(pageNum, pageSize));
        }

        //lấy tất cả xe trong table       
        public ActionResult Sanpham(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);

            var xe = (from s in data.Xes select s).ToList();
            return View(xe.ToPagedList(pageNum, pageSize));
        }
        //

        private List<Xe> Topgia(int count)
        {
            //sắp xếp xe cập nhật theo gia cao nhat top 3
            return data.Xes.OrderByDescending(a => a.GiaTien).Take(count).ToList();
        }

        public ActionResult Laygiacao()
        {
            var giacao = Topgia(3);
            return PartialView(giacao);
        }

        private List<Xe> Tonkho(int count)
        {
            return data.Xes.OrderByDescending(a => a.SoLuongTon > 2).Take(count).ToList();
        }

        public ActionResult Xetonkho()
        {
            var tonkho = Tonkho(3);
            return PartialView(tonkho);
        }



        //truyền các loại xe lên trang index
        public ActionResult Cacloaixe()
        {
            var loaixe = from tt in data.LoaiXes select tt;
            return PartialView(loaixe);
        }
        //truyền các hãng xe lên trang index
        public ActionResult Hangxe()
        {
            var hangxe = from hx in data.HangXes select hx;
            return PartialView(hangxe);
        }

        //Lấy xe theo loại
        public ActionResult SpTheoLoai(int id)
        {
            var xe = from tt in data.Xes where tt.idLoaiXe == id select tt;
            return View(xe);
        }
        //Lấy xe theo hãng
        public ActionResult SpTheoHang(int id)
        {
            var xe = from tt in data.Xes where tt.idHangXe == id select tt;
            return View(xe);
        }

        //thông tin chi tiết xe
        public ActionResult Details(int id)
        {
            var xe = from tt in data.Xes where tt.idXe == id select tt;
            return View(xe.Single());
        }

        
    }
}