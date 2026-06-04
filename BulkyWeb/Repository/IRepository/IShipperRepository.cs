namespace BulkyWeb.Repository.IRepository
{
    public interface IShipperRepository : IRepository<Shipper>
    {


        void Update(Shipper obj);
        void Save();


        void Deleted(int id);
    }
}
