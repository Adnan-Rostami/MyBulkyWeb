using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _db;
        public ProjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {

        }

        public void Update(Project obj)
        {
            _db.Update(obj);

        }

        public void Add(Project obj)
        {
            _db.Add(obj);

        }


        //public void Deleted(int id)
        //{
        //    var obj =  _db.Projects.Find(id);
        //    if (obj == null)
        //    {
        //        return;
        //    }
        //    _db.Projects.Remove(obj);

        //}



        // I write it

    }
}