//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;
//using Microsoft.AspNetCore.Identity;

//namespace BulkyWeb.Repository
//{
//    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
//    {
//        private readonly ApplicationDbContext _db;
//        public RoleRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }

//        public void Save()
//        {

//        }

//        //public void Update(RoleCreateDTO obj)
//        //{
//        //    _db.Update(obj);

//        //}



//        //public void Add(RoleCreateDTO obj)
//        //{
//        //    _db.Add(obj);

//        //}


//        //public void Deleted(int id)
//        //{
//        //    var obj = _db.Roles.Find(id);
//        //    if (obj == null)
//        //    {
//        //        return;
//        //    }
//        //    _db.Roles.Remove(obj);

//        //}
//    }
//}
