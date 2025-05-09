﻿using Tea.Application.DTOs.Discounts;
using Tea.Domain.Common;

namespace Tea.Application.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<PaginationResponse<DiscountResponse>> GetPaginationAsync(PaginationRequest request);
        Task<DiscountResponse> CheckDiscountAsync(string code);
        Task<DiscountResponse> GetByIdAsync(int id);
        Task<IEnumerable<DiscountResponse>> GetAllAsync();
        Task<DiscountResponse> CreateAsync(DiscountCreateRequest request);
        Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateRequest request);
        Task DeleteAsync(int id);
        Task DeletesAsync(List<int> discountIdList);
    }
}
