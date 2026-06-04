//using BulkyWeb.Data;
//using BulkyWeb.Models;
//using BulkyWeb.Repository.IRepository;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace BulkyWeb.Repository
//{
//    public class UserRepository : Repository<User>, IUserRepository
//    {
//        private readonly ApplicationDbContext _db;
//        public UserRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }
//        public void Save()
//        {

//        }

//        public void Update(User obj)
//        {
//            _db.Update(obj);

//        }

//        public void Add(User obj)
//        {
//            _db.Add(obj);

//        }


//        //public void Deleted(int id)
//        //{
//        //    var obj =  _db.Users.Find(id);
//        //    if (obj == null)
//        //    {
//        //        return;
//        //    }
//        //    _db.Users.Remove(obj);

//        //}



//        // I write it

//    }
//}