using System.ComponentModel.DataAnnotations;

namespace Tea.Application.DTOs.Users
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Password không được trống")]
        [StringLength(23, MinimumLength = 5, ErrorMessage = "Password phải lớn hơn 4 ký tự và bé hơn 24 ký tự")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Password hiện tại không được trống")]
        [StringLength(23, MinimumLength = 5, ErrorMessage = "Password hiện tại phải lớn hơn 4 ký tự và bé hơn 24 ký tự")]
        public required string CurrentPassword { get; set; }
    }
}
