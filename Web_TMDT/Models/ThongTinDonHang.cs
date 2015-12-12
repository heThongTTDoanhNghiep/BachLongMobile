using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TMDT.Models
{
    public class ThongTinDonHang
    {
        public int masp { set; get; }
        public string tensp { set; get; }
        public string hinhanh { set; get; }
        public float dongia { set; get; }
        public int soluong { set; get; }
        public float thanhtien
        {
            get { return dongia * soluong; }
        }
        public int tongtien { get; set; }

        public int IDUSer { get; set; }
        public string UserName { get; set; }
    }
}