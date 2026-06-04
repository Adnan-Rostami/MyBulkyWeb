using BulkyWeb.Data;
using BulkyWeb.Models.Orders;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {

        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Order obj)
        {
            _db.Update(obj);

        }

        public void Add(Order obj)
        {
            _db.Add(obj);

        }


        public void Remove(int id)
        {
            var obj = _db.Orders.Find(id);
            if (obj == null)
            {
                return;
            }
            _db.Orders.Remove(obj);

        }

    }
}
