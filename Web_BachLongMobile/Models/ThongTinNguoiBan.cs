using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TMDT.Models
{
    public class ThongTinNguoiBan
    {
        public int Masp { set; get; }
        public string TenSP { set; get; }

        public int IDUser { set; get; }
        public string TenUser { set; get; }
        public string Mail { set; get; }
        public string Sdt { set; get; }
        public string Tinh { set; get; }

        public float DiemDG { get; set; }
        public int demNguoiDG { get; set; }
    }
}