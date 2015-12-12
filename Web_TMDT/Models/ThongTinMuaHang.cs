using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web_TMDT.Models
{
    public class ThongTinMuaHang
    {
        public int IDDonHang { get; set; }
        public int TongTien { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayLap { get; set; }
        public int TongSP { get; set; }
        public int IDTrangThai { get; set; }
        public string TenTrangThai { get; set; }
    }
}