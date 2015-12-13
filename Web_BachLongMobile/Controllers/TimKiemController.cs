using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TMDT.Models;
using PagedList;
using PagedList.Mvc;

namespace Web_TMDT.Controllers
{
    public class TimKiemController : Controller
    {
        //
        // GET: /TimKiem/
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult KQTim(int? page, string tim, string loai, string tu, string den)
        {

            @ViewBag.tu = tu;
            @ViewBag.den = den;
            @ViewBag.tim = tim;
            @ViewBag.loai = loai;

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            List<BaiDang> KQTimKiem = new List<BaiDang>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                KQTimKiem = (from n in db.BaiDangs
                             where n.IDTrangThai == 2 && n.Soluongton > 0
                             select n).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                return View();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP <= giaden && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && !string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.TenSP.Contains(tim) && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.MaLoai == timloai && n.GiaSP <= giaden && n.GiaSP >= giatu && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }

            if (KQTimKiem.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm cần tìm!";
            }
            return View(KQTimKiem.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult KQTim(int? page, FormCollection f)
        {
            
            string tim = f["txttimkiem"].ToString();           
            string loai = (Request["MaLoai"]).ToString();
            string tu = f["thang"];
            string den = f["nam"];

            @ViewBag.tu = tu;
            @ViewBag.den = den;
            @ViewBag.tim = tim;
            @ViewBag.loai = loai;

            int pageSize = 8;
            int pageNumber = (page ?? 1);

            List<BaiDang> KQTimKiem = new List<BaiDang>();

            if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                KQTimKiem = (from n in db.BaiDangs
                             where n.IDTrangThai == 2 && n.Soluongton > 0
                             select n).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                return View();
            }
            else if (!string.IsNullOrWhiteSpace(tim) && string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if(string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP <= giaden && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(loai) && string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (string.IsNullOrWhiteSpace(loai) && !string.IsNullOrWhiteSpace(tim) && !string.IsNullOrEmpty(tu) && !string.IsNullOrEmpty(den))
            {
                int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                KQTimKiem = db.BaiDangs.Where(n => n.GiaSP >= giatu && n.GiaSP <= giaden && n.TenSP.Contains(tim) && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else if (!string.IsNullOrWhiteSpace(tim) && !string.IsNullOrWhiteSpace(loai) && string.IsNullOrEmpty(tu) && string.IsNullOrEmpty(den))
            {
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.MaLoai == timloai && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }
            else
            {
                 int giatu = int.Parse(tu);
                int giaden = int.Parse(den);
                int timloai = int.Parse(loai);
                KQTimKiem = db.BaiDangs.Where(n => n.TenSP.Contains(tim) && n.MaLoai == timloai && n.GiaSP <= giaden  && n.GiaSP >= giatu  && n.IDTrangThai == 2 && n.Soluongton > 0).ToList();
                ViewBag.ThongBao = "Sản Phẩm Cần Tìm!";
            }

            if (KQTimKiem.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm cần tìm!";
            }
            return View(KQTimKiem.ToPagedList(pageNumber, pageSize));
        }
    }
}