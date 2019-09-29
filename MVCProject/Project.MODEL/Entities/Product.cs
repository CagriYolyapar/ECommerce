using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MODEL.Entities
{
    public class Product:BaseEntity
    {
        public string ProductName { get; set; }

        public int UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public string ImagePath { get; set; }

        public int? CategoryID { get; set; }

        //Relational Properties

        public virtual Category Category { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}
