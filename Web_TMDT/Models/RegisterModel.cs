using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Web_TMDT.Models
{
    public class RegisterModel 
    {
        [Display(Name = "Tên Đăng Nhập: ")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        public string Username { set; get; }

        [Display(Name = "Mật Khẩu: ")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 20 kí tự!")]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Display(Name = "Nhập Lại MK:")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { set; get; }

        [Display(Name = "Địa Chỉ:")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        public string DiaChi { set; get; }

        [EmailAddress]
        [Display(Name = "Email:")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "{0} không hợp lệ!")]
        public string Mail { set; get; }

        [Display(Name = "Số Điện Thoại:")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [RegularExpression(@"^-*0+[0-9,\.?\-?\(?\)?\ ]+$", ErrorMessage = "{0} không hợp lệ!")]
        [StringLength(maximumLength: 11, MinimumLength = 10, ErrorMessage = "{0} không hợp lệ!")]
        public string Sdt { set; get; }

        
    }
}