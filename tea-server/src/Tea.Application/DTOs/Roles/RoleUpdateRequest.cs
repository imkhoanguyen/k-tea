namespace Tea.Application.DTOs.Roles
{
    public class RoleUpdateRequest
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
