﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAn.Models;

namespace DoAn.Controllers
{
    public class DonHangsController : Controller
    {
        private Model1 db = new Model1();

        // GET: DonHangs
        public ActionResult Index()
        {
            List<DonHang> donHangs = db.DonHangs.Include(d => d.KhachHang).ToList();
            donHangs = donHangs.OrderByDescending(d => d.madonhang).ToList();
            return View(donHangs);
        }

        public ActionResult ChiTietDonHang(int? id)
        {
            DonHang donHang = db.DonHangs.Find(id);
            List<DonHangChiTiet> listDHCT = new List<DonHangChiTiet>();
            listDHCT = db.DonHangChiTiets.Where(s => s.madonhang == id).ToList();
            ViewBag.DHCT = listDHCT;
            return View(donHang);
        }

        public ActionResult Huy(int id)
        {
            DonHang donHang = db.DonHangs.Find(id);
            //List<DonHangChiTiet> listDHCT = new List<DonHangChiTiet>();
            //listDHCT = db.DonHangChiTiets.Where(s => s.madonhang == id).ToList();
            //listDHCT.Clear();
            //db.DonHangs.Remove(donHang);
            donHang.trangthai = null;
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
