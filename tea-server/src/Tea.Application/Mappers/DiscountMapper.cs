using Tea.Application.DTOs.Discounts;
using Tea.Domain.Entities;

namespace Tea.Application.Mappers
{
    public class DiscountMapper
    {
        public static DiscountResponse EntityToResponse(Discount discount)
        {
            return new DiscountResponse
            {
                Id = discount.Id,
                Name = discount.Name,
                AmountOff = discount.AmountOff,
                PercentOff = discount.PercentOff,
                PromotionCode = discount.PromotionCode,
            };
        }
    }
}
