//using BulkyWeb.Data;
//using BulkyWeb.Models;
//using BulkyWeb.Repository.IRepository;

//namespace BulkyWeb.Repository
//{
//    public class CustomerdemographicRepository : Repository<CustomerDemographic>, ICustomerDemographicRepository
//    {
//        private readonly ApplicationDbContext _db;
//        public CustomerdemographicRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }
//        public void Save()
//        {

//        }

//        public void Update(CustomerDemographic obj)
//        {
//            _db.Update(obj);

//        }

//        public void Add(CustomerDemographic obj)
//        {
//            _db.Add(obj);

//        }


//        public void Deleted(int id)
//        {
//            var obj = _db.CustomerDemographics.Find(id);
//            if (obj == null)
//            {
//                return;
//            }
//            _db.CustomerDemographics.Remove(obj);

//        }

//    }


//}
