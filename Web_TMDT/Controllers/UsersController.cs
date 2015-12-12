using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_TMDT.Common;
using Web_TMDT.Models;
using System.ComponentModel.DataAnnotations;



namespace Web_TMDT.Controllers
{
    public class UsersController : Controller
    {
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        // GET: Users

        //------------------Đăng Nhập---------------------
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = Login(model.UserName, HamMaHoaPass.MD5Hash(model.PassWord));
                //dang nhap thanh cong
                if (result == 1)
                {
                    var user = GetByID(model.UserName);
                    var userSession = new LoginModel();
                    userSession.UserName = user.UserName;
                    userSession.IDUser = user.IDUser;
                    
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    Session["Name"] = model.UserName;
                    Session["id"] = userSession.IDUser;
                    Session["NameUser"] = user;
                    return RedirectToAction("SPMoi", "SPMoi");
                }
                else if(result == 3)
                {
                    return View("../Admin/Index");
                }
                else if (result == 0)
                {
                    ViewBag.ThongBao = ("Tài khoản không tồn tại!");
                }
                else if (result == -1)
                {
                    ViewBag.ThongBao = ("Tài khoản đã bị khóa!");
                }
                else if (result == -2)
                {
                    ViewBag.ThongBao = ("Sai mật khẩu!");
                }
                else
                {
                    ViewBag.ThongBao = ("Sai thông tin đang nhập!");
                }
            }
            return View("Login");
        }

        public User GetByID(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }
        public int Login(string username, string password)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == username);
            if (result == null)
            {
                //Tài khoản không tồn tại.
                return 0;
            }
            else if (result.Password == password && result.IDTrangThaiUser == 3)
            {
                return 3;
            }

            else
            {
                if (result.Password == password && result.IDTrangThaiUser == 2)
                {
                    //Tài khoản bị khóa
                    return -1;
                }
                else
                {
                    if (result.Password == password)
                        //Đăng nhập đúng thông tin
                        return 1;
                    else
                        // Sai mật khẩu
                        return -2;
                }
            }
        }
        //--------------------Đăng Ký---------------------

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            //Nếu đầy đủ thông tin hợp lệ
            if (ModelState.IsValid)
            {
                if (CheckUserName(model.Username))
                {
                    // ModelState.AddModelError("","Tên người dùng đã tồn tại");
                    return Content("<script language='javascript' type='text/javascript'>alert('Tài khoản đã tồn tại ');window.location.href='../SPMoi/SPMoi';</script>");
                }
                else
                {
                    var user = new User();
                    user.UserName = model.Username;
                    user.Password = HamMaHoaPass.MD5Hash(model.Password);
                    user.DiaChi = model.DiaChi;

                    user.Mail = model.Mail;
                    user.SoDienThoai = model.Sdt;
                    user.IDTrangThaiUser = 1;
                    var result = Insert(user);
                    if (result > 0)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Đăng ký thành công');window.location.href='../SPMoi/SPMoi';</script>");
                        //ViewBag.Success = "Đăng ký thành công!";
                        model = new RegisterModel();
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công");
                    }
                }
            }
            return View(model);
        }
        public long Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            return entity.IDUser;
        }
        public bool CheckUserName(string username)
        {
            return db.Users.Count(x => x.UserName == username) > 0;
        }

        //------------------Đăng Xuất-------------
        public ActionResult Logout()
        {
            Session["Name"] = null;
            Session["GioHang"] = null;
            FormsAuthentication.SignOut();

            return RedirectToAction("SPMoi", "SPMoi");
        }

        //-----------------Sửa Thông Tin người Dùng---------------------

        [HttpGet]
        public ActionResult SuaUser()
        {
            int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);
            return View(user);
        }


        [HttpPost]
        public ActionResult SuaUser(FormCollection f)
        {
            int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);

            user.UserName = f["ten"];


            user.Mail = f["mail"];

            user.DiaChi = f["diachi"];


            user.SoDienThoai = f["sdt"];

            db.SaveChanges();
            Session["Name"] = user.UserName;
            return Content("<script language='javascript' type='text/javascript'>alert('Cập Nhật thành công');window.location.href='../Users/SuaUser';</script>");
        }

        //Đổi mật Khẩu
        public ActionResult MK()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MK(FormCollection f)
        {
            int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);

            if (HamMaHoaPass.MD5Hash(f["pass"]) != user.Password)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Mật Khẩu cũ không đúng');window.location.href='../Users/MK';</script>");
            }
            if (f["pass1"] != f["pass2"])
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Nhập lại mật khẩu không khớp');window.location.href='../Users/MK';</script>");
            }

            user.Password = HamMaHoaPass.MD5Hash(f["pass1"]);
            db.SaveChanges();
            return Content("<script language='javascript' type='text/javascript'>alert('Cập Nhật thành công');window.location.href='../Users/MK';</script>");
        }

    }
}