using Project.BLL.RepositoryPattern.BaseRep;
using Project.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.RepositoryPattern.ConcRep
{
    public class ProductRepository:BaseRepository<Product>
    {
        /// <summary>
        /// Windows Form icin geliştirilen iş mantıgı metodudurr.
        /// </summary>
        /// <param name="productName">Urun İsmi</param>
        /// <param name="unitsInStock"> Urun Stok Sayısı</param>
        /// <param name="unitPrice">Urun Fiyatı</param>
        public void SpecialInsert(string productName,int unitsInStock,decimal unitPrice)
        {
            Product p = new Product();
            p.ProductName = productName;
            p.UnitsInStock = unitsInStock;
            p.UnitPrice = unitPrice;
            db.Products.Add(p);
            Save();
        }
    }
}
