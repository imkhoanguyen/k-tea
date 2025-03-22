using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tea.Application.DTOs.Permissions;
using Tea.Application.DTOs.Roles;
using Tea.Application.Mappers;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Utilities;

namespace Tea.Api.Controllers
{
    public class RolesController : BaseApiController
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RolesController> _logger;
        private readonly IUnitOfWork _unit;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager,
            ILogger<RolesController> logger, IUnitOfWork unit)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _unit = unit;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<RoleResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IActionResult>> GetPagination([FromQuery] PaginationRequest request)
        {
            var query = _roleManager.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(r => r.Name!.ToLower().Contains(request.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                query = request.OrderBy switch
                {
                    "id" => query.OrderBy(r => r.Id),
                    "id_desc" => query.OrderByDescending(r => r.Id),
                    "name" => query.OrderBy(r => r.Name!.ToLower()),
                    "name_desc" => query.OrderByDescending(r => r.Name!.ToLower()),
                    _ => query.OrderByDescending(r => r.Name),
                };
            }

            // Không dùng RoleResponse.FromEntity ở đây vì phương thức này ko thể dịch sang sql
            var roleResponses = query.Select(r => new RoleResponse // mapping
            {
                Id = r.Id,
                Name = r.Name!,
                Description = r.Description!,
            });

            var response = await query.ApplyPaginationAsync(request.PageIndex, request.PageSize);

            return Ok(response);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles.Select(RoleMapper.EntityToRoleResponse));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppRole>> Get(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning($"role with id: {id} not found");
                throw new RoleNotFoundException(id);
            }

            var response = RoleMapper.EntityToRoleResponse(role); // map

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = AppPermission.Role_Create)]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> Create(RoleCreateRequest request)
        {
            if (await RoleNameExists(request.Name))
            {
                _logger.LogWarning("role name exists");
                throw new RoleExistsException("Tên quyền đã tồn tại");
            }

            var appRole = new AppRole
            {
                Name = request.Name,
                Description = request.Description,
            };

            var result = await _roleManager.CreateAsync(appRole);

            if (result.Succeeded)
            {
                var response = RoleMapper.EntityToRoleResponse(appRole);
                return CreatedAtAction(nameof(Get), new { id = appRole.Id }, response);
            }

            _logger.LogError("error when create role");
            throw new AddNewRoleFailedException("Đã xảy ra lỗi khi thêm quyền. Vui lòng thử lại sau");
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AppPermission.Role_Edit)]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(string id, [FromBody] RoleUpdateRequest request)
        {
            if (request.Id != id)
            {
                _logger.LogWarning(Logging.IdMismatch(routeId: id, bodyId: request.Id));
                throw new IdMismatchException("Vui lòng chọn lại quyền cần cập nhật");
            }

            if (await CheckEdit(request.Name, id))
            {
                _logger.LogWarning("role name exists");
                throw new RoleExistsException("Tên quyền đã tồn tại");
            }

            var roleFromDb = await _roleManager.FindByIdAsync(id);

            if (roleFromDb == null)
            {
                _logger.LogWarning("Role not found");
                throw new RoleNotFoundException(id);
            }

            roleFromDb.Name = request.Name;
            roleFromDb.Description = request.Description;

            var result = await _roleManager.UpdateAsync(roleFromDb);

            if (result.Succeeded)
            {
                return Ok(RoleMapper.EntityToRoleResponse(roleFromDb));
            }

            throw new UpdateRoleFailedException("Đã xảy ra lỗi khi cập nhật quyền");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AppPermission.Role_Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning($"role with id: {id} not found");
                throw new RoleNotFoundException(id);
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                _logger.LogWarning("Failed to delete role: The role is assigned to users.");
                throw new DeleteRoleFailedException("Không thể xóa quyền vì nó đang được gán cho người dùng");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return NoContent();

            _logger.LogError("error when delete role");
            throw new DeleteRoleFailedException("Đã xảy ra lỗi khi xóa quyền");
        }

        [HttpGet("permissions")]
        [ProducesResponseType(typeof(IEnumerable<PermissionGroupResponse>), StatusCodes.Status200OK)]
        public IActionResult GetAllPermission()
        {
            return Ok(PermissionGroup.AllPermissionGroups);
        }

        [HttpGet("{roleId}/claims")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning($"role not found with id: {roleId}");
                throw new RoleNotFoundException(roleId);
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            return Ok(claims.Select(c => c.Value).ToList());
        }

        [HttpPut("{roleId}/update-claims")]
        [Authorize(Policy = AppPermission.Role_Edit)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoleClaims(string roleId, [FromBody] List<string> newRoleClaims)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning($"role not found with id: {roleId}");
                throw new RoleNotFoundException(roleId);
            }

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var currentClaimValues = currentClaims.Select(c => c.Value).ToList();


            var claimsToAdd = newRoleClaims.Except(currentClaimValues).ToList();

            var claimsToRemove = currentClaimValues.Except(newRoleClaims).ToList();

            await _unit.BeginTransactionAsync();

            try
            {
                // Xóa các claim không còn trong danh sách mới
                foreach (var claimValue in claimsToRemove)
                {
                    var claim = currentClaims.FirstOrDefault(c => c.Value == claimValue);
                    if (claim != null)
                    {
                        var result = await _roleManager.RemoveClaimAsync(role, claim);
                        if (!result.Succeeded)
                        {
                            _logger.LogWarning($"delete claim of role name: {role.Name} failed");
                            throw new DeleteClaimFailedException($"Đã xảy ra lỗi khi cập nhật chức năng {claim.Value} của quyền {role.Name}");
                        }
                    }
                }

                // Thêm các claim mới
                foreach (var claimValue in claimsToAdd)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim(Auth.Permission, claimValue));
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning($"add claim: {claimValue} to role name: {role.Name} failed");
                        throw new AddNewClaimFailedException($"Đã xảy ra lỗi khi cập nhật chức năng {claimValue} của quyền {role.Name}");
                    }
                }

                await _unit.CommitTransactionAsync();

                return NoContent();
            }
            catch
            {
                await _unit.RollbackTransactionAsync(); throw;
            }
        }

        #region Helper
        private async Task<bool> RoleNameExists(string roleName)
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name!.ToLower() == roleName.ToLower());
            if (role != null) return true;
            return false;
        }

        private async Task<bool> CheckEdit(string roleName, string roleId)
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name!.ToLower() == roleName.ToLower() && r.Id != roleId);
            if (role != null) return true;
            return false;
        }
        #endregion
    }
}
