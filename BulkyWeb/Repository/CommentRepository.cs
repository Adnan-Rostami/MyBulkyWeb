using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        public CommentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Comment obj)
        {
            _db.Update(obj);

        }

        public void Add(Comment obj)
        {
            _db.Add(obj);

        }


        //public void Deleted(int id)
        //{
        //    var obj =  _db.Comments.Find(id);
        //    if (obj == null)
        //    {
        //        return;
        //    }
        //    _db.Comments.Remove(obj);

        //}



        // I write it

    }
}