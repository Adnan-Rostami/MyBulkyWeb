using BulkyWeb.Models.Orders;

namespace BulkyWeb.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {


        void Update(Order obj);
        void Save();


        void Remove(int id);


        //void Add(Category obj);




    }
}
