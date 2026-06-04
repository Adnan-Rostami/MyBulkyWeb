using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BulkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        public Repository(ApplicationDbContext db)
        {
            _db = db;

        }
        public IQueryable<T> Query(bool tracking = false)
        {
            if (tracking is false)
            {
                var query = _db.Set<T>().AsQueryable().AsNoTracking();
                return query;
                //AsNoTracking();
            }
            //return _db.Set<T>().AsNoTracking();
            else
            {
                var query = _db.Set<T>().AsQueryable();
                return query;
            }

        }
        public void Add(T entity)
        {
            _db.Add(entity);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter)
        {


            return await _db.Set<T>()
                .AsNoTracking()
                .Where(filter)
                .FirstOrDefaultAsync();
        }


        public async Task<List<T>> GetAllAsync()
        {
            return await _db.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }



        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _db.Set<T>().AsQueryable().AsNoTracking();
            //query = query.Where(filter);
            if (filter != null)
            {

                query = query.Where(filter);

            }

            // query = query.Where(filter);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {

            return await (_db.Set<T>().FindAsync(id));
        }



        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _db.Set<T>().AddAsync(entity, ct);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
        }


    }
}
