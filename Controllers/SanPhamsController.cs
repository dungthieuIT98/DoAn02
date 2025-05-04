using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DoAn.Models;
using PagedList;

namespace DoAn.Controllers
{
    public class SanPhamsController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index(int? page, int[] brands, string price, string rating)
        {
            var searchString = Request.Form["timkiem"];
            int pageSize = 20;
            int pageNumber = page ?? 1;
            List<SanPham> sanPhams = db.SanPhams.Include(s => s.HangSP).Where(s => s.trangthai == true).ToList();

            // Get review counts and average ratings for each product
            var productRatings = db.DanhGiaSanPhams
                .GroupBy(d => d.MaSanPham)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Count = g.Count(),
                    AvgRating = g.Average(x => x.DiemDanhGia)
                })
                .ToList();

            // Create ProductViewModel list
            var productViewModels = sanPhams.Select(sp => new ProductViewModel
            {
                Product = sp,
                ReviewCount = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.Count ?? 0,
                AverageRating = productRatings.FirstOrDefault(x => x.ProductId == sp.masp)?.AvgRating ?? 0
            }).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                productViewModels = productViewModels.Where(sp => sp.Product.tensp.ToLower().Contains(searchString)).ToList();
            }

            // Handle brand filtering
            if (brands != null)
            {
                if (brands.Length > 0)
                {
                    productViewModels = productViewModels.Where(sp => brands.Contains(sp.Product.HangSP.mahang)).ToList();
                }
                // If brands array is empty, show all products
            }
            if (!string.IsNullOrEmpty(price))
            {
                switch (price)
                {
                    case "price-1":
                        productViewModels = productViewModels.OrderBy(s => s.Product.giaban).ToList();
                        break;
                    case "price-2":
                        productViewModels = productViewModels.OrderByDescending(s => s.Product.giaban).ToList();
                        break;
                    case "price-3":
                        productViewModels = productViewModels.Where(s => s.Product.giaban <= 10000000).ToList();
                        break;
                    case "price-4":
                        productViewModels = productViewModels.Where(s => s.Product.giaban >= 10000000 && s.Product.giaban <= 15000000).ToList();
                        break;
                    case "price-5":
                        productViewModels = productViewModels.Where(s => s.Product.giaban > 15000000 && s.Product.giaban <= 20000000).ToList();
                        break;
                    case "price-6":
                        productViewModels = productViewModels.Where(s => s.Product.giaban > 20000000 && s.Product.giaban <= 25000000).ToList();
                        break;
                    case "price-7":
                        productViewModels = productViewModels.Where(s => s.Product.giaban > 25000000).ToList();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(rating))
            {
                switch (rating)
                {
                    case "cus-rating-1":
                        productViewModels = productViewModels.Where(p => p.AverageRating >= 4.5).ToList();
                        break;
                    case "cus-rating-2":
                        productViewModels = productViewModels.Where(p => p.AverageRating >= 3.5 && p.AverageRating < 4.5).ToList();
                        break;
                    case "cus-rating-3":
                        productViewModels = productViewModels.Where(p => p.AverageRating >= 2.5 && p.AverageRating < 3.5).ToList();
                        break;
                    case "cus-rating-4":
                        productViewModels = productViewModels.Where(p => p.AverageRating >= 1.5 && p.AverageRating < 2.5).ToList();
                        break;
                    case "cus-rating-5":
                        productViewModels = productViewModels.Where(p => p.AverageRating < 1.5).ToList();
                        break;
                }
            }

            List<HangSP> hsp = db.HangSPs
                .Where(h => db.SanPhams.Any(s => s.mahang == h.mahang))
                .ToList();

            ViewBag.dsh = hsp;
            ViewBag.searchString = searchString;
            ViewBag.selectedBrands = brands;
            ViewBag.selectedPrice = price;
            ViewBag.selectedRating = rating;
            return View(productViewModels.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            List<SanPham> listsp = db.SanPhams.Where(s => s.mahang == sanPham.mahang).ToList();
            listsp.Remove(sanPham);
            // Get review counts for each product
            var reviewCounts = db.DanhGiaSanPhams
                .GroupBy(d => d.MaSanPham)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .ToList();

            var saleViewModels = listsp.Select(sp => new ProductViewModel
            {
                Product = sp,
                ReviewCount = reviewCounts.FirstOrDefault(x => x.ProductId == sp.masp)?.Count ?? 0
            }).ToList();

            ViewBag.ds = saleViewModels;

            // Lấy danh sách đánh giá của sản phẩm
            var diemTrungBinh = 0.0;
            var soLuongDanhGia = 0;


            // Lấy danh sách đánh giá với thông tin khách hàng sử dụng LINQ to SQL
            var danhGias = (from dg in db.DanhGiaSanPhams
                            join kh in db.KhachHangs on dg.MaNguoiDung equals kh.makhachhang
                            where dg.MaSanPham == id
                            select new DanhGiaSanPham1
                            {
                                MaDanhGia = dg.MaDanhGia,
                                MaSanPham = dg.MaSanPham,
                                MaNguoiDung = dg.MaNguoiDung,
                                DiemDanhGia = dg.DiemDanhGia,
                                NoiDungDanhGia = dg.NoiDungDanhGia,
                                NgayDanhGia = dg.NgayDanhGia,
                                KhachHang = kh.hoten
                            })
                        .OrderByDescending(d => d.NgayDanhGia)
                        .ToList<dynamic>();

            if (danhGias != null && danhGias.Any())
            {
                diemTrungBinh = danhGias.Average(d => d.DiemDanhGia);
                soLuongDanhGia = danhGias.Count;
            }


            ViewBag.DanhGias = danhGias;
            ViewBag.DiemTrungBinh = diemTrungBinh;
            ViewBag.SoLuongDanhGia = soLuongDanhGia;

            return View(sanPham);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddCart(int id)
        {
            if (Session["ma"] == null)
            {
                string returnUrl = Url.Action("Index", "SanPhams");
                Session["ReturnUrl"] = returnUrl;

                return Json(new { success = false });
            }
            else
            {
                SanPham sp = db.SanPhams.Find(id);
                int ma = (int)Session["ma"];
                var invalid = db.GioHangs.Any(x => x.makhachhang == ma && x.masp == id);
                if (invalid)
                {
                    var existingCart = db.GioHangs.FirstOrDefault(x => x.makhachhang == ma && x.masp == id);
                    if (existingCart != null)
                    {
                        if (existingCart.soluong <= sp.soluong)
                        {
                            existingCart.soluong += 1;
                            db.SaveChanges();
                            return Json(new { success = true, message = "T" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "F" });
                        }
                    }
                }
                GioHang gh = new GioHang();
                gh.makhachhang = (int)Session["ma"];
                gh.masp = id;
                gh.soluong = 1;
                db.GioHangs.Add(gh);
                db.SaveChanges();
                Session["giohang"] = (int)Session["giohang"] + 1;
                return Json(new { success = true });
            }
        }
        public ActionResult AddCart1(int id)
        {
            if (Session["ma"] == null)
            {
                string returnUrl = Url.Action("Index", "Home");
                Session["ReturnUrl"] = returnUrl;

                return Json(new { success = false });
            }
            else
            {
                SanPham sp = db.SanPhams.Find(id);

                int ma = (int)Session["ma"];
                var invalid = db.GioHangs.Any(x => x.makhachhang == ma && x.masp == id); if (invalid)
                {
                    var existingCart = db.GioHangs.FirstOrDefault(x => x.makhachhang == ma && x.masp == id);
                    if (existingCart != null)
                    {
                        if (existingCart.soluong <= sp.soluong)
                        {
                            existingCart.soluong += 1;
                            db.SaveChanges();
                            return Json(new { success = true, message = "T" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "F" });
                        }
                    }
                }
                GioHang gh = new GioHang();
                gh.makhachhang = ma;
                gh.masp = id;
                gh.soluong = 1;
                db.GioHangs.Add(gh);
                db.SaveChanges();
                Session["giohang"] = (int)Session["giohang"] + 1;
                return Json(new { success = true });
            }
        }
        public ActionResult AddCart2(int id, int soluong)
        {
            if (Session["ma"] == null)
            {
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues["id"] = id;
                string returnUrl = Url.Action("Details", "SanPhams", routeValues);
                Session["ReturnUrl"] = returnUrl;
                return Json(new { success = false });
            }
            else
            {
                SanPham sp = db.SanPhams.Find(id);
                if (soluong > sp.soluong)
                {
                    return Json(new { success = true, message = "F" });
                }
                else
                {
                    int ma = (int)Session["ma"];
                    var invalid = db.GioHangs.Any(x => x.makhachhang == ma && x.masp == id); if (invalid)
                    {
                        var existingCart = db.GioHangs.FirstOrDefault(x => x.makhachhang ==ma && x.masp == id);
                        if (existingCart != null)
                        {
                            if (existingCart.soluong + soluong <= sp.soluong)
                            {
                                existingCart.soluong += soluong;
                                db.SaveChanges();
                                return Json(new { success = true, message = "T" });
                            }
                            else
                            {
                                return Json(new { success = true, message = "F" });
                            }
                        }
                    }

                    GioHang gh = new GioHang();
                    gh.makhachhang =ma;
                    gh.masp = id;
                    gh.soluong = soluong;
                    db.GioHangs.Add(gh);
                    db.SaveChanges();
                    Session["giohang"] = (int)Session["giohang"] + 1;
                    return Json(new { success = true, message = "T" });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int MaSanPham, int DiemDanhGia, string NoiDungDanhGia)
        {
            if (Session["ma"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                var danhGia = new DanhGiaSanPham
                {
                    MaSanPham = MaSanPham,
                    MaNguoiDung = (int)Session["ma"],
                    DiemDanhGia = DiemDanhGia,
                    NoiDungDanhGia = NoiDungDanhGia,
                    NgayDanhGia = DateTime.Now
                };

                db.DanhGiaSanPhams.Add(danhGia);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = MaSanPham });
            }

            return RedirectToAction("Details", new { id = MaSanPham });
        }
    }
}
