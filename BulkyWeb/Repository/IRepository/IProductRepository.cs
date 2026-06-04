namespace BulkyWeb.Repository.IRepository
{

    public interface IProductRepository : IRepository<Product>
    {
        void Save();


        void Deleted(int id);
        void Update(Product obj);
    }
}

