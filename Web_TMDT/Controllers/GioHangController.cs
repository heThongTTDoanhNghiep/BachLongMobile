using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TMDT.Models;

namespace Web_TMDT.Controllers
{
    public class GioHangController : Controller
    {
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
#region Giỏ Hàng
        //lấy giỏ hàng
        public List<GioHang> LayGioHang()
        {
            List<GioHang> list = Session["GioHang"] as List<GioHang>;
            if(list==null)
            {
                list=new List<GioHang>();
                Session["GioHang"] = list;
            }
            return list;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGH(int masp, string url)
        {
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == masp);
            if(bd==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<GioHang> list=LayGioHang();
            GioHang sp = list.Find(n => n.masp == masp);
            if(sp==null)
            {
                sp = new GioHang(masp);
                list.Add(sp);
                return Redirect(url);
            }
            else
            {
                if(sp.soluong>=bd.Soluongton)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Số lượng mua vượt quá số lượng có');window.location.href='../SPMoi/SPMoi';</script>");
                }
                else
                {
                    sp.soluong++;
                    return Redirect(url);
                }
               
            }
        }

        //Cập nhật giỏ hàng
        public ActionResult CapNhatGH(int masp, FormCollection f)
        {
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == masp);
            if(bd==null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            List<GioHang> list = LayGioHang();
            GioHang sp = list.Find(n => n.masp == masp);
            if (sp == null || bd.Soluongton < int.Parse(f["txtSoLuong"].ToString()))
            {
               
                return Content("<script language='javascript' type='text/javascript'>alert('Số lượng mua vượt quá số lượng có');window.location.href='../GioHang/GioHang';</script>");
            }
            else
            {
                sp.soluong = int.Parse(f["txtSoLuong"].ToString());
                return RedirectToAction("GioHang");
            }
                
           
           
        }
        //Xoá Giỏ Hàng
        public ActionResult XoaGH(int masp)
        {
            BaiDang bd = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == masp);
            if (bd == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            List<GioHang> list = LayGioHang();
            GioHang sp = list.Find(n => n.masp == masp);
            if (sp != null)
            {
                list.RemoveAll(n => n.masp == masp);
            }
            if(list.Count==0)
            {
                return RedirectToAction("SPMoi","SPMoi");
            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if(Session["GioHang"]==null)
            {
                return RedirectToAction("SPMoi", "SPMoi");
            }
            List<GioHang> list = LayGioHang();
            ViewBag.tongtien = tongtien();
            return View(list);
        }
        //Tổng số lượng
        public int tongsl()
        {
            int sl = 0;
            List<GioHang> list = Session["GioHang"] as List<GioHang>;
            if(list!=null)
            {
                sl = list.Sum(n => n.soluong);
            }
            return sl;
        }
        //Tổng tiền
        public float tongtien()
        {
            float tong = 0;
            List<GioHang> list = Session["GioHang"] as List<GioHang>;
            if(list!=null)
            {
                tong = list.Sum(n => n.thanhtien);
            }
            return tong;
        }
        public ActionResult GioHangPartial()
        {
            if(tongsl()==0)
            {
                return PartialView();
            }
            ViewBag.tongsoluong = tongsl();
            ViewBag.tongtien = tongtien();
            return PartialView(); ;
        }

        public ActionResult SuaGH(int masp)
        {
            if(Session["GioHang"]==null)
            {
                RedirectToAction("SPMoi", "SPMoi");
            }
            List<GioHang> list = LayGioHang();
            return View(list);
        }

        public ActionResult DatHang()
        {
            //kiểm tra đăng nhập
            if(Session["Name"]==null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Bạn Chưa đăng nhập ');window.location.href='../Users/Login';</script>");
            }
            //thêm đơn hàng
            User user = (User)Session["NameUser"];
            GioHang_DonHangMua dh = new GioHang_DonHangMua();
            List<GioHang> gh = LayGioHang();
            dh.IDUserMua = user.IDUser;
            dh.NgayLap =  DateTime.Now;
            dh.TongTien = tongtien();
            dh.IDTrangThai = 2;
            db.GioHang_DonHangMua.Add(dh);
            
            db.SaveChanges();
            //thêm chi tiết đơn hàng
            foreach(var item in gh)
            {
                ChiTietGioHang ct = new ChiTietGioHang();
                ct.IDDonHangMua = dh.IDDonHangMua;
                ct.MaSanPham = item.masp;
                ct.SoLuong = item.soluong;
                ct.ThanhTien = item.thanhtien;
                BaiDang s = db.BaiDangs.SingleOrDefault(n => n.MaSanPham == item.masp);
                db.ChiTietGioHangs.Add(ct);
                //cập nhật số lượng sản phẩm của bài đăng
                var sp = db.BaiDangs.Find(s.MaSanPham);
                sp.MaSanPham = s.MaSanPham;
                sp.Soluongton = s.Soluongton - item.soluong;
                if(sp.Soluongton==0 )
                {
                    sp.IDTrangThai = 1;
                }
                else
                {
                    sp.IDTrangThai = s.IDTrangThai;
                }
               
                db.SaveChanges();
            }
            
            return Content("<script language='javascript' type='text/javascript'>alert('Đơn hàng đã được lập ');window.location.href='../GioHang/Thongtin';</script>");
           
        }
        
        #endregion


        public List<GioHang> LayThongTin()
        {
            List<GioHang> list = Session["GioHang"] as List<GioHang>;
            if (list == null)
            {
                list = new List<GioHang>();
                Session["GioHang"] = list;
            }
            ViewBag.tongtien = tongtien();
            return list;
        }
        public ActionResult Thongtin()
        {
            List<GioHang> list = LayThongTin();
            Session["GioHang"] = null;
            return View(list);
        }
	}
}