using System.ComponentModel.DataAnnotations;

namespace Tea.Application.DTOs.Discounts
{
    public class DiscountCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên mã giảm giá")]
        public required string Name { get; set; }
        [Range(1000.01, double.MaxValue, ErrorMessage = "Số tiền giảm phải lớn hơn 1.000 VNĐ")]
        public decimal? AmountOff { get; set; }

        [Range(0.01, 100, ErrorMessage = "Phần trăm giảm phải từ 1 đến 100")]
        public decimal? PercentOff { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã của mã giảm giá")]
        public required string PromotionCode { get; set; }
    }
}
