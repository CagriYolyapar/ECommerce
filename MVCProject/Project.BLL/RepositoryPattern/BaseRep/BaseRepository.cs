using Project.BLL.RepositoryPattern.InterfaceRep;
using Project.BLL.SingletonPattern;
using Project.DAL.Context;
using Project.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.RepositoryPattern.BaseRep
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        //BaseRepository<Category> crep = new BaseRepository<Category>();


        //Bu BaseRepository sınıfı ViewModel işlemleri icin de farklı kodlar yazmasına bu işlemlerin ragmen genel mantıgı aynı olacagı icin icerisinde barındırdıgı Context field'ini miras verdigi Repository sınıflarına aktarırsa cok daha saglıklı olur. Ancak public yapmamız güvenligi bozacagı icin bunu sadece bu hiyerarşi icinde kalacak şekilde düzenliyoruz. Dolayısıyla protected erişim belirtecini kullandık.

        protected MyContext db;
        public BaseRepository()
        {
            db = DBTool.DBInstance;
        }
        public void Add(T item)
        {
            db.Set<T>().Add(item);
            Save();
        }

        protected void Save()
        {
            db.SaveChanges();
        }

        public bool Any(Expression<Func<T, bool>> exp)
        {
            return db.Set<T>().Any(exp);
        }

        public T Default(Expression<Func<T, bool>> exp)
        {
            return db.Set<T>().FirstOrDefault(exp);
        }

        public void Delete(T item)
        {
            item.Status = MODEL.Enums.DataStatus.Deleted;
            item.DeletedDate = DateTime.Now;
            Save();
        }

        public T GetByID(int id)
        {
            return db.Set<T>().Find(id);
        }

        public object ListAnonymus(Expression<Func<T, object>> exp)
        {
           


            return db.Set<T>().Select(exp).ToList();
        }

        public List<T> SelectActives()
        {
            return db.Set<T>().Where(x => x.Status != MODEL.Enums.DataStatus.Deleted).ToList();
        }

        public List<T> SelectAll()
        {
            return db.Set<T>().ToList();
        }

        public List<T> SelectDeleteds()
        {
            return db.Set<T>().Where(x => x.Status == MODEL.Enums.DataStatus.Deleted).ToList();
        }

        public List<T> SelectModifieds()
        {
            return db.Set<T>().Where(x => x.Status == MODEL.Enums.DataStatus.Updated).ToList();
        }

        public void SpecialDelete(int id)
        {
            db.Set<T>().Remove(GetByID(id));
            Save();
        }

        public void Update(T item)
        {
            item.Status = MODEL.Enums.DataStatus.Updated;

            item.ModifiedDate = DateTime.Now;
            T toBeUpdated = GetByID(item.ID);

            db.Entry(toBeUpdated).CurrentValues.SetValues(item);
            Save();
          
        }

        //Sorgula
        public List<T> Where(Expression<Func<T, bool>> exp)
        {
            //public void Add(bool item){}
           //Topla(12)
           //Where(x=>x.CategoryName=="Beverages")
           //select * from Categories where CategoryName="Beverages"
            return db.Set<T>().Where(exp).ToList();

            //Sorgula(x=>x.CategoryName=="Beverages")

            //Select(x=> new{}) => Anonim Sorguların yazılabilmesi icin Lambda parametresinin ikinci generic tipinin object olabilmesi lazım
        }
    }
}
