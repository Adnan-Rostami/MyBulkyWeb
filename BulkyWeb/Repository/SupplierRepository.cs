using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {


        private readonly ApplicationDbContext _db;
        public SupplierRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Supplier obj)
        {
            _db.Update(obj);

        }

        public void Add(Supplier obj)
        {
            _db.Add(obj);

        }


        public void Deleted(int id)
        {
            var obj = _db.Suppliers.Find(id);
            if (obj == null)
            {
                return;
            }
            _db.Suppliers.Remove(obj);

        }


    }
}
