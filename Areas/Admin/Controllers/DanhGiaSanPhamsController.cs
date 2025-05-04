using DoAn.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace DoAn.Areas.Admin.Controllers
{
    public class DanhGiaSanPhamsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/DanhGiaSanPhams
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.CurrentFilter = searchString;

            var danhGias = db.DanhGiaSanPhams.Include(d => d.SanPham).Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayDanhGia);

            if (!string.IsNullOrEmpty(searchString))
            {
                danhGias = danhGias.Where(d => 
                    d.SanPham.tensp.Contains(searchString) || 
                    d.KhachHang.email.Contains(searchString) ||
                    d.NoiDungDanhGia.Contains(searchString))
                    .OrderByDescending(d => d.NgayDanhGia);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(danhGias.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/DanhGiaSanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGiaSanPham danhGia = db.DanhGiaSanPhams.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // GET: Admin/DanhGiaSanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGiaSanPham danhGia = db.DanhGiaSanPhams.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // POST: Admin/DanhGiaSanPhams/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhGiaSanPham danhGia = db.DanhGiaSanPhams.Find(id);
            db.DanhGiaSanPhams.Remove(danhGia);
            db.SaveChanges();
            return Json(new { success = true });
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