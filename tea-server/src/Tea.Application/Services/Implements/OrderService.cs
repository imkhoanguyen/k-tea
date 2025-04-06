using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tea.Application.DTOs.Orders;
using Tea.Application.Interfaces;
using Tea.Application.Mappers;
using Tea.Application.Services.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Enums;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;

namespace Tea.Application.Services.Implements
{
    public class OrderService(IUnitOfWork unit, ILogger<OrderService> logger, UserManager<AppUser> userManager, ICartService cartService) : IOrderService
    {
        public async Task<OrderResponse> CreateInStoreAsync(OrderCreateInStoreRequest request)
        {
            string userId = await GetUserIdByUserNameAsync(request.CreatedById);
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning($"cant found user with username: {request.CreatedById}");
                throw new UserNotFoundException($"{request.CreatedById}");
            }

            // Convert string to enum
            if (!Enum.TryParse<PaymentType>(request.PaymentType, ignoreCase: true, out var paymentType))
            {
                logger.LogError($"Invalid payment type: {request.PaymentType}");
                throw new ConvertStringToEnumFailedException("Lỗi khi chọn phương thức thanh toán.Vui lòng thử lại sau.");
            }

            var listOrderItem = request.Items.Select(x => new OrderItem
            {
                ItemName = x.ItemName,
                Price = x.Price,
                ItemSize = x.ItemSize,
                ItemImg = x.ItemImg,
                Quantity = x.Quantity,
                ItemId = x.ItemId,
            }).ToList();

            var order = new Order
            {
                CreatedById = userId,
                DiscountPrice = request.DiscountPrice,
                DiscountId = request.DiscountId,
                OrderStatus = request.OrderStatus,
                OrderType = request.OrderType,
                PaymentStatus = request.PaymentStatus,
                PaymentType = paymentType,
                Items = listOrderItem
            };

            await unit.BeginTransactionAsync();

            try
            {
                unit.Order.Add(order);

                if (await unit.SaveChangesAsync())
                {
                    await unit.CommitTransactionAsync();
                    logger.LogInformation("Create order success");
                    return OrderMapper.EntityToResponse(order);
                }

                logger.LogError("Create order failed. (save change failed");
                throw new SaveChangesFailedException();
            } catch
            {
                await unit.RollbackTransactionAsync();
                throw;
            }
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeletesAsync(List<int> orderIdList)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> GetByIdAsync(int id)
        {
            var order = await unit.Order.FindAsync(x => x.Id == id);
            if (order == null)
            {
                throw new OrderNotFoundException(id);
            }

            return OrderMapper.EntityToResponse(order);
        }

        public Task<PaginationResponse<OrderResponse>> GetPaginationAsync(PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> UpdateAsync(int id, OrderUpdateRequest request)
        {
            throw new NotImplementedException();
        }


        #region Helper
        private async Task<string> GetUserIdByUserNameAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if(user == null) return string.Empty;

            return user.Id;
        }
        #endregion
    }
}
