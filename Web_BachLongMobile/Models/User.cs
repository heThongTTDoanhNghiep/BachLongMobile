//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_TMDT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.GioHang_DonHangMua = new HashSet<GioHang_DonHangMua>();
        }
    
        public int IDUser { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DiaChi { get; set; }
        public string Mail { get; set; }
        public string SoDienThoai { get; set; }
        public int IDTrangThaiUser { get; set; }
    
        public virtual ICollection<GioHang_DonHangMua> GioHang_DonHangMua { get; set; }
        public virtual TrangThaiUser TrangThaiUser { get; set; }
    }
}
