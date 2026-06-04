namespace BulkyWeb.Repository.IRepository
{
    public interface ISupplierRepository : IRepository<Supplier>
    {


        void Update(Supplier obj);
        void Save();


        void Deleted(int id);
    }
}
