using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Orders;
using Tea.Domain.Entities;
using Tea.Domain.Enums;

namespace Tea.Application.Mappers
{
    public class OrderMapper
    {
        public static OrderResponse EntityToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                OrderType = order.OrderType,
                PaymentStatus = order.PaymentStatus,
                PaymentType = order.PaymentType,
                Created = order.Created,
                SubTotal = order.SubTotal,
                DiscountPrice = order.DiscountPrice,
                ShippingFee = order.ShippingFee,
                DiscountId = order.DiscountId,
                Items = order.Items.Select(OrderMapper.EntityToResponse).ToList(),
                Description = order.Description,
                UserId = order.UserId,
                CustomerAddress = order.CustomerAddress,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CreatedById = order.CreatedById,
            };

        }

        public static OrderItemResponse EntityToResponse(OrderItem orderItem)
        {
            return new OrderItemResponse
            {
                Id = orderItem.Id,
                ItemImg = orderItem.ItemImg,
                ItemName = orderItem.ItemName,
                ItemSize = orderItem.ItemSize,
                OrderId = orderItem.OrderId,
                ItemId = orderItem.ItemId,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity,
            };
        }
    }
}
