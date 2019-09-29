using Project.BLL.RepositoryPattern.ConcRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        CategoryRepository crep;
        ProductRepository prep;
        public CategoryController()
        {
            prep = new ProductRepository();
            crep = new CategoryRepository();
        }
        // GET: Admin/Category
        public ActionResult CategoryList()
        {
            return View(crep.SelectActives());
        }
    }
}