using Tea.Application.DTOs.Roles;
using Tea.Domain.Entities;

namespace Tea.Application.Mappers
{
    public class RoleMapper
    {
        public static RoleResponse EntityToRoleResponse(AppRole appRole)
        {
            return new RoleResponse
            {
                Id = appRole.Id,
                Name = appRole.Name,
                Description = appRole.Description,
            };
        }
    }
}
