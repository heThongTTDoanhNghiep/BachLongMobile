using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TMDT.Models
{
    public class ChiTietSanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string hinhanh { get; set; }
        public int GiaSP { get; set; }

        public int IDUer { get; set; }
        public string TenUser { get; set; }
        public int SoLuongTon { get; set; }
        public string MoTa { get; set; }
        public float DiemDG { get; set; }
        public int DemNguoiDG { get; set; }
    }
}