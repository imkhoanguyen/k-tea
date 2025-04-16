using Tea.Application.DTOs.Orders;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginationResponse<OrderResponse>> GetPaginationAsync(PaginationRequest request);
        Task<OrderResponse> GetByIdAsync(int id);
        Task<IEnumerable<OrderResponse>> GetAllAsync();
        Task<OrderResponse> CreateInStoreAsync(OrderCreateInStoreRequest request);
        Task<OrderResponse> UpdateAsync(int id, OrderUpdateRequest request);
        Task DeleteAsync(int id);
        Task DeletesAsync(List<int> orderIdList);
        Task<OrderResponse> CreateOnlineAsync(OrderCreateOnlineRequest request);
    }
}
