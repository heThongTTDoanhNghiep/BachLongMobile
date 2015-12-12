using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TMDT.Models
{
    public class GioHang
    {
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        public int masp { set; get; }
        public string tensp { set; get; }
        public string hinhanh { set; get; }
        public float dongia { set; get; }
        public int soluong { set; get; }
        public float thanhtien{
                get { return dongia*soluong; }
            }

        public GioHang(int ma)
        {
            masp = ma;
            BaiDang bd = db.BaiDangs.Single(n => n.MaSanPham == masp);
            tensp = bd.TenSP;
            hinhanh = bd.HinhAnh;
            dongia = bd.GiaSP;
            soluong = 1;
        }
    }
}