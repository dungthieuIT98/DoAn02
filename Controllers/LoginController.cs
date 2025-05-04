using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class LoginController : Controller
    {
        Model1 db = new Model1();
        // GET: Login
        public ActionResult Index()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult DangNhap()
        {
            string mail = Request.Form["email"];
            string password = Request.Form["password"];
            var user = db.KhachHangs.Where(s => s.email == mail && s.matkhau == password).FirstOrDefault();

            if (user == null)
            {
                Console.WriteLine(password);
                return RedirectToAction("Index", "Login");

            }
            else
            {
                if(user.chucvu == true)
                {
                    return RedirectToAction("Index", "Admin/Login");
                }
                Session["ma"] = user.makhachhang;
                Session["hoten"] = user.hoten;
                Session["giohang"] = db.GioHangs.Where(s=>s.makhachhang == user.makhachhang).ToList().Count;
                string returnUrl = Session["ReturnUrl"] as string;
                Session.Remove("ReturnUrl");
                Session.Timeout = 100;
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    // Chuyển hướng đến returnUrl nếu tồn tại
                    return Redirect(returnUrl);  
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
        }
        

        [HttpPost]     
        public ActionResult DangKy(string hoten, string email, string pass, string repass)
        {
            //string hoten = Request.Form["rhoten"];
            //string mail = Request.Form["remail"];
            //string password = Request.Form["rpassword"];
            //string re_password = Request.Form["rrpassword"];
            if(pass == repass)
            {
                KhachHang kh = new KhachHang();
                kh.hoten = hoten;
                kh.email = email;
                kh.matkhau = pass;
                kh.ngaydangky = DateTime.Now;
                db.KhachHangs.Add(kh);
                db.SaveChanges();
                return Json(new {success = true});
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult SignOut()
        {
            Session["ma"] = null;
            Session["hoten"] = null;
            Session["giohang"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.KhachHangs.Where(s => s.email == email).FirstOrDefault();
            if (user != null)
            {
                // Generate a new random password
                string newPassword = System.Web.Security.Membership.GeneratePassword(8, 1);
                
                // Update user's password
                user.matkhau = newPassword;
                db.SaveChanges();

                // Send email with new password
                try
                {
                    string fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailAddress"];
                    string emailPassword = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
                    string smtpServer = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                    int smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);

                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    mail.To.Add(email);
                    mail.From = new System.Net.Mail.MailAddress(fromEmail);
                    mail.Subject = "Khôi phục mật khẩu - NEOTECH Computer";
                    mail.Body = $"Xin chào {user.hoten},<br><br>" +
                               $"Mật khẩu mới của bạn là: <strong>{newPassword}</strong><br><br>" +
                               $"Vui lòng đăng nhập và thay đổi mật khẩu sau khi đăng nhập thành công.<br><br>" +
                               $"Trân trọng,<br>NEOTECH Computer";
                    mail.IsBodyHtml = true;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = smtpServer;
                    smtp.Port = smtpPort;
                    smtp.Credentials = new System.Net.NetworkCredential(fromEmail, emailPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                    return Json(new { success = true, message = "Mật khẩu mới đã được gửi đến email của bạn." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Email không tồn tại trong hệ thống." });
            }
        }

        public ActionResult Register(string hoten, string email, string pass, string repass)
        {
            if (pass == repass)
            {
                KhachHang kh = new KhachHang();
                kh.hoten = hoten;
                kh.email = email;
                kh.matkhau = pass;
                db.KhachHangs.Add(kh);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}