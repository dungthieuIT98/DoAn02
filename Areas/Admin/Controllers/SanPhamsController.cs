using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Services.Description;
using DoAn.Models;
using PagedList;

namespace DoAn.Areas.Admin.Controllers
{
    public class SanPhamsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/SanPhams
        public ActionResult Index(int ? page)
        {
            var searchString = Request.Form["search"];
            ViewBag.CurrentFilter = searchString;

            var sanPhams = db.SanPhams.Include(s => s.HangSP).ToList();
            sanPhams = sanPhams.OrderByDescending(s => s.masp).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhams = db.SanPhams.Where(sp => sp.tensp.Contains(searchString)).ToList();
            }
            
            int pageSize = 10; // Số lượng dữ liệu trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, nếu không được cung cấp thì mặc định là trang 1

            // Truy vấn dữ liệu từ nguồn dữ liệu và tạo đối tượng PagedList
            return View(sanPhams.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/SanPhams/Details/5
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
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang");
            List<SelectListItem> trangThaiList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Hiển thị" },
                new SelectListItem { Value = "false", Text = "Ẩn" }
            };
            ViewBag.TrangThaiList = trangThaiList;
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "masp,mahang,tensp,hinhanh,hinhanh2,hinhanh3,hinhanh4,hinhanh5,soluong,giaban,mota,CPU,RAM,OS,manhinh,carddohoa,SSD,trangthai,model")] SanPham sanPham)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle main image
                    sanPham.hinhanh = "";
                    var mainImage = Request.Files["ImageUpload"];
                    if (mainImage != null && mainImage.ContentLength > 0)
                    {
                        string originalFilename = Path.GetFileName(mainImage.FileName);
                        string fileExtension = Path.GetExtension(originalFilename);
                        string newFilename = Guid.NewGuid().ToString() + fileExtension; // Tạo tên file mới duy nhất
                        string filepath = Server.MapPath("~/wwwroot/images/" + newFilename);
                        mainImage.SaveAs(filepath);
                        sanPham.hinhanh = newFilename; // Lưu tên file mới vào database
                    }

                    // Handle additional images
                    int imageCount = 0;
                    // Bắt đầu từ index 1 vì index 0 là ImageUpload (hình chính)
                    for (int i = 1; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        // Chỉ xử lý các file được đặt tên là "ImageUploadPhu"
                        if (file != null && file.ContentLength > 0 && Request.Files.AllKeys[i].StartsWith("ImageUploadPhu") && imageCount < 4) // Limit to 4 additional images
                        {
                            string originalFilename = Path.GetFileName(file.FileName);
                            string fileExtension = Path.GetExtension(originalFilename);
                            string newFilename = Guid.NewGuid().ToString() + fileExtension; // Tạo tên file mới duy nhất
                            string filepath = Server.MapPath("~/wwwroot/images/" + newFilename);
                            file.SaveAs(filepath);

                            // Assign to the appropriate property based on count
                            switch (imageCount)
                            {
                                case 0:
                                    sanPham.hinhanh2 = newFilename;
                                    break;
                                case 1:
                                    sanPham.hinhanh3 = newFilename;
                                    break;
                                case 2:
                                    sanPham.hinhanh4 = newFilename;
                                    break;
                                case 3:
                                    sanPham.hinhanh5 = newFilename;
                                    break;
                            }
                            imageCount++;
                        }
                    }

                    db.SanPhams.Add(sanPham);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi nhập dữ liệu: " + ex.Message;
                ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang");
                return View(sanPham);
            }
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang", sanPham.mahang);
            List<SelectListItem> trangThaiList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Hiển thị" },
                new SelectListItem { Value = "false", Text = "Ẩn" }
            };
            ViewBag.TrangThaiList = trangThaiList;
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "masp,mahang,tensp,soluong,giaban,mota,CPU,RAM,OS,manhinh,carddohoa,SSD,trangthai,model")] SanPham sanPhamFromForm)
        {
            try
            {
                // Tải sản phẩm hiện có
                var existingSanPham = db.SanPhams.Find(sanPhamFromForm.masp);
                if (existingSanPham == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra ModelState và cập nhật các thuộc tính *không phải file*
                if (ModelState.IsValid)
                {
                    // Cập nhật các thuộc tính không phải ảnh từ form
                    existingSanPham.mahang = sanPhamFromForm.mahang;
                    existingSanPham.tensp = sanPhamFromForm.tensp;
                    existingSanPham.soluong = sanPhamFromForm.soluong;
                    existingSanPham.giaban = sanPhamFromForm.giaban;
                    existingSanPham.mota = sanPhamFromForm.mota;
                    existingSanPham.CPU = sanPhamFromForm.CPU;
                    existingSanPham.RAM = sanPhamFromForm.RAM;
                    existingSanPham.OS = sanPhamFromForm.OS;
                    existingSanPham.manhinh = sanPhamFromForm.manhinh;
                    existingSanPham.carddohoa = sanPhamFromForm.carddohoa;
                    existingSanPham.SSD = sanPhamFromForm.SSD;
                    existingSanPham.trangthai = sanPhamFromForm.trangthai;
                    existingSanPham.model = sanPhamFromForm.model;
                    // Xử lý upload nhiều file ảnh
                    var files = Request.Files;
                    if (files != null && files.Count > 0)
                    {
                        // Xử lý file hinhanh
                        var file1 = files["ImageUpload"];
                        if (file1 != null && file1.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file1.FileName);
                            var path = Path.Combine(Server.MapPath("~/wwwroot/images"), fileName);
                            file1.SaveAs(path);
                            existingSanPham.hinhanh = fileName;
                        }

                        // Xử lý file hinhanh2  
                        var file2 = files["ImageUpload2"];
                        if (file2 != null && file2.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file2.FileName);
                            var path = Path.Combine(Server.MapPath("~/wwwroot/images"), fileName);
                            file2.SaveAs(path);
                            existingSanPham.hinhanh2 = fileName;
                        }

                        // Xử lý file hinhanh3
                        var file3 = files["ImageUpload3"];
                        if (file3 != null && file3.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file3.FileName);
                            var path = Path.Combine(Server.MapPath("~/wwwroot/images"), fileName);
                            file3.SaveAs(path);
                            existingSanPham.hinhanh3 = fileName;
                        }

                        // Xử lý file hinhanh4
                        var file4 = files["ImageUpload4"];
                        if (file4 != null && file4.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file4.FileName);
                            var path = Path.Combine(Server.MapPath("~/wwwroot/images"), fileName);
                            file4.SaveAs(path);
                            existingSanPham.hinhanh4 = fileName;
                        }

                        // Xử lý file hinhanh5
                        var file5 = files["ImageUpload5"]; 
                        if (file5 != null && file5.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file5.FileName);
                            var path = Path.Combine(Server.MapPath("~/wwwroot/images"), fileName);
                            file5.SaveAs(path);
                            existingSanPham.hinhanh5 = fileName;
                        }
                    }

                    // EF tự động theo dõi thay đổi trên existingSanPham
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                // Nếu ModelState không hợp lệ
                // Cần trả lại View với model `sanPhamFromForm`
                // Các trường hình ảnh sẽ hiển thị giá trị hiện tại từ DB (vì chúng không được submit cùng form này)
                // Ta cần lấy lại các giá trị hinhanh từ existingSanPham để hiển thị đúng
                sanPhamFromForm.hinhanh = existingSanPham.hinhanh;
                sanPhamFromForm.hinhanh2 = existingSanPham.hinhanh2;
                sanPhamFromForm.hinhanh3 = existingSanPham.hinhanh3;
                sanPhamFromForm.hinhanh4 = existingSanPham.hinhanh4;
                sanPhamFromForm.hinhanh5 = existingSanPham.hinhanh5;

                // Thiết lập lại ViewBag cần thiết cho view
                ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang", sanPhamFromForm.mahang);
                List<SelectListItem> trangThaiList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Hiển thị" },
                    new SelectListItem { Value = "false", Text = "Ẩn" }
                };
                ViewBag.TrangThaiList = trangThaiList;
                ViewBag.Error = "Dữ liệu nhập không hợp lệ.";
                return View(sanPhamFromForm);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Xử lý lỗi tương tranh...
                // (Giữ nguyên logic xử lý lỗi tương tranh)
                var entry = ex.Entries.Single();
                var clientValues = (SanPham)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lưu thay đổi. Sản phẩm đã bị xóa bởi người dùng khác.");
                }
                else
                {
                    var databaseValues = (SanPham)databaseEntry.ToObject();
                    ModelState.AddModelError(string.Empty, "Sản phẩm bạn đang sửa đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang.");
                    sanPhamFromForm = databaseValues; // Gán lại giá trị từ DB
                }
                ViewBag.Error = "Lỗi cập nhật dữ liệu (tương tranh). Vui lòng thử lại.";
                 // Thiết lập lại ViewBag khi có lỗi tương tranh và trả về view
                ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang", sanPhamFromForm.mahang);
                List<SelectListItem> trangThaiListConcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Hiển thị" },
                    new SelectListItem { Value = "false", Text = "Ẩn" }
                };
                ViewBag.TrangThaiList = trangThaiListConcurrency;
                return View(sanPhamFromForm);
            }
            catch (Exception ex)
            {
                // Log lỗi ex...
                ViewBag.Error = "Có lỗi hệ thống xảy ra trong quá trình cập nhật sản phẩm.";
                // Nếu lỗi xảy ra trước khi load existingSanPham hoặc sau khi load
                // Cố gắng gán lại ảnh gốc nếu có thể để hiển thị
                 if (db.SanPhams.AsNoTracking().FirstOrDefault(p => p.masp == sanPhamFromForm.masp) is SanPham currentDbSanPham)
                 {
                    sanPhamFromForm.hinhanh = currentDbSanPham.hinhanh;
                    sanPhamFromForm.hinhanh2 = currentDbSanPham.hinhanh2;
                    sanPhamFromForm.hinhanh3 = currentDbSanPham.hinhanh3;
                    sanPhamFromForm.hinhanh4 = currentDbSanPham.hinhanh4;
                    sanPhamFromForm.hinhanh5 = currentDbSanPham.hinhanh5;
                 }
                // Thiết lập lại ViewBag khi có lỗi chung
                ViewBag.mahang = new SelectList(db.HangSPs, "mahang", "tenhang", sanPhamFromForm.mahang);
                List<SelectListItem> trangThaiListError = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Hiển thị" },
                    new SelectListItem { Value = "false", Text = "Ẩn" }
                };
                ViewBag.TrangThaiList = trangThaiListError;
                return View(sanPhamFromForm);
            }
        }

        // GET: Admin/SanPhams/Delete/5
        public ActionResult Delete(int? id)
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
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanPham);
            db.SaveChanges();
            return Json(new {success = true});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
