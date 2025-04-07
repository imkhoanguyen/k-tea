using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Discounts;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class DiscountService(IUnitOfWork unit, ILogger<DiscountService> logger) : IDiscountService
    {
        public async Task<DiscountResponse> CreateAsync(DiscountCreateRequest request)
        {
            if (await unit.Discount.ExistsAsync(u => u.Name.ToLower() == request.Name.ToLower()))
            {
                logger.LogWarning("Discount name exists");
                throw new DiscountExistsException("Tên của mã giảm giá đã tồn tại");
            }

            if (await unit.Discount.ExistsAsync(u => u.PromotionCode.ToLower() == request.PromotionCode.ToLower()))
            {
                logger.LogWarning("Discount promotion code exists");
                throw new DiscountExistsException("Mã của mã giảm giá đã tồn tại");
            }

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

            logger.LogError("Add discount failed");
            throw new SaveChangesFailedException();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await unit.Discount.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
            {
                logger.LogError($"discount with id {id} not found");
                throw new DiscountNotFoundException(id);
            }

            entity.IsDeleted = true;

            if (!await unit.SaveChangesAsync())
            {
                logger.LogError($"delete discount with id {id} failed");
                throw new SaveChangesFailedException();
            }

            logger.LogInformation($"delete discount with id {id} successfully");
        }

        public async Task DeletesAsync(List<int> discountIdList)
        {
            var entities = await unit.Discount.FindAllAsync(x => discountIdList.Contains(x.Id), tracked: true);
            if (entities == null || !entities.Any())
            {
                logger.LogWarning("No discounts found to delete.");
                throw new EmptyDiscountIdListException("Vui lòng chọn lại danh sách mã giảm giá cần xóa");
            }

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }

            if (!await unit.SaveChangesAsync())
            {
                logger.LogError($"{Logging.SaveChangesFailed}");
                throw new SaveChangesFailedException();
            }

            logger.LogInformation($"Discounts with IDs: {string.Join(", ", discountIdList)} were deleted.");
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllAsync()
        {
            var discounts = await unit.Discount.FindAllAsync(predicate: null, tracked: false);
            return discounts.Select(DiscountMapper.EntityToResponse);    
        }

        public async Task<DiscountResponse> CheckDiscountAsync(string code)
        {
            var entity = await unit.Discount.FindAsync(x => x.PromotionCode == code);
            if (entity == null)
            {
                logger.LogError($"discount with code {code} not found");
                throw new DiscountNotFoundException(code);
            }

            if(entity.IsDeleted)
            {
                logger.LogError("Coupon code has expired");
                throw new DiscountHasExpired();
            }

            return DiscountMapper.EntityToResponse(entity);
        }

        public async Task<DiscountResponse> GetByIdAsync(int id)
        {
            var entity = await unit.Discount.FindAsync(x => x.Id == id);
            if (entity == null)
            {
                logger.LogError($"discount with id {id} not found");
                throw new DiscountNotFoundException(id);
            }

            return DiscountMapper.EntityToResponse(entity);
        }

        public async Task<PaginationResponse<DiscountResponse>> GetPaginationAsync(PaginationRequest request)
        {
            var pagination = await unit.Discount.GetPaginationAsync(request);
            var responseList = pagination.Data.Select(DiscountMapper.EntityToResponse);
            return new PaginationResponse<DiscountResponse>(pagination.PageIndex, pagination.PageIndex, pagination.Count, responseList);
        }

        public async Task<DiscountResponse> UpdateAsync(int id, DiscountUpdateRequest request)
        {
            if(id != request.Id)
            {
                logger.LogWarning("id route and request mismatch");
                throw new IdMismatchException("Vui lòng chọn lại hàng cần cập nhật");
            }

            if (await unit.Discount.ExistsAsync(d => d.Name.ToLower() == request.Name.ToLower() && d.Id != id))
            {
                logger.LogWarning("Discount name exists");
                throw new DiscountExistsException("Tên của mã giảm giá đã tồn tại");
            }

            if (await unit.Discount.ExistsAsync(d => d.PromotionCode.ToLower() == request.PromotionCode.ToLower() && d.Id != id))
            {
                logger.LogWarning("Discount promotion code exists");
                throw new DiscountExistsException("Mã của mã giảm giá đã tồn tại");
            }

            var entity = await unit.Discount.FindAsync(x => x.Id == id, tracked: true);
            if (entity == null)
            {
                logger.LogError($"discount with id {id} not found");
                throw new DiscountNotFoundException(id);
            }

            entity.PercentOff = request.PercentOff;
            entity.AmountOff = request.AmountOff;
            entity.Name = request.Name;
            entity.PromotionCode = request.PromotionCode;

            if (await unit.SaveChangesAsync())
            {
                logger.LogInformation("update discount successfully");
                return DiscountMapper.EntityToResponse(entity);
            }

            logger.LogError("update discount failed");
            throw new SaveChangesFailedException();
        }
    }
}
