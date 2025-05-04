using DoAn.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();
        public ActionResult Index()
        {
            List<SanPham> ds = new List<SanPham>();
            List<SanPham> ds_sale = db.SanPhams.Where(s => s.trangthai == true).Take(10).ToList();
            List<SanPham> ds_new = db.SanPhams.Where(s => s.trangthai == true).OrderByDescending(s => s.masp).Take(15).ToList();
            List<HangSP> ds_hang = db.HangSPs
                .Where(h => db.SanPhams.Any(s => s.mahang == h.mahang))
                .ToList();

            // Get review counts and average ratings for all products
            var productRatings = db.DanhGiaSanPhams
                .GroupBy(d => d.MaSanPham)
                .Select(g => new { 
                    ProductId = g.Key, 
                    Count = g.Count(),
                    AvgRating = g.Average(x => x.DiemDanhGia)
                })
                .ToList();

            // Create ProductViewModel lists for sale and new products
            var saleViewModels = ds_sale.Select(sp => new ProductViewModel
            {
                Product = sp,
                ReviewCount = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.Count ?? 0,
                AverageRating = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.AvgRating ?? 0
            }).ToList();

            var newViewModels = ds_new.Select(sp => new ProductViewModel
            {
                Product = sp,
                ReviewCount = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.Count ?? 0,
                AverageRating = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.AvgRating ?? 0
            }).ToList();

            ViewBag.sale = saleViewModels;
            ViewBag.dsnew = newViewModels;
            Session["dshang"] = ds_hang;
            Session.Timeout = 100;
            ViewBag.dsh = ds_hang;

            return View();
        }

        public ActionResult DetailProduct()
        {
            return View();
        }


        public ActionResult LienHe()
        {
            return View();
        }

        public ActionResult TinTuc(int? page)
        {
            int pageSize = 9; // Số tin tức trên mỗi trang
            int pageNumber = (page ?? 1);
            
            var newsList = db.TinTucs
                .Where(t => t.trangthai == 1)
                .OrderByDescending(t => t.ngaytao)
                .ToList();
                
            return View(newsList);
        }

        public ActionResult ChiTietTinTuc(int id)
        {
            var news = db.TinTucs
                .Include(t => t.KhachHang) // Include thông tin KhachHang
                .FirstOrDefault(t => t.matintuc == id);
                
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        [HttpPost]
        public JsonResult LuuLienHe(LienHe lienHe)
        {
            try
            {
                lienHe.ngaygui = System.DateTime.Now;
                lienHe.trangthai = 0; // Chưa xử lý
                db.LienHes.Add(lienHe);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult HuongDanDatHang()
        {
            return View();
        }

        public ActionResult ChinhSachBaoHanh()
        {
            return View();
        }

        public ActionResult ChinhSachVanChuyen()
        {
            return View();
        }

        public ActionResult ChinhSachKiemHang()
        {
            return View();
        }
    }
}