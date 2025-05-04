using DoAn.Models;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    public class LienHesController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/LienHes
        public ActionResult Index()
        {

            var lienHes = db.LienHes.OrderByDescending(l => l.ngaygui).ToList();
            return View(lienHes);
        }

        // GET: Admin/LienHes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LienHe lienHe = db.LienHes.Find(id);
            if (lienHe == null)
            {
                return HttpNotFound();
            }
            return View(lienHe);
        }

        // POST: Admin/LienHes/UpdateStatus/5
        [HttpPost]
        public JsonResult UpdateStatus(int id, int status)
        {
            try
            {
                var lienHe = db.LienHes.Find(id);
                if (lienHe != null)
                {
                    lienHe.trangthai = status;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy liên hệ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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