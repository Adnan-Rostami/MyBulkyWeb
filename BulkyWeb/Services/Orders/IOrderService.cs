using BulkyWeb.Models.DTO.Order;

namespace BulkyWeb.Services.Orders
{


    public interface IOrderService
    {
        Task<List<OrderGetAllDTO>> GetAllAsync(OrderFilterDTO filter);
        Task<OrderCreateResponseDTO> OrderCreate(OrderCreateDTO filter, CancellationToken ct = default);




        Task<OrderUpdateResponseDTO> UpdateByIDAsync(int id, OrderUpdateDTO body);






        Task DeleteAsync(int id);

    }





}
