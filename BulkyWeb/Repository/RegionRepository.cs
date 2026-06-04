using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class RegionRepository : Repository<Region>, IRegionRepository
    {
        private readonly ApplicationDbContext _db;
        public RegionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Region obj)
        {
            _db.Update(obj);

        }

        public void Add(Region obj)
        {
            _db.Add(obj);

        }


        public void Deleted(int id)
        {
            //var obj = _db.Region.Find(id);
            //if (obj == null)
            //{
            //    return;
            //}
            //_db.Region.Remove(obj);

        }
        public void Delete(int id)
        {
        }
    }
}
