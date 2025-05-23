﻿using System.ComponentModel.DataAnnotations;
using Tea.Domain.ValidationAttributes;

namespace Tea.Application.DTOs.Users
{
    public class UserUpdateRequest
    {
        [Required(ErrorMessage = "UserName không được trống")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "UserName phải lớn hơn 4 ký tự và bé hơn 50 ký tự")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Email không được trống")]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Không đúng định dạng email")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "FullName không được trống")]
        [StringLength(100, ErrorMessage = "FullName không được quá 100 ký tự")]
        public required string FullName { get; set; }

        public string? Password { get; set; }

        [VietNamPhoneNumber]
        public required string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Địa chỉ không được trống")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
        public required string Address { get; set; }

    }
}
