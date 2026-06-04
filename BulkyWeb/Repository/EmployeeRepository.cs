//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;

//namespace BulkyWeb.Repository
//{
//    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
//    {


//        private readonly ApplicationDbContext _db;
//        public EmployeeRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }
//        public void Save()
//        {

//        }

//        public void Update(Employee obj)
//        {
//            _db.Update(obj);

//        }

//        public void Add(Employee obj)
//        {
//            _db.Add(obj);

//        }


//        public void Deleted(int id)
//        {
//            var obj = _db.Employees.Find(id);
//            if (obj == null)
//            {
//                return;
//            }
//            _db.Employees.Remove(obj);

//        }

//    }
//}
