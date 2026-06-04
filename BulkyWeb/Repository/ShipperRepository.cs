using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class ShipperRepository : Repository<Shipper>, IShipperRepository
    {


        private readonly ApplicationDbContext _db;
        public ShipperRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Shipper obj)
        {
            _db.Update(obj);

        }

        public void Add(Shipper obj)
        {
            _db.Add(obj);

        }


        public void Deleted(int id)
        {
            var obj = _db.Shippers.Find(id);
            if (obj == null)
            {
                return;
            }
            _db.Shippers.Remove(obj);

        }
    }
}
