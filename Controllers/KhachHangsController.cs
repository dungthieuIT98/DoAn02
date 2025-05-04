using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn.Models;

namespace DoAn.Controllers
{
    public class KhachHangsController : Controller
    {
        private Model1 db = new Model1();

        // GET: KhachHangs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserInfor()
        {
            if (Session["ma"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            KhachHang khachHang = db.KhachHangs.Find((int)Session["ma"]);
            if (khachHang == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Load related data
            var khachHangWithDetails = db.KhachHangs
                .Include("DonHangs")
                .Include("GioHangs")
                .FirstOrDefault(k => k.makhachhang == khachHang.makhachhang);

            if (khachHangWithDetails == null)
            {
                return HttpNotFound();
            }
            Session["taikhoan"] = khachHangWithDetails;
            return View(khachHangWithDetails);
        }
    }
}