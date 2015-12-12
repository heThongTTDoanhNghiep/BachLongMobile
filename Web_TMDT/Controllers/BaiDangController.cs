using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TMDT.Models;
using System.IO;
using PagedList.Mvc;
using PagedList;

namespace Web_TMDT.Controllers
{
    public class BaiDangController : Controller
    {
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        //
        // GET: /BaiDang/
        #region Đăng Tin Sản Phẩm
        [HttpGet]
        public ActionResult BaiDang()
        {
            
            if (Session["admin"] == null)
            {
                if (Session["NguoiBan"] == null)
                {
                    return View("../Admin/Index");

                }
            }
            ViewBag.MaLoai = new SelectList(db.LoaiSanPhams.ToList(), "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
       
        public ActionResult BaiDang(BaiDang baidang, HttpPostedFileBase fileUpload, FormCollection f)
        {
            if (Session["admin"] == null)
            {
                return View("../Admin/Index");
            }

            ViewBag.MaLoai = new SelectList(db.LoaiSanPhams.ToList(), "MaLoai", "TenLoai");
           

            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    ViewBag.ThongBao = "Chọn hình ảnh của Sản Phẩm";
                    return View();
                }
                var filename = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Hinh_Anh"), filename);
                if (System.IO.File.Exists(path))
                {
                    ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    return View();
                }
                else
                {
                    fileUpload.SaveAs(path);
                }
                baidang.HinhAnh = fileUpload.FileName;
                baidang.NgayDang = DateTime.Now;
                baidang.SoLuong = int.Parse(f["sl"]);
                baidang.Soluongton = baidang.SoLuong;
                baidang.IDTrangThai = 2;
                baidang.DanhGia = 0;
                baidang.MoTa = baidang.MoTa;
                db.BaiDangs.Add(baidang);
                db.SaveChanges();
                return Content("<script language='javascript' type='text/javascript'>alert('Đăng bài thành công');window.location.href='../BaiDang/BaiDang';</script>");
            }
            return View();
        }
        #endregion

        #region Load TT Đơn hàng
        public ActionResult ThongTinMuaHang(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string id = Session["id"].ToString();
           
            int iduser = int.Parse(id);

            @ViewBag.sl = db.GioHang_DonHangMua.Count(n => n.IDUserMua == iduser);
            //var model = db.GioHang_DonHangMua.Where(n => n.IDUserMua == id).ToList();
            var model = (from gh in db.GioHang_DonHangMua.ToList()
                         where gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap,
                         }).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }
        #endregion
        #region Load TT Chi Tiết DH
        public ActionResult ThongTinDonHang(int? page, int id)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            Session["idDH"] = id;
           var list =(from t in db.BaiDangs.ToList()
                      from gh in db.GioHang_DonHangMua.ToList()
                      from ct in db.ChiTietGioHangs.ToList()
                        where t.MaSanPham == ct.MaSanPham && ct.IDDonHangMua == gh.IDDonHangMua && gh.IDDonHangMua == id 
                        select new ThongTinDonHang()                       
                        { 
                         masp = t.MaSanPham,
                          tensp = t.TenSP,
                          hinhanh = t.HinhAnh,
                          dongia = t.GiaSP,
                          soluong = ct.SoLuong
  
                          
                        }).ToList();
           float sum = 0;
           foreach (var item in list)
           {
               sum = sum + Convert.ToInt32(item.thanhtien);
           }
           ViewBag.sum = sum;
           return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThongTinNguoiBan(int masp)
        {
            List<DanhGiaSanPham> dem = new List<DanhGiaSanPham>();
            dem = db.DanhGiaSanPhams.Where(n => n.MaSanPham == masp).ToList();
            int tong = dem.Count;
            
            var sp = (from bd in db.BaiDangs.ToList()
                      where bd.MaSanPham == masp
                      select new ChiTietSanPham()
                      {
                          MaSP = bd.MaSanPham,
                          hinhanh = bd.HinhAnh,
                          TenSP = bd.TenSP,
                          GiaSP = bd.GiaSP,
                          SoLuongTon = bd.Soluongton,
                          MoTa = bd.MoTa,
                          DemNguoiDG = tong,
                          DiemDG = Convert.ToInt32(bd.DanhGia)
                      }).ToList();

            return View(sp);
        }

        #endregion
        #region Tìm Kiếm Đơn Hàng
        [HttpGet]
        public ActionResult KQTimDH(int? page, string tungay, string denngay)
        {
            //ViewBag.tim = tim;
            ViewBag.tungay = tungay;
            ViewBag.denngay = denngay;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string user = Session["id"].ToString();
            int iduser = int.Parse(user);
            List<ThongTinMuaHang> KQTim = new List<ThongTinMuaHang>();
            if (string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                         where gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap
                         }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
              //  ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                         where gh.NgayLap >= ngaytu && gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap
                         }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
             //   ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                         where gh.NgayLap <= ngayden && gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap
                         }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                         where ngaytu <= gh.NgayLap && gh.NgayLap <= ngayden && gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap
                         }).ToList();
            }
            else if (KQTim.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../BaiDang/ThongTinMuaHang';</script>");
            }
            return View(KQTim.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult KQTimDH(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string user = Session["id"].ToString();
            int iduser = int.Parse(user);
            string tungay = Request["tungay"];
            string denngay = Request["denngay"];
            ViewBag.tungay = tungay;
            ViewBag.denngay = denngay;

            List<ThongTinMuaHang> KQTim = new List<ThongTinMuaHang>();
            if (string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                             where gh.IDUserMua == iduser
                             select new ThongTinMuaHang()
                             {
                                 IDDonHang = gh.IDDonHangMua,
                                 TongTien = Convert.ToInt32(gh.TongTien),
                                 NgayLap = gh.NgayLap
                             }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
               // ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                             where gh.NgayLap >= ngaytu && gh.IDUserMua == iduser
                             select new ThongTinMuaHang()
                             {
                                 IDDonHang = gh.IDDonHangMua,
                                 TongTien = Convert.ToInt32(gh.TongTien),
                                 NgayLap = gh.NgayLap
                             }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(denngay) && string.IsNullOrWhiteSpace(tungay))
            {
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
              //  ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                             where  gh.NgayLap <= ngayden && gh.IDUserMua == iduser
                             select new ThongTinMuaHang()
                             {
                                 IDDonHang = gh.IDDonHangMua,
                                 TongTien = Convert.ToInt32(gh.TongTien),
                                 NgayLap = gh.NgayLap
                             }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(tungay) && !string.IsNullOrWhiteSpace(denngay))
            {
                DateTime ngaytu = Convert.ToDateTime(tungay);
                DateTime ngayden = Convert.ToDateTime(denngay);
                ngayden = ngayden.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngaytu = ngaytu.AddHours(23).AddMinutes(59).AddSeconds(59);
                KQTim = (from gh in db.GioHang_DonHangMua.ToList()
                         where ngaytu <= gh.NgayLap && gh.NgayLap <= ngayden && gh.IDUserMua == iduser
                         select new ThongTinMuaHang()
                         {
                             IDDonHang = gh.IDDonHangMua,
                             TongTien = Convert.ToInt32(gh.TongTien),
                             NgayLap = gh.NgayLap
                         }).ToList();
            }
            else if(KQTim.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../BaiDang/ThongTinMuaHang';</script>");
            }
            return View(KQTim.ToPagedList(pageNumber, pageSize));
        }
        #endregion
        #region Tìm Kiếm Chi tiết ĐH
        [HttpGet]
        public ActionResult KQtimCTDH(int? page, string tim)
        {
            string idDH = Session["idDH"].ToString();
            @ViewBag.tim = tim;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<ThongTinDonHang> KQTim = new List<ThongTinDonHang>();
            if (string.IsNullOrWhiteSpace(tim))
            {
                int id = int.Parse(idDH);
                KQTim = (from t in db.BaiDangs.ToList()
                         from ct in db.ChiTietGioHangs.ToList()
                         where t.MaSanPham == ct.MaSanPham && ct.IDDonHangMua == id
                         select new ThongTinDonHang()
                         {
                             masp = t.MaSanPham,
                             tensp = t.TenSP,
                             hinhanh = t.HinhAnh,
                             dongia = t.GiaSP,
                             soluong = ct.SoLuong
                         }).ToList();
                float sum = 0;
                foreach (var item in KQTim)
                {
                    sum = sum + Convert.ToInt32(item.thanhtien);
                }
                ViewBag.sum = sum;
            }
            else if (!string.IsNullOrWhiteSpace(tim))
            {
                int id = int.Parse(idDH);
                KQTim = (from t in db.BaiDangs.ToList()
                         from ct in db.ChiTietGioHangs.ToList()
                         where t.MaSanPham == ct.MaSanPham && ct.IDDonHangMua == id && t.TenSP.Contains(tim)
                         select new ThongTinDonHang()
                         {
                             masp = t.MaSanPham,
                             tensp = t.TenSP,
                             hinhanh = t.HinhAnh,
                             dongia = t.GiaSP,
                             soluong = ct.SoLuong
                         }).ToList();
                float sum = 0;
                foreach (var item in KQTim)
                {
                    sum = sum + Convert.ToInt32(item.thanhtien);
                }
                ViewBag.sum = sum;
            }
            else if (KQTim.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../BaiDang/ThongTindonHang';</script>");
            }
            return View(KQTim.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult KQtimCTDH(int? page, FormCollection f)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string idDH = Session["idDH"].ToString();
            string tim = f["txttimkiem"];
            @ViewBag.tim = tim;
            List<ThongTinDonHang> KQTim = new List<ThongTinDonHang>();
            if(string.IsNullOrWhiteSpace(tim))
            {
                int id = int.Parse(idDH);
                KQTim = (from t in db.BaiDangs.ToList()
                            from ct in db.ChiTietGioHangs.ToList()
                            where t.MaSanPham == ct.MaSanPham && ct.IDDonHangMua == id
                            select new ThongTinDonHang()
                            {
                                masp = t.MaSanPham,
                                tensp = t.TenSP,
                                hinhanh = t.HinhAnh,
                                dongia = t.GiaSP,
                                soluong = ct.SoLuong
                            }).ToList();
                float sum = 0;
                foreach (var item in KQTim)
                {
                    sum = sum + Convert.ToInt32(item.thanhtien);
                }
                ViewBag.sum = sum;
            }
            else if (!string.IsNullOrWhiteSpace(tim))
            {
                int id = int.Parse(idDH);
                KQTim = (from t in db.BaiDangs.ToList()
                         from ct in db.ChiTietGioHangs.ToList()
                         where t.MaSanPham == ct.MaSanPham && ct.IDDonHangMua == id && t.TenSP.Contains(tim)
                         select new ThongTinDonHang()
                         {
                             masp = t.MaSanPham,
                             tensp = t.TenSP,
                             hinhanh = t.HinhAnh,
                             dongia = t.GiaSP,
                             soluong = ct.SoLuong
                         }).ToList();
                float sum = 0;
                foreach (var item in KQTim)
                {
                    sum = sum + Convert.ToInt32(item.thanhtien);
                }
                ViewBag.sum = sum;
            }
            else if(KQTim.Count == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Không Tìm Thấy Từ Khóa cần tìm');window.location.href='../BaiDang/ThongTindonHang';</script>");
            }
            return View(KQTim.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        public ActionResult Score(int diem, int masp)
        {           
                DanhGiaSanPham rate = new DanhGiaSanPham();
                rate.MaSanPham = masp;
                rate.IDUserMua = int.Parse(Session["id"].ToString());
                rate.NgayDanhGia = DateTime.Now;
                rate.DiemDanhGia = diem;
                db.DanhGiaSanPhams.Add(rate);

                BaiDang bd = (from i in db.BaiDangs
                               where i.MaSanPham == masp
                               select i).SingleOrDefault();
                if (bd.DanhGia == 0)
                {
                    bd.DanhGia = diem;
                    db.SaveChanges();
                }
                else
                {
                    var sum = (from i in db.DanhGiaSanPhams
                               where i.MaSanPham == masp
                               select i.DiemDanhGia).Sum();
                    List<DanhGiaSanPham> kq = new List<DanhGiaSanPham>();
                    kq = db.DanhGiaSanPhams.Where(n => n.MaSanPham == masp).ToList();
                    int tong = kq.Count + 1;
                    var result = ((float)sum + diem) / tong;
                    bd.DanhGia = Math.Round((float)result, 2);
                    db.SaveChanges();
                }

                
                return Content("<script language='javascript' type='text/javascript'>alert('Cảm Ơn Bạn Đã Đánh GIá Sản Phẩm Này!'); window.location.href = '../SPMoi/SPMoi';</script>");
        }
    }
}