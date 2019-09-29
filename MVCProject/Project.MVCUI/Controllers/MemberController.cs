using PagedList;
using Project.BLL.RepositoryPattern.ConcRep;
using Project.MODEL.Entities;
using Project.MVCUI.Models.MyTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Controllers
{

    public class MemberController : Controller
    {
        ProductRepository prep;
        OrderRepository orep;
        OrderDetailRepository odrep;
        CategoryRepository crep;
        public MemberController()
        {
            prep = new ProductRepository();
            orep = new OrderRepository();
            odrep = new OrderDetailRepository();
            crep = new CategoryRepository();
        }
        // GET: Member
        //Pagination yapacak isek öncelikle Action'ımıza null gecilebilir bir int tipi vermeliyiz.
        public ActionResult Index(int? page)
        {
            /*
              string a = null;
              a="Changed";
            //eger a null ise istedigim bir deger halini alsın. Eger degilse kendi degerini atsın..
               string b  = a??"Hello";
             
             */


            return View(Tuple.Create(crep.SelectActives(), prep.SelectActives().ToPagedList(page ?? 1, 9)));
        }

        public ActionResult GetByCategory(int id, int? page)
        {


            return View(Tuple.Create(prep.Where(x => x.CategoryID == id).ToPagedList(page ?? 1, 9), crep.SelectActives()));
        }


        public ActionResult AddToCart(int id)
        {
            if (Session["member"] == null)
            {
                TempData["uyeDegil"] = "Lutfen sepete ürün eklemeden önce üye olun";
                Product bekleyenUrun = prep.GetByID(id);
                Session["bekleyenUrun"] = bekleyenUrun;
                return RedirectToAction("Register", "Home");
            }

            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;

            Product eklenecekUrun = prep.GetByID(id);

            CartItem ci = new CartItem();
            ci.ID = eklenecekUrun.ID;
            ci.Name = eklenecekUrun.ProductName;
            ci.Price = eklenecekUrun.UnitPrice;
            ci.ImagePath = eklenecekUrun.ImagePath;
            c.SepeteEkle(ci);

            Session["scart"] = c;
            return RedirectToAction("Index");


        }

        public ActionResult CartPage()
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;
                return View(c);
            }
            else if (Session["member"] == null)
            {
                TempData["uyeDegil"] = "Lutfen önce üye olun";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Sepetinizde ürün bulunmamaktadır";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult FiltreliUrunler(string item, int? page)
        {
            ViewBag.Fiyat = item;
            //"$75 - $355"
            string[] filter = item.Split('-');
            //"$75"
            //" $355"
            filter[1] = filter[1].TrimStart(); //basındaki boslugu almıs olduk..
            for (int i = 0; i < 2; i++)
            {
                filter[i] = filter[i].TrimStart('$');
            }

            decimal start = Convert.ToDecimal(filter[0]);

            decimal end = Convert.ToDecimal(filter[1]);

            return PartialView("_FiltreliUrunler", Tuple.Create(crep.SelectActives(), prep.Where(x => x.UnitPrice > start && x.UnitPrice < end).ToPagedList(page ?? 1, 9)));

        }

        [HttpGet]
        public ActionResult SearchProduct(string keyword, int? page)
        {
            //todo keyword null ise ilk sayfaya döndürsün
            //if (keyword==null)
            //{
            //    return RedirectToAction("Index");
            //}
            ViewBag.Keyword = keyword;
            return PartialView("_SearchProducts", Tuple.Create(crep.SelectActives(), prep.Where(x => x.ProductName.Contains(keyword)).ToPagedList(page ?? 1, 9)));
        }

        public PartialViewResult FiltreKeywordUrunleri(string fiyat, string keyword, int categoryId, int? page)
        {
            ViewBag.Keyword = keyword;
            ViewBag.Fiyat = fiyat;
            //"$75 - $355"
            string[] filter = fiyat.Split('-');
            //"$75"
            //" $355"
            filter[1] = filter[1].TrimStart(); //basındaki boslugu almıs olduk..
            for (int i = 0; i < 2; i++)
            {
                filter[i] = filter[i].TrimStart('$');
            }

            decimal start = Convert.ToDecimal(filter[0]);

            decimal end = Convert.ToDecimal(filter[1]);

            return PartialView(Tuple.Create(crep.SelectActives(), prep.Where(x => x.UnitPrice > start && x.UnitPrice < end && x.CategoryID == categoryId).ToPagedList(page ?? 1, 9)));
        }

        public ActionResult SiparisiOnayla()
        {
            if (Session["member"] != null)
            {
                AppUser mevcutKullanici = Session["member"] as AppUser;

                return View(Tuple.Create(new Order(), new PaymentVM()));
            }

            TempData["message"] = "Üye olmadan sipariş veremezsiniz";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SiparisiOnayla([Bind(Prefix = "Item1")] Order item, [Bind(Prefix = "Item2")] PaymentVM item2)
        {
            //Burada artık bir client olarak Api'a istek göndermemiz lazım(Api consume)..Bunun icin WebApiClient package'ini Nuget'tan indiriyoruz..

            //Bir Api consume etme sürecinde actıgımız degişkenleri veya nesneler veya sürecleri ram'de cok uzun süre tutmamalıyız.
            bool result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63701/api/");

                var postTask = client.PostAsJsonAsync("Payment/ReceivePayment", item2);
                //Burada item2 nesnesini Json olarak gönderiyoruz...Ve Base Adresimizin üzerine Payment/ReceivePayment'i ekliyoruz..
                var sonuc = postTask.Result;

                if (sonuc.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            if (result)
            {
                AppUser kullanici = Session["member"] as AppUser;
                item.AppUserID = kullanici.ID; //Order'in kim tarafından sipariş edildigini belirliyoruz.
                orep.Add(item); //save edildiginde Order nesnesinin ID'si olusacak..
                int sonSiparisID = item.ID;
                Cart sepet = Session["scart"] as Cart;

                foreach (CartItem urun in sepet.Sepetim)
                {
                    OrderDetail od = new OrderDetail();
                    od.OrderID = sonSiparisID;
                    od.ProductID = urun.ID;
                    od.TotalPrice = urun.SubTotal;
                    od.Quantity = urun.Amount;
                    odrep.Add(od);

                }
                TempData["odeme"] = "Siparişiniz bize ulasmıstır..Teşekkür ederiz";
                return RedirectToAction("Index");
                
            }
            else
            {
                TempData["odeme"] = "Odeme ile ilgili bir sıkıntı olustu.Lutfen banka ile iletişime geciniz";
                return RedirectToAction("Index");
            }













        }


    }
}