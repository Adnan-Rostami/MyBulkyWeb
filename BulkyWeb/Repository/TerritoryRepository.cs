using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class TerritoryRepository : Repository<Territory>, ITerritoryRepository
    {
        private readonly ApplicationDbContext _db;
        public TerritoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Territory obj)
        {
            _db.Update(obj);

        }

        public void Add(Territory obj)
        {
            _db.Add(obj);

        }

        public void Deleted(int id)
        {
            throw new NotImplementedException();
        }


        //public void Deleted(int id)
        //{
        //    var obj = _db.Territories.Find(id);
        //    if (obj == null)
        //    {
        //        return;
        //    }
        //    _db.Territories.Remove(obj);

        //}


    }
}
