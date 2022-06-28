using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom2_WebsiteBanXe.Models
{
    public class ShoppingCart
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        public int iMaxe { get; set; }
        public string sTenxe { get; set; }
        public string sAnhbia { get; set; }
        public Double dGiatien { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dGiatien; }
        }
        public ShoppingCart(int idXe)
        {
            iMaxe = idXe;
            Xe xe = data.Xes.Single(n => n.idXe == iMaxe);
            sTenxe = xe.TenXe;
            sAnhbia = xe.Anhbia;
            dGiatien = double.Parse(xe.GiaTien.ToString());
            iSoluong = 1;
        }
    }
}