namespace BulkyWeb.Repository.IRepository
{
    public interface ITerritoryRepository : IRepository<Territory>
    {


        void Update(Territory obj);
        void Save();


        void Deleted(int id);


    }
}
