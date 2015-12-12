using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_TMDT.Models
{
    [Serializable]
    public class LoginModel
    {
        [Display(Name ="Tên Tài Khoản")]
        [Required(ErrorMessage ="Chưa nhập tên tài khoản!" )]
        public string UserName { set; get; }

        [Display(Name ="Mật Khẩu")]
        [Required(ErrorMessage ="Chưa nhập mật khẩu!" )]
        [DataType(DataType.Password)]
        public string PassWord { set; get; }

        public int IDUser { get; set; }
        public bool RememberMe { set; get; }
    }
}