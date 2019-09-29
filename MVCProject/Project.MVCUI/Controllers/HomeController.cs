using Projec.COMMON.MyTools;
using Project.BLL.RepositoryPattern.ConcRep;
using Project.MODEL.Entities;
using Project.MVCUI.Models.MyTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Controllers
{
    public class HomeController : Controller
    {
        AppUserRepository aprep;
        AppUserDetailRepository apdrep;
        public HomeController()
        {
            aprep = new AppUserRepository();
            apdrep = new AppUserDetailRepository();
        }

        // GET: Home
        public ActionResult Register()
        {
            //if (!aprep.CheckCredentials("asd","asdas@gmail.com"))
            //{
            //    ViewBag.Kayitli = "Kullanici zaten kayıtlıdır.";
            //    return View();
               //Ekle("Cay","12",400);
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Register( [Bind(Prefix ="Item1")]AppUser item,[Bind(Prefix ="Item2")]AppUserDetail item2)
        {
            if (aprep.Any(x=>x.UserName==item.UserName))
            {
                ViewBag.ZatenVar = "Lütfen baska bir kullanıcı ismi giriniz";
                return View();
            }
            else if (aprep.Any(x => x.Email == item.Email))
            {
                ViewBag.ZatenVar = "Bu Email zaten bizde kayıtlıdır";
            }
            //Kullanıcı basarılı bir şekilde register işlemini tamamlıyorsa ona mail gönder...

            string gonderilecekMail = "Tebrikler hesabınız olusturulmustur. Hesabınızı aktive etmek icin http://localhost:59157/Home/Aktivasyon/"+item.ActivationCode+" linkine tıklayabilirsiniz.";

            MailSender.Send(item.Email,password:"Cf8885++--",body:gonderilecekMail,subject:"Hesap Aktivasyon!");

            aprep.Add(item); // buradan sonra item'in id'si olusmus oluyor...O yüzden item2'nin id'sini verecek isek buradan sonra vermeliyiz.

            item2.ID = item.ID;

            apdrep.Add(item2);

            return View("RegisterOk");
        }

        public ActionResult Aktivasyon(Guid id)
        {
            if (aprep.Any(x=>x.ActivationCode==id))
            {
              AppUser aktiveEdilecek=  aprep.Default(x => x.ActivationCode == id);
                aktiveEdilecek.IsActive = true;
                aprep.Update(aktiveEdilecek);

               

                TempData["HesapAktif"]="Hesabınız aktif hale getirildi.";
                Session["member"] = aktiveEdilecek;
                if (Session["bekleyenUrun"]!=null)
                {
                    Product bekleyenUrun = Session["bekleyenUrun"] as Product;
                    CartItem ci = new CartItem();
                    ci.ID = bekleyenUrun.ID;
                    ci.Name = bekleyenUrun.ProductName;
                    ci.Price = bekleyenUrun.UnitPrice;
                    ci.ImagePath = bekleyenUrun.ImagePath;
                    Cart c = new Cart();
                    c.SepeteEkle(ci);

                    Session["scart"] = c;
                }

                return RedirectToAction("Index", "Member");
            }
            TempData["HesapAktif"] = "Aktif edilecek hesap bulunamadı";
            return RedirectToAction("Register");
        }

        public ActionResult RegisterOk()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AppUser item)
        {
            if (aprep.Any
                (
                x => x.UserName == item.UserName &&
                x.Password == item.Password &&
                x.Status != MODEL.Enums.DataStatus.Deleted &&
                x.IsBanned == false &&
                x.Role == MODEL.Enums.UserRole.Admin
                ))
            {
                
                AppUser girisYapan = aprep.Default(x => x.UserName == item.UserName && x.Password == item.Password);
                if (girisYapan.IsActive==false)
                {
                    ViewBag.AktifDegil = "Lutfen Hesabınızı aktif hale getiriniz";
                    return View("RegisterOk");
                }
                Session["admin"] = girisYapan;
                return RedirectToAction("");//burada Admin kendi areasına yönlendirilmeli
            }
            else if (aprep.Any
                (
                x => x.UserName == item.UserName &&
                x.Password == item.Password &&
                x.Status != MODEL.Enums.DataStatus.Deleted &&
                x.IsBanned == false &&
                x.Role == MODEL.Enums.UserRole.Member
                ))
            {
                AppUser girisYapan = aprep.Default(x => x.UserName == item.UserName && x.Password == item.Password);

                if (girisYapan.IsActive == false)
                {
                    ViewBag.AktifDegil = "Lutfen Hesabınızı aktif hale getiriniz";
                    return View("RegisterOk");

                
                }
                Session["member"] = girisYapan;
                if (Session["bekleyenUrun"]!=null)
                {
                    Product bekleyenUrun = Session["bekleyenUrun"] as Product;
                    CartItem ci = new CartItem();
                    ci.ID = bekleyenUrun.ID;
                    ci.Name = bekleyenUrun.ProductName;
                    ci.Price = bekleyenUrun.UnitPrice;
                    ci.ImagePath = bekleyenUrun.ImagePath;
                    Cart c = new Cart();
                    c.SepeteEkle(ci);

                    Session["scart"] = c;
                }
                return RedirectToAction("Index", "Member");
            }

            ViewBag.KullaniciBulunamadi = "Böyle bir kullanıcı yoktur";
            return View();
        }
    }
}