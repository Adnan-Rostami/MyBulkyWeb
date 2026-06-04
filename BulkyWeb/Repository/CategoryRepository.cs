using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Category obj)
        {
            _db.Update(obj);

        }



        public void Add(Category obj)
        {
            _db.Add(obj);

        }


        public void Deleted(int id)
        {
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return;
            }
            _db.Categories.Remove(obj);

        }


    }
}