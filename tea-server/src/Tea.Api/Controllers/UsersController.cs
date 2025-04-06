using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tea.Application.DTOs.Users;
using Tea.Application.Interfaces;
using Tea.Domain.Common;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Utilities;

namespace Tea.Api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unit;
        public UsersController(UserManager<AppUser> userManager, ICloudinaryService cloudinaryService,
            ILogger<UsersController> logger, IUnitOfWork unit)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            _unit = unit;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var query = _userManager.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(u => u.PhoneNumber.Contains(request.Search)
                || u.FullName.ToLower().Contains(request.Search)
                || u.UserName.ToLower().Contains(request.Search));
            }

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                query = request.OrderBy switch
                {
                    "userName" => query.OrderBy(u => u.UserName),
                    "userName_desc" => query.OrderByDescending(u => u.UserName),
                    "email" => query.OrderBy(u => u.Email),
                    "email_desc" => query.OrderByDescending(u => u.Email),
                    "fullName" => query.OrderBy(u => u.FullName),
                    "fullName_desc" => query.OrderByDescending(u => u.FullName),
                    "created" => query.OrderBy(u => u.Created),
                    "created_desc" => query.OrderByDescending(u => u.Created),
                    _ => query.OrderBy(u => u.Created)
                };
            }

            var users = await query.ApplyPaginationAsync(request.PageIndex, request.PageSize);

            var usersResponse = new List<UserResponse>();

            foreach (var user in users.Data)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var res = new UserResponse
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Created = user.Created,
                    Role = roles[0],
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsLooked = user.LockoutEnd.HasValue && (user.LockoutEnd > DateTimeOffset.UtcNow || user.LockoutEnd == DateTimeOffset.MaxValue)
                };
                usersResponse.Add(res);
            }

            var response = new PaginationResponse<UserResponse>(users.PageIndex, users.PageSize, users.Count, usersResponse);

            return Ok(response);
        }

        [HttpGet("{userName}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> Get(string userName)
        {
            var query = _userManager.Users.AsNoTracking().AsQueryable();

            var user = await query.FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
            {
                _logger.LogWarning($"user with username: {userName} doesn't exists");
                throw new UserNotFoundException(userName);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var res = new UserResponse
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Created = user.Created,
                Role = roles[0],
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsLooked = user.LockoutEnd.HasValue && (user.LockoutEnd > DateTimeOffset.UtcNow || user.LockoutEnd == DateTimeOffset.MaxValue)
            };

            return Ok(res);
        }

        [HttpPost]
        //[Authorize(Policy = AppPermission.User_Create)]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModalState not valid");
                return BadRequest(ModelState);
            }

            if (await CheckEmailExistAsync(request.Email))
            {
                _logger.LogWarning("Email exists");
                throw new EmailExistsException();
            }

            if (await CheckUserNameExistAsync(request.UserName))
            {
                _logger.LogWarning("UserName exists");
                throw new UsernameExistsException();
            }

            var userToAdd = new AppUser
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
            };

            await _unit.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(userToAdd, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Create user failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new AddNewUserFailedException("Tạo người dùng thất bại");
                }

                var resultAddRole = await _userManager.AddToRoleAsync(userToAdd, request.Role);
                if (!resultAddRole.Succeeded)
                {
                    _logger.LogError("Add role to user failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new AddNewUserFailedException("Có lỗi xảy ra khi thêm quyền cho người dùng.");
                }

                await _unit.CommitTransactionAsync();

                var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                var res = new UserResponse
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Created = user.Created,
                    Role = roles[0],
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsLooked = user.LockoutEnd.HasValue && (user.LockoutEnd > DateTimeOffset.UtcNow || user.LockoutEnd == DateTimeOffset.MaxValue)
                };

                return CreatedAtAction(nameof(Get), new { userName = user.UserName }, res);
            }
            catch
            {
                await _unit.RollbackTransactionAsync();
                throw;
            }
        }

        [HttpPut("{userName}")]
        public async Task<IActionResult> Update(string userName, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModalState not valid");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"{userName} is not exists.");
                throw new UserNotFoundException($"{userName}");
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase) &&
                await CheckEmailExistAsync(request.Email))
            {
                _logger.LogWarning("email exists");
                throw new EmailExistsException();
            }

            if (!string.Equals(user.UserName, request.UserName, StringComparison.OrdinalIgnoreCase) &&
                await CheckUserNameExistAsync(request.UserName))
            {
                _logger.LogWarning("username exists");
                throw new UsernameExistsException();
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.UserName = request.UserName;

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                _logger.LogError($"{string.Join(", ", updateUserResult.Errors.Select(er => er.Description))}");
                throw new UpdateUserFailedException("Cập nhật thông tin người dùng thất bại");
            }

            var updatedRoles = await _userManager.GetRolesAsync(user);
            var response = new UserResponse
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Created = user.Created,
                Role = updatedRoles.FirstOrDefault(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsLooked = user.LockoutEnd.HasValue && (user.LockoutEnd > DateTimeOffset.UtcNow || user.LockoutEnd == DateTimeOffset.MaxValue)
            };

            return Ok(response);
        }

        [HttpPut("{userName}/change-password")]
        public async Task<ActionResult> ChangePassword(string userName, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModalState not valid");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"username: {userName} not found");
                throw new UserNotFoundException(userName);
            }

            // check mật khẩu hiện tại có giống mật khẩu trong db ko 
            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!passwordCheck)
            {
                _logger.LogWarning("your input current password not corret");
                throw new PasswordNotCorretException("Mật khẩu hiện tại không đúng.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
            if (!changePasswordResult.Succeeded)
            {
                _logger.LogWarning($"{changePasswordResult.Errors.Select(er => er.Description)}");
                throw new UpdatePasswordFailedException("Xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại sau");
            }

            return NoContent();
        }

        [HttpPut("{userName}/change-role")]
        //[Authorize(Policy = AppPermission.Role_ChangePermission)]
        public async Task<ActionResult> ChangeRole(string userName, ChangeRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModalState not valid");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"username: {userName} not found");
                throw new UserNotFoundException(userName);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _unit.BeginTransactionAsync();

            try
            {
                // delete current role
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError($"{removeResult.Errors.Select(er => er.Description)}");
                    throw new DeleteRoleFailedException("Không thể loại bỏ quyền cũ của người dùng.");
                }

                // add new role
                var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
                if (!addRoleResult.Succeeded)
                {
                    _logger.LogError($"{addRoleResult.Errors.Select(er => er.Description)}");
                    throw new AddRoleToUserFailedException("Không thể thêm quyền mới cho người dùng.");
                }

                await _unit.CommitTransactionAsync();

            }
            catch
            {
                await _unit.RollbackTransactionAsync();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{userName}")]
        [Authorize(Policy = AppPermission.Role_Delete)]
        public async Task<ActionResult> Delete(string userName)
        {

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"username: {userName} not found");
                throw new UserNotFoundException(userName);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to delete {userName}");
                throw new DeleteUserFailedException("Xoá người dùng thất bại.");
            }

            return NoContent();
        }



        #region
        private async Task<bool> CheckEmailExistAsync(string text)
        {
            return await _userManager.Users.AnyAsync(u => u.Email.ToLower() == text.ToLower());
        }

        private async Task<bool> CheckUserNameExistAsync(string text)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == text.ToLower());
        }

        #endregion
    }
}
