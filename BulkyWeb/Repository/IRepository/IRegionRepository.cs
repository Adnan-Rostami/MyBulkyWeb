namespace BulkyWeb.Repository.IRepository
{
    public interface IRegionRepository : IRepository<Region>
    {


        void Update(Region obj);
        void Save();


        void Deleted(int id);




    }
}
