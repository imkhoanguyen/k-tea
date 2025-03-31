using System.ComponentModel.DataAnnotations;

namespace Tea.Application.DTOs.Users
{
    public class ChangeRoleRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn quyền để thay đổi")]
        public required string Role { get; set; }
    }
}
