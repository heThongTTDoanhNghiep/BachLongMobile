using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TMDT.Models;
using System.IO;
using PagedList.Mvc;
using PagedList;
using Web_TMDT.Common;
using System.ComponentModel.DataAnnotations;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Web_TMDT.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        public ActionResult Index()
        {
            return View();
        }
        #region Đăng nhập
        //Đăng nhập Admin
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection f)
        {

            if (f["ten"] == "" || f["pass"] == "")
            {
                ViewBag.thongbao = "Tên Đăng nhập hoặc Mật Khẩu không được trống!!!";
                return View("Dangnhap");
            }
            else
            {
                var ad = Dangnhap(f["ten"], f["pass"]);
                if (ad == 0)
                {
                    ViewBag.thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!!!";
                    return View("Dangnhap");
                }
                else if (ad == 1)
                {
                    Session["admin"] = f["ten"];
                    return View("Index");
                }
                else if (ad == 2)
                {
                    Session["NguoiBan"] = f["ten"];
                    return View("Index");
                }
                else
                {
                    ViewBag.thongbao = "Mật khẩu không đúng!!!";
                    return View("Dangnhap");
                }
            }
        }
        public int Dangnhap(string ten, string pass)
        {
            var kq = db.Users.SingleOrDefault(n => n.UserName == ten) ;
            if (kq == null)
            {
                return 0;
            }
            else
            {
                if (kq.Password == HamMaHoaPass.MD5Hash(pass) && kq.IDTrangThaiUser == 3)
                {
                    //đăng nhập đúng
                    return 1;
                }
                if (kq.Password == HamMaHoaPass.MD5Hash(pass) && kq.IDTrangThaiUser == 4)
                {
                    //đăng nhập đúng
                    return 2;
                }
                else
                {
                    //đăng nhập sai mk
                    return -1;
                }
            }
        }


        public ActionResult Logout()
        {
            Session["admin"] = null;
            Session["NguoiBan"] = null;
            return RedirectToAction("Index");
        }
        #endregion


        #region QL Bài Đăng
        public ActionResult QLBaiDang(int? page)
        {
            
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                        return View("Index");

                }
            }
            
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<BaiDang> bd = (from n in db.BaiDangs orderby (n.SoLuong - n.Soluongton) descending select n).ToList();
            return View(bd.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SapXepGiamdan(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<BaiDang> b = (from bd in db.BaiDangs orderby bd.DanhGia descending select bd).ToList();
            return View(b.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult CapnhatSL(int id)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);
            return View(bd);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapnhatSL(int id, FormCollection f)
        {

            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);

            
            bd.SoLuong = bd.SoLuong + int.Parse(f["slthem"].ToString());
            bd.Soluongton = bd.Soluongton + int.Parse(f["slthem"].ToString());
            //bd.SoLuong = int.Parse(f["sldang"].ToString());
            //bd.Soluongton = int.Parse(f["slton"].ToString());
            db.SaveChanges();
            return RedirectToAction("QLBaiDang");
        }

        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.Loai = new SelectList(db.LoaiSanPhams.ToList(), "MaLoai", "TenLoai", bd.MaLoai );
            return View(bd);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSP(int id, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);           

            bd.TenSP = f["tensp"].ToString();
            //bd.Soluongton = int.Parse(f["sl"].ToString());
            bd.MaLoai = int.Parse(f["Loai"].ToString());
            bd.GiaSP = int.Parse(f["giasp"].ToString());
            bd.MoTa = f["mota"].ToString();
            db.SaveChanges();

            ViewBag.Loai = new SelectList(db.LoaiSanPhams.ToList(), "MaLoai", "TenLoai", bd.MaLoai);
            return RedirectToAction("QLBaiDang");
        }


         [HttpGet]
        public ActionResult CapNhatTT(int id)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.IDTrangThai = new SelectList(db.TrangThaiBaiDangs.ToList(), "IDTrangThai", "TenTrangThai", bd.IDTrangThai);
            return View(bd);
        }
         [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatTT(int id,FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == id);
            bd.IDTrangThai = int.Parse(f["IDTrangThai"].ToString());
          
            db.SaveChanges();
            ViewBag.IDTrangThai = new SelectList(db.TrangThaiBaiDangs.ToList(), "IDTrangThai", "TenTrangThai", bd.IDTrangThai);
            return RedirectToAction("QLBaiDang");
        }
       

        #endregion
         public ActionResult QLUser(int? page)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }

            
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<User> user = db.Users.ToList();
            return View(user.ToPagedList(pageNumber, pageSize));
        }

         [HttpGet]
        
         public ActionResult ThemUser()
         {

             if (Session["NguoiBan"] != null)
             {
                 return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
             }
             else if (Session["admin"] == null)
             {
                 if (Session["NguoiBan"] == null)
                 {
                     return View("Index");

                 }
             }
             return View();
         }
         [HttpPost]
         [ValidateInput(false)]
        public ActionResult ThemUser(RegisterModel model)
         {
             if (Session["NguoiBan"] != null)
             {
                 return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
             }
             else if (Session["admin"] == null)
             {
                 if (Session["NguoiBan"] == null)
                 {
                     return View("Index");

                 }
             }
             //Nếu đầy đủ thông tin hợp lệ
             if (ModelState.IsValid)
             {
                 if (CheckUserName(model.Username))
                 {
                     // ModelState.AddModelError("","Tên người dùng đã tồn tại");
                     return Content("<script language='javascript' type='text/javascript'>alert('Tài khoản đã tồn tại ');window.location.href='../Admin/ThemUser';</script>");
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
                         return Content("<script language='javascript' type='text/javascript'>alert('Thêm Tài Khoản thành công');window.location.href='../Admin/QLUser';</script>");
                         //ViewBag.Success = "Đăng ký thành công!";
                         model = new RegisterModel();
                         return View();
                     }
                     else
                     {
                         ModelState.AddModelError("", "Sửa Tài Khoản không thành công");
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
        [HttpGet]
        public ActionResult SuaUser(int id)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
           // int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);
            return View(user);
        }


        [HttpPost]
        public ActionResult SuaUser(FormCollection f, int id)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            //int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);

            user.UserName = f["ten"];


            user.Mail = f["mail"];

            user.DiaChi = f["diachi"];


            user.SoDienThoai = f["sdt"];

            db.SaveChanges();
            Session["Name"] = user.UserName;
            return Content("<script language='javascript' type='text/javascript'>alert('Sửa Tài Khoản Thành Công');window.location.href='../QLUser';</script>");
        }
        [HttpGet]
        public ActionResult DoiMatKhau(int id)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            // int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);
            return View(user);
        }


        [HttpPost]
        public ActionResult DoiMatKhau(FormCollection f, int id)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            //int id = int.Parse(Session["id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.IDUser == id);

            user.UserName = f["ten"];
           

            if (f["MatKhau"] != f["NhapLaiMK"])
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Nhập lại mật khẩu không khớp');window.location.href='../DoiMatKhau/"+id+"';</script>");
            }
            user.Password = HamMaHoaPass.MD5Hash(f["MatKhau"]);
            db.SaveChanges();
            Session["Name"] = user.UserName;
            return Content("<script language='javascript' type='text/javascript'>alert('Đổi Mật Khẩu Thành Công');window.location.href='../QLUser';</script>");
        }

        public ActionResult TTUser(int id)
         {
             if (Session["admin"] == null)
             {
                 if (Session["NguoiBan"] == null)
                 {
                     return View("Index");

                 }
             }
             List<User> user = db.Users.Where(n => n.IDUser == id).ToList();
             return View(user);
         }

        [HttpGet]
        public ActionResult CapNhatTTUser(int id)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            User bd = db.Users.SingleOrDefault(n => n.IDUser == id);
            ViewBag.IDTrangThaiUser = new SelectList(db.TrangThaiUsers.ToList(), "IDTrangThaiUser", "TenTrangThaiUser", bd.IDTrangThaiUser);
            return View(bd);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatTTUser(int id, FormCollection f)
        {
            if (Session["NguoiBan"] != null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Xin Lỗi! Bạn Không Có Quyền Vào Trang Này');window.location.href='../Admin/Index';</script>");
            }
            else if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            User bd = db.Users.SingleOrDefault(n => n.IDUser == id);
            
            bd.IDTrangThaiUser = int.Parse(f["IDTrangThaiUser"].ToString());

            db.SaveChanges();
            ViewBag.IDTrangThaiUser = new SelectList(db.TrangThaiUsers.ToList(), "IDTrangThaiUser", "TenTrangThaiUser", bd.IDTrangThaiUser);
            return RedirectToAction("QLUser");
        }
        
        [HttpGet]
        public ActionResult HoaDon(int id)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            User us = db.Users.SingleOrDefault(n => n.IDUser == id);
            return View(us);
        }
        public ActionResult DHMua(int? page)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<GioHang_DonHangMua> dh = (from n in db.GioHang_DonHangMua orderby n.NgayLap descending select n).ToList();

            ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.IDTrangThai != 5 select n.TongTien).Sum();
            ViewBag.sl = (from n in db.GioHang_DonHangMua select n.IDDonHangMua).Count();

            return View(dh.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult CapNhatTTDHMua(int id)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            GioHang_DonHangMua bd = db.GioHang_DonHangMua.SingleOrDefault(n => n.IDDonHangMua == id);
            ViewBag.IDTrangThai = new SelectList(db.TrangThai_DonHangMua.ToList(), "IDTrangThai", "TenTrangThai", bd.IDTrangThai);
            return View(bd);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CapNhatTTDHMua(int id, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            GioHang_DonHangMua bd = db.GioHang_DonHangMua.SingleOrDefault(n => n.IDDonHangMua == id);
            bd.IDTrangThai = int.Parse(f["IDTrangThai"].ToString());

            db.SaveChanges();
            ViewBag.IDTrangThai = new SelectList(db.TrangThai_DonHangMua.ToList(), "IDTrangThai", "TenTrangThai", bd.IDTrangThai);
            return RedirectToAction("DHMua");
        }
        public ActionResult CTDHMua(int id, int? page)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<ChiTietGioHang> ct = db.ChiTietGioHangs.Where(n => n.IDDonHangMua == id).ToList();
            Session["iddh"] = id;
            return View(ct.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult TimQuanLyUser(int? page, string tim)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            @ViewBag.tim = tim;
            List<User> ql = new List<User>();
            if (!string.IsNullOrEmpty(tim))
            {
                ql = db.Users.Where(n => n.UserName.Contains(tim)).ToList();
            }
            else if (string.IsNullOrEmpty(tim))
            {
                ql = db.Users.ToList();
            }
            else if (ql.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/QLUser';</script>");
            }
            return View(ql.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult TimQuanLyUser(int? page, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string tim = f["txttimkiem"];
            @ViewBag.tim = tim;
            List<User> ql = new List<User>();
            if (!string.IsNullOrEmpty(tim))
            {
                ql = db.Users.Where(n => n.UserName.Contains(tim)).ToList();
            }
            else if(string.IsNullOrEmpty(tim))
            {
                ql = db.Users.ToList();
            }
            else if(ql.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/QLUser';</script>");
            }
            return View(ql.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult TimQuanLyBaiDang(int? page, string tim, string tungay, string denngay)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            @ViewBag.tim = tim;
            @ViewBag.tungay = tungay;
            @ViewBag.denngay = denngay;

            List<BaiDang> bd = new List<BaiDang>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.BaiDangs orderby (n.SoLuong - n.Soluongton) descending select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.BaiDangs
                      where n.TenSP.Contains(tim)
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs.ToList()
                      where n.NgayDang >= ngaytu
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden && n.NgayDang >= ngaytu
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden && n.NgayDang >= ngaytu && n.TenSP.Contains(tim)
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (bd.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/QLBaiDang';</script>");
            }
            return View(bd.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult TimQuanLyBaiDang(int? page, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string tim = f["txttimkiem"];
            string tungay = Request["tungay"];
            string denngay = Request["denngay"];

            @ViewBag.tim = tim;
            @ViewBag.tungay = tungay;
            @ViewBag.denngay = denngay;

            List<BaiDang> bd = new List<BaiDang>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.BaiDangs orderby (n.SoLuong - n.Soluongton) descending select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.BaiDangs
                      where n.TenSP.Contains(tim)
                      orderby n.MaSanPham descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                //ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs.ToList()
                      where n.NgayDang >= ngaytu
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
               // ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden && n.NgayDang >= ngaytu
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.BaiDangs
                      where n.NgayDang <= ngayden && n.NgayDang >= ngaytu && n.TenSP.Contains(tim)
                      orderby (n.SoLuong - n.Soluongton) descending
                      select n).ToList();
            }
            else if (bd.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/QLBaiDang';</script>");
            }
            return View(bd.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult TimDonHangMua(int? page, string tim, string tungay, string denngay)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            @ViewBag.tim = tim;
            @ViewBag.tungay = tungay;
            @ViewBag.denngay = denngay;
            List<GioHang_DonHangMua> bd = new List<GioHang_DonHangMua>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.GioHang_DonHangMua orderby n.NgayLap descending select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.GioHang_DonHangMua
                      where n.User.UserName.Contains(tim)
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.User.UserName.Contains(tim) && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.User.UserName.Contains(tim) select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                //ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap >= ngaytu && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap >= ngaytu select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
               // ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap <= ngayden
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden select n.IDDonHangMua).Count();

            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu && n.NgayLap <= ngayden
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu && n.NgayLap <= ngayden && n.User.UserName.Contains(tim)
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.IDTrangThai != 5 && n.User.UserName.Contains(tim) select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.User.UserName.Contains(tim) select n.IDDonHangMua).Count();
            }
            else if (bd.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/ThongKeHD';</script>");
            }
            //DateTime tu = Convert.ToDateTime(tungay);
            //DateTime den = Convert.ToDateTime(denngay);

            return View(bd.ToPagedList(pageNumber, pageSize));

        }

        [HttpPost]
        public ActionResult TimDonHangMua(int? page, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string tim = f["txttimkiem"];
            string tungay = Request["tungay"];
            string denngay = Request["denngay"];

            @ViewBag.tim = tim;
            @ViewBag.tungay = tungay;
            @ViewBag.denngay = denngay;
            List<GioHang_DonHangMua> bd = new List<GioHang_DonHangMua>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.GioHang_DonHangMua orderby n.NgayLap descending select n).ToList();
               
                    ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.IDTrangThai != 5 select n.TongTien).Sum();
                    ViewBag.sl = (from n in db.GioHang_DonHangMua select n.IDDonHangMua).Count();
                
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                bd = (from n in db.GioHang_DonHangMua
                      where n.User.UserName.Contains(tim)
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.User.UserName.Contains(tim) && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.User.UserName.Contains(tim) select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                //ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap >= ngaytu && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap >= ngaytu select n.IDDonHangMua).Count();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            //    ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap <= ngayden
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.IDTrangThai != 5 select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden select n.IDDonHangMua).Count();

            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tim))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu && n.NgayLap <= ngayden
                      orderby n.NgayLap descending
                      select n).ToList();
                
                    ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.IDTrangThai != 5 select n.TongTien).Sum();
                    ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu select n.IDDonHangMua).Count();
                
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                bd = (from n in db.GioHang_DonHangMua
                      where n.NgayLap >= ngaytu && n.NgayLap <= ngayden && n.User.UserName.Contains(tim)
                      orderby n.NgayLap descending
                      select n).ToList();
                ViewBag.Tong = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.IDTrangThai != 5 && n.User.UserName.Contains(tim) select n.TongTien).Sum();
                ViewBag.sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.User.UserName.Contains(tim) select n.IDDonHangMua).Count();
            }
            else if (bd.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/ThongKeHD';</script>");
            }

            return View(bd.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult TimCTDHMua(FormCollection f)
        {
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("Index");

                }
            }
            string tim = f["txttimkiem"].ToString();
            int id = int.Parse(Session["iddh"].ToString());
            List<ChiTietGioHang> ct = new List<ChiTietGioHang>();

            if (string.IsNullOrWhiteSpace(tim))
            {
                ct = db.ChiTietGioHangs.Where(n => n.IDDonHangMua == id).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tim))
            {
                ct = db.ChiTietGioHangs.Where(n => n.IDDonHangMua == id).ToList();
            }
            else if (ct.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../Admin/ThongKeHD';</script>");
            }
            return View(ct);

        }

        //----------------------------Xuất Tất Cả Đơn Hàng---------------------
        public ActionResult XuatDonHang()
        {
            List<GioHang_DonHangMua> dh = (from n in db.GioHang_DonHangMua orderby n.NgayLap descending select n).ToList();

            var sum = (from n in db.GioHang_DonHangMua where n.IDTrangThai != 5 select n.TongTien).Sum();
            int sl = (from n in db.GioHang_DonHangMua select n.IDDonHangMua).Count();
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;
               
                worksheet.Cells[2] = "Danh Sách Tất Cả Đơn Hàng";
                worksheet.Cells[3, 2] ="Tổng Doanh Thu: "  +string.Format("{0:0,0} VNĐ", sum) ;
                worksheet.Cells[4, 2] = "Số Đơn Hàng: "+sl;

                worksheet.Cells[6, 1] = "Mã ĐH";
                worksheet.Cells[6, 2] = "Tên Khách Hàng";
                worksheet.Cells[6, 3] = "Tổng Tiền";
                worksheet.Cells[6, 4] = "Ngày Lập";


                int row = 7;
                foreach (var p in dh)
                {
                    worksheet.Cells[row, 1] = p.IDDonHangMua;
                    worksheet.Cells[row, 2] = p.User.UserName;
                    worksheet.Cells[row, 3] = p.NgayLap.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.TongTien);
                    row++;
                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A6", "D6").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;
                
                

                var dinhdang = worksheet.get_Range("A6", "D6");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3", "B4");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;
                
                work.SaveAs("D:\\DHMua.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/DHMua';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/DHMua';</script>");
        }

        //---------------------Xuất Doanh Thu Theo Thời Gian---------------------

        public ActionResult XuatThongKeDonHang(string tu, string den)
        {
           
            DateTime ngaytu = Convert.ToDateTime(tu);
            DateTime ngayden = Convert.ToDateTime(den);


            ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<GioHang_DonHangMua> dh = (from n in db.GioHang_DonHangMua
                  where n.NgayLap >= ngaytu && n.NgayLap <= ngayden
                  orderby n.NgayLap descending
                  select n).ToList();
            var sum = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu && n.IDTrangThai != 5 select n.TongTien).Sum();
            int sl = (from n in db.GioHang_DonHangMua where n.NgayLap <= ngayden && n.NgayLap >= ngaytu select n.IDDonHangMua).Count();
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Đơn Hàng Từ " + ngaytu.ToString("dd/MM/yyyy") + " Đến " + ngayden.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 2] = "Tổng Doanh Thu: " + string.Format("{0:0,0} VNĐ", sum);
                worksheet.Cells[4, 2] = "Số Đơn Hàng: " + sl;

                worksheet.Cells[6, 1] = "Mã ĐH";
                worksheet.Cells[6, 2] = "Tên Khách Hàng";
                worksheet.Cells[6, 3] = "Tổng Tiền";
                worksheet.Cells[6, 4] = "Ngày Lập";


                int row = 7;
                foreach (var p in dh)
                {
                    worksheet.Cells[row, 1] = p.IDDonHangMua;
                    worksheet.Cells[row, 2] = p.User.UserName;
                    worksheet.Cells[row, 3] = p.NgayLap.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.TongTien);
                    row++;
                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A6", "D6").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A6", "D6");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3", "B4");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;
                work.SaveAs("D:\\ThongKe_DoanhThu.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/DHMua';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/DHMua';</script>");
        }


        //---------------------------Xuất Tất Cả Sản Phẩm-------------------------------

        public ActionResult XuatSanPhamBanChay()
        {
           
            int max = (from n in db.BaiDangs select (n.SoLuong - n.Soluongton)).Max();
            List<BaiDang> spmax = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) >= (max - 3) orderby (bd.SoLuong - bd.Soluongton) descending select bd).ToList();
            int slmax = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) >= (max - 3) select (bd.SoLuong - bd.Soluongton)).Count();          
            
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Bán Chạy Nhất";
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + slmax;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Số Lượng Bán";

                int row = 6;
                foreach (var p in spmax)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.SoLuong - p.Soluongton;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;             

                work.SaveAs("D:\\SanPhamBanChayNhat.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }

        public ActionResult XuatSanPhamBanCham()
        {
         
            int min = (from n in db.BaiDangs select (n.SoLuong - n.Soluongton)).Min();
            List<BaiDang> spmin = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) <= (min + 3) orderby (bd.SoLuong - bd.Soluongton) descending select bd).ToList();
            int slmin = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) <= (min + 3) select (bd.SoLuong - bd.Soluongton)).Count();
  
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Bán Chậm Nhất";
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + slmin;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Số Lượng Bán";

                int row = 6;
                foreach (var p in spmin)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.SoLuong - p.Soluongton;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;             


                work.SaveAs("D:\\ SanPhamBanChamNhat.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }




        public ActionResult XuatSanPhamDanhGiaCao()
        {

            var maxdg = (from n in db.BaiDangs where n.DanhGia != 0 select n.DanhGia).Max();
            List<BaiDang> danhgiamax = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia >= (maxdg - 3) orderby bd.DanhGia descending select bd).ToList();
            int sl = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia >= (maxdg - 3) select bd.DanhGia).Count();
               
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Đánh Giá Cao Nhất";
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + sl;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Đánh Giá";

                int row = 6;
                foreach (var p in danhgiamax)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.DanhGia;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;

                work.SaveAs("D:\\SanPhamDanhGiaCao.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }


        public ActionResult XuatSanPhamDanhGiaThap()
        {

          //  var mindg = (from n in db.BaiDangs where n.DanhGia != 0 select n.DanhGia).Min();
            List<BaiDang> danhgiamin = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia <= 4 orderby bd.DanhGia descending select bd).ToList();
            int sl = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia <= 4 select bd.DanhGia).Count();            
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Đánh Giá Thấp Nhất";
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + sl;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Đánh Giá";

                int row = 6;
                foreach (var p in danhgiamin)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.DanhGia;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;

                work.SaveAs("D:\\SanPhamDanhGiaThap.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }

        //-------------Xuất Sản Phẩm Theo Thời Gian--------------

        public ActionResult TKXuatSanPhamBanChay(string tungay, string denngay)
        {
            DateTime ngaytu = Convert.ToDateTime(tungay);
            DateTime ngayden = Convert.ToDateTime(denngay);
            ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
            int max = (from n in db.BaiDangs where n.NgayDang >= ngaytu && n.NgayDang <= ngayden  select (n.SoLuong - n.Soluongton)).Max();
            List<BaiDang> spmax = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) >= (max - 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden  orderby (bd.SoLuong - bd.Soluongton) descending select bd).ToList();
            int slmax = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) >= (max - 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden select (bd.SoLuong - bd.Soluongton)).Count();

            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Bán Chạy Nhất Từ: " + ngaytu.ToString("dd/MM/yyyy") + " Đến " + ngayden.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + slmax;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Số Lượng Bán";

                int row = 6;
                foreach (var p in spmax)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.SoLuong - p.Soluongton;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;

                work.SaveAs("D:\\SanPhamBanChayNhat_TheoThoiGian.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }

        public ActionResult TKXuatSanPhamBanCham(string tungay, string denngay)
        {
            DateTime ngaytu = Convert.ToDateTime(tungay);
            DateTime ngayden = Convert.ToDateTime(denngay);
            ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);

            int min = (from n in db.BaiDangs where n.NgayDang >= ngaytu && n.NgayDang <= ngayden select (n.SoLuong - n.Soluongton)).Min();
            List<BaiDang> spmin = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) <= (min + 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden orderby (bd.SoLuong - bd.Soluongton) descending select bd).ToList();
            int slmin = (from bd in db.BaiDangs where (bd.SoLuong - bd.Soluongton) <= (min + 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden select (bd.SoLuong - bd.Soluongton)).Count();


            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Bán Chậm Nhất Từ: " + ngaytu.ToString("dd/MM/yyyy") + " Đến " + ngayden.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + slmin;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Số Lượng Bán";

                int row = 6;
                foreach (var p in spmin)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.SoLuong - p.Soluongton;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;


                work.SaveAs("D:\\SanPhamBanChamNhat_TheoThoiGian.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }



        public ActionResult TKXuatSanPhamDanhGiaCao(string tungay, string denngay)
        {
            DateTime ngaytu = Convert.ToDateTime(tungay);
            DateTime ngayden = Convert.ToDateTime(denngay);
            ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
            var maxdg = (from n in db.BaiDangs where n.DanhGia != 0 && n.NgayDang >= ngaytu && n.NgayDang <= ngayden select n.DanhGia).Max();
            List<BaiDang> danhgiamax = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia >= (maxdg - 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden orderby bd.DanhGia descending select bd).ToList();
            int sl = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia >= (maxdg - 3) && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden select bd.DanhGia).Count();

            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Đánh Giá Cao Nhất Từ: " + ngaytu.ToString("dd/MM/yyyy") + " Đến " + ngayden.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + sl;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Đánh Giá";

                int row = 6;
                foreach (var p in danhgiamax)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.DanhGia;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;

                work.SaveAs("D:\\SanPhamDanhGiaCao_TheoThoiGian.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }


        public ActionResult TKXuatSanPhamDanhGiaThap(string tungay, string denngay)
        {
            DateTime ngaytu = Convert.ToDateTime(tungay);
            DateTime ngayden = Convert.ToDateTime(denngay);
            ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
            ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
          //  var mindg = (from n in db.BaiDangs where  n.DanhGia != 0 && n.NgayDang >= ngaytu && n.NgayDang <= ngayden  select n.DanhGia).Min();
            List<BaiDang> danhgiamin = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia <= 4 && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden orderby bd.DanhGia descending select bd).ToList();
            int sl = (from bd in db.BaiDangs where bd.DanhGia != 0 && bd.DanhGia <= 4 && bd.NgayDang >= ngaytu && bd.NgayDang <= ngayden select bd.DanhGia).Count();
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook work = app.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet worksheet = work.ActiveSheet;

                worksheet.Cells[2] = "Danh Sách Sản Phẩm Đánh Giá Thấp Nhất Từ: " + ngaytu.ToString("dd/MM/yyyy") + " Đến " + ngayden.ToString("dd/MM/yyyy");
                worksheet.Cells[3, 2] = "Số Lượng Sản Phẩm: " + sl;

                worksheet.Cells[5, 1] = "Mã";
                worksheet.Cells[5, 2] = "Tên Sản Phẩm";
                worksheet.Cells[5, 3] = "Loại Sản Phẩm";
                worksheet.Cells[5, 4] = "Giá Sản Phẩm";
                worksheet.Cells[5, 5] = "Ngày Đăng";
                worksheet.Cells[5, 6] = "Đánh Giá";

                int row = 6;
                foreach (var p in danhgiamin)
                {

                    worksheet.Cells[row, 1] = p.MaSanPham;
                    worksheet.Cells[row, 2] = p.TenSP;
                    worksheet.Cells[row, 3] = p.LoaiSanPham.TenLoai;
                    worksheet.Cells[row, 4] = string.Format("{0:0,0}", p.GiaSP);
                    worksheet.Cells[row, 5] = p.NgayDang.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6] = p.DanhGia;
                    row++;

                }

                worksheet.Cells[2].EntireColumn.AutoFit();
                worksheet.get_Range("A5", "F5").EntireColumn.AutoFit();


                var tieude = worksheet.Cells[2];
                tieude.Font.Bold = true;
                tieude.Font.Color = System.Drawing.Color.Red;
                tieude.Font.Size = 20;



                var dinhdang = worksheet.get_Range("A5", "F5");
                dinhdang.Font.Bold = true;
                dinhdang.Font.Color = System.Drawing.Color.Blue;
                dinhdang.Font.Size = 13;



                var tongtien = worksheet.get_Range("B3");
                tongtien.Font.Bold = true;
                tongtien.Font.Color = System.Drawing.Color.Black;
                tongtien.Font.Size = 15;

                work.SaveAs("D:\\SanPhamDanhGiaThap_TheoThoiGian.xls");
                work.Close();
                Marshal.ReleaseComObject(work);

                app.Quit();
                Marshal.FinalReleaseComObject(app);

                return Content("<script language='javascript' type='text/javascript'>alert('Đã Xuất Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = ex.Message;
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Không Xuất Được Thống Kê ra file Excel');window.location.href='../Admin/QLBaiDang';</script>");
        }





   }

}