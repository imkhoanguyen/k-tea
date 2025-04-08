using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tea.Domain.ValidationAttributes
{
    public class VietNamPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("SĐT không được trống");
            }

            string phoneNumber = value.ToString();

            // Regex pattern cho số điện thoại Việt Nam
            // Hỗ trợ các đầu số phổ biến: 03, 05, 07, 08, 09, 01, và mã vùng 84
            string pattern = @"^(0[0-9]{9,10}|84[0-9]{9,10})$";

            if (!Regex.IsMatch(phoneNumber, pattern))
            {
                return new ValidationResult("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại Việt Nam (10-11 số, bắt đầu bằng 0 hoặc +84/84)");
            }

            return ValidationResult.Success;
        }
    }
}
