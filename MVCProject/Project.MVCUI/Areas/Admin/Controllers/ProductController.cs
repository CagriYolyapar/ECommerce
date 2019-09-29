using Project.BLL.RepositoryPattern.ConcRep;
using Project.COMMON.MyTools;
using Project.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductRepository prep;
        CategoryRepository crep;
        public ProductController()
        {
            prep = new ProductRepository();
            crep = new CategoryRepository();
        }
        // GET: Admin/Product
        public ActionResult ProductList()
        {
            return View(prep.SelectActives());
        }

        public ActionResult AddProduct()
        {
            return View(Tuple.Create(crep.SelectActives(), new Product()));
        }

        [HttpPost]
        public ActionResult AddProduct([Bind(Prefix = "Item2")] Product item, HttpPostedFileBase resim)
        {
            item.ImagePath = ImageUploader.UploadImage("~/Pictures/", resim);
            prep.Add(item);
            //prep.SpecialInsert("Cizi", 12, 12);
            //prep.SpecialInsert(item.ProductName, item.UnitsInStock, item.UnitPrice);
            return RedirectToAction("ProductList");
        }

        public ActionResult ProductDetail(int id)
        {
            return View(prep.Default(x => x.ID == id));
        }

        public ActionResult DeleteProduct(int id)
        {
            prep.Delete(prep.GetByID(id));
            return RedirectToAction("ProductList");
        }

        public ActionResult UpdateProduct(int id)
        {

            return View(Tuple.Create(crep.SelectActives(), prep.GetByID(id)));
        }

        [HttpPost]
        public ActionResult UpdateProduct([Bind(Prefix = "Item2")] Product item, HttpPostedFileBase resim, string resimSil)
        {
            if (resimSil == "true")
            {
                item.ImagePath = "Dosya bos";
            }
            else
            {
                
                if (resim==null)
                {
                    Product tempProduct = prep.GetByID(item.ID);
                    item.ImagePath = tempProduct.ImagePath;

                }
                else
                {
                    item.ImagePath = ImageUploader.UploadImage("~/Pictures/", resim);
                }
            }

            prep.Update(item);
            return RedirectToAction("ProductList");
        }
    }
}