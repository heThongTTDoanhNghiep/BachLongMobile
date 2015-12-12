using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_TMDT.Models
{
    [MetadataTypeAttribute(typeof(BaiDangMetadata))]
    public partial class BaiDang
    {
        internal sealed class BaiDangMetadata
        {
            
            [Display(Name = "Tên Sản Phẩm")]
            [Required(ErrorMessage = "{0} Không được để trống!")]
            public string TenSP { get; set; }
            [Display(Name = "Giá Bán")]
            [Required(ErrorMessage = " {0} Không được để trống!")]
            public int GiaSP { get; set; }
            [Display(Name = "Mô Tả")]
            [Required(ErrorMessage = " {0} Không được để trống!")]
            public string MoTa { get; set; }
            [Display(Name = "Loại Sản Phẩm")]
            [Required(ErrorMessage = "Vui lòng chọn {0}!")]
            public int MaLoai { get; set; }
            [Display(Name = "Số Lượng")]
            [Required(ErrorMessage = " {0} Không được để trống!")]
            public int SoLuong { get; set; }
            [Display(Name = "Hình Ảnh")]
            public int HinhAnh { get; set; }
            public float DanhGia { get; set; }
        }

    }
}