﻿using Tea.Application.DTOs.Orders;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginationResponse<OrderListResponse>> GetPaginationAsync(OrderPaginationRequest request);
        Task<OrderResponse> GetByIdAsync(int id);
        Task<IEnumerable<OrderResponse>> GetAllAsync();
        Task<OrderResponse> CreateInStoreAsync(OrderCreateInStoreRequest request);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task UpdatePaymentStatusAsync(int orderId, string status);
        Task DeleteAsync(int id);
        Task DeletesAsync(List<int> orderIdList);
        Task<OrderResponse> CreateOnlineAsync(OrderCreateOnlineRequest request);
    }
}
