using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        private readonly ApplicationDbContext _db;
        public TaskRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Task obj)
        {
            _db.Update(obj);

        }

        public void Add(Task obj)
        {
            _db.Add(obj);

        }


        //public void Deleted(int id)
        //{
        //    var obj =  _db.TaskItems.Find(id);
        //    if (obj == null)
        //    {
        //        return;
        //    }
        //    _db.TaskItems.Remove(obj);

        //}



        // I write it

    }
}