using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Discounts;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class DiscountService(IUnitOfWork unit, ILogger<DiscountService> logger) : IDiscountService
    {
        public async Task<DiscountResponse> CreateAsync(DiscountCreateRequest request)
        {
            var entity = new Discount
            {
                Name = request.Name,
                PromotionCode = request.PromotionCode,
                AmountOff = request.AmountOff,
                PercentOff = request.PercentOff,
            };

            unit.Discount.Add(entity);

            if(await unit.SaveChangesAsync())
            {
                logger.LogInformation("Add discount success");
                return DiscountMapper.EntityToResponse(entity);
            }

            throw new SaveChangesFailedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeletesAsync(List<int> categoryIdList)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiscountResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<DiscountResponse> GetByCodeAsync(string code)
        {
            var entity = await unit.Discount.FindAsync(x => x.PromotionCode == code);
            if (entity == null)
            {
                throw new DiscountNotFoundException(code);
            }

            return DiscountMapper.EntityToResponse(entity);
        }

        public Task<PaginationResponse<DiscountResponse>> GetPaginationAsync(PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
