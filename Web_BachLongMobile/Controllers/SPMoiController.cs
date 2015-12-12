using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TMDT.Models;
using PagedList.Mvc;
using PagedList;

namespace Web_TMDT.Controllers
{
    public class SPMoiController : Controller
    {
        //
        // GET: /SPMoi/
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SPMoi(int? page)
        {
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var bd = (from s in db.BaiDangs
                     where s.IDTrangThai == 2 && s.Soluongton > 0
                      orderby s.GiaSP descending
                     select s).ToList();
            return View(bd.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CTSP(int masp)
        {
            List<DanhGiaSanPham> dem = new List<DanhGiaSanPham>();
            dem = db.DanhGiaSanPhams.Where(n => n.MaSanPham == masp).ToList();
            int tong = dem.Count;
            var sp = (from bd in db.BaiDangs.ToList()
                      //from dg in db.ChiTietDanhGiaNguoiMuas.ToList()
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

        public ActionResult SanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var bd = (from s in db.BaiDangs
                      where s.IDTrangThai == 2 && s.Soluongton > 0
                      orderby s.GiaSP descending
                      select s).ToList();
            return View(bd.ToPagedList(pageNumber, pageSize));
        }
	}
}