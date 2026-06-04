using System.Linq.Expressions;

namespace BulkyWeb.Repository.IRepository
{

    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter);
        IQueryable<T> Query(bool tracking = false);




        Task AddAsync(T entity, CancellationToken ct = default);
        //void Update(T entity);
        void Remove(T entity);




        void RemoveRange(IEnumerable<T> entity);
    }

}
