//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;

//namespace BulkyWeb.Repository
//{
//    public class CustomerRepository : Repository<Customer>, ICustomerRepository
//    {


//        private readonly ApplicationDbContext _db;
//        public CustomerRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }
//        public void Save()
//        {

//        }

//        public void Update(Customer obj)
//        {
//            _db.Update(obj);

//        }

//        public void Add(Customer obj)
//        {
//            _db.Add(obj);

//        }


//        public void Deleted(int id)
//        {
//            //var obj = _db.Customers.Find(id);
//            //if (obj == null)
//            //{
//            //    return;
//            //}
//            //_db.Customers.Remove(obj);

//        }

//        //void Add(Category obj);


//    }
//}
