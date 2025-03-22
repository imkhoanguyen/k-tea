namespace Tea.Application.DTOs.Permissions
{
    public class PermissionGroupResponse
    {
        public required string GroupName { get; set; }
        public List<PermissionItemResponse> Permissions { get; set; } = [];
    }
}
