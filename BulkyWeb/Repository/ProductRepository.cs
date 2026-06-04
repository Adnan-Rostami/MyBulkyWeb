using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Product obj)
        {
            _db.Update(obj);

        }

        public void Add(Product obj)
        {
            _db.Add(obj);

        }


        public void Deleted(int id)
        {
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return;
            }
            _db.Products.Remove(obj);

        }

        //old
        //public void Update(Product obj)
        //{
        //    var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
        //    if (objFromDb != null)
        //    {
        //        objFromDb.Title = obj.Title;
        //        objFromDb.ISBN = obj.ISBN;
        //        objFromDb.Price = obj.Price;
        //        objFromDb.Price50 = obj.Price50;
        //        objFromDb.ListPrice = obj.ListPrice;
        //        objFromDb.Price100 = obj.Price100;
        //        objFromDb.Description = obj.Description;
        //        //objFromDb.CategoryId = obj.CategoryId;
        //        objFromDb.Author = obj.Author;
        //        //objFromDb.CoverTypeId = obj.CoverTypeId;
        //        //if (obj.ImageUrl != null)
        //        //{
        //        //    objFromDb.ImageUrl = obj.ImageUrl;
        //        //}
        //    }
        //}
    }
}