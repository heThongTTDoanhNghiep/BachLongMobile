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
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        HTTT_DoanhNghiepEntities db = new HTTT_DoanhNghiepEntities();
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult LoaiSPPartial()
        {
            var hieu = db.LoaiSanPhams.ToList();
            return PartialView(hieu);
        }
        /*dien thoai theo chu de*/
        public ViewResult SPtheo_loai(int maloai, int? page)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            List<BaiDang> listsp = db.BaiDangs.Where(n => n.MaLoai == maloai && n.IDTrangThai == 2 && n.Soluongton > 0).OrderBy(n => n.NgayDang).ToList();
            if (listsp.Count == 0)
            {
                ViewBag.SP = "Không có sản phẩm nào thuộc loại này";
            }
            ViewBag.SP = "Sản Phẩm Cần Tìm!";
            return View(listsp.ToPagedList(pageNumber, pageSize));
            
        }   
        public PartialViewResult TimKiem()
        {
            List<SelectListItem> thang = new List<SelectListItem>();
            for (int i = 0; i <= 30000000; i = i+1000000)
            {
                SelectListItem one = new SelectListItem() { Text = i.ToString(), Value = i.ToString() };
                thang.Add(one);
            }
            ViewBag.thang = thang;
            List<SelectListItem> nam = new List<SelectListItem>();
            int aa = Convert.ToInt32(DateTime.Now.Year);
            for (int i = 0; i <= 30000000; i = i+1000000)
            {
                SelectListItem one = new SelectListItem() { Text = i.ToString(), Value = i.ToString() };
                nam.Add(one);
            }
            ViewBag.nam = nam;
            ViewBag.MaLoai = new SelectList(db.LoaiSanPhams.ToList(), "MaLoai", "TenLoai");
            return PartialView();
        }


        public PartialViewResult TimKiemTT()
        {
            return PartialView();
        }
        public PartialViewResult TimCTDH()
        {
            return PartialView();
        }
        
	}
}