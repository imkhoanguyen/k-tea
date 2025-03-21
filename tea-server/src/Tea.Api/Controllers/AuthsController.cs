using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tea.Application.DTOs.Users;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Exceptions.Unauthorizes;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Configurations;
using Tea.Infrastructure.Interfaces;

namespace Tea.Api.Controllers
{
    public class AuthsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<AuthsController> _logger;
        private readonly IUnitOfWork _unit;
        public AuthsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, IEmailService emailService, IOptions<EmailConfig> emailConfig, ILogger<AuthsController> logger
            , IUnitOfWork unit)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _emailConfig = emailConfig.Value;
            _logger = logger;
            _unit = unit;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName) ??
                await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                _logger.LogWarning("user not found");
                throw new UserNotFoundException($"{request.UserName}");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Wrong password");
                throw new WrongPasswordException();
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenService.CreateAccessTokenAsync(user);
            string refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return Ok(new UserLoginResponse
            {
                UserName = user.UserName,
                FullName = user.FullName,
                ImgUrl = user.ImgUrl,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiredDateAccessToken = expiredDateAccessToken,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await CheckEmailExistAsync(request.Email))
            {
                _logger.LogWarning("Email exists");
                throw new EmailExistsException();
            }

            if (await CheckUserNameExistAsync(request.UserName))
            {
                _logger.LogWarning("Username exists");
                throw new UsernameExistsException();
            }

            var userToAdd = new AppUser
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                ImgUrl = @"https://res.cloudinary.com/dh1zsowbp/image/upload/v1735543269/user_pez7rf.webp"
            };

            await _unit.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(userToAdd, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Create user failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new AddNewUserFailedException("Đăng ký thất bại vui lòng thử lại sau");
                }

                var resultAddRole = await _userManager.AddToRoleAsync(userToAdd, "Customer");
                if (!resultAddRole.Succeeded)
                {
                    _logger.LogError("Add role to user failed");
                    throw new AddRoleToUserFailedException("Đăng ký thất bại vui lòng thử lại sau");
                }

                await _unit.CommitTransactionAsync();

                return Ok();
            }
            catch
            {
                await _unit.RollbackTransactionAsync(); throw;
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserLoginResponse>> RefreshToken(string rfToken)
        {
            if (rfToken.IsNullOrEmpty())
            {
                _logger.LogWarning("Could not get refresh token");
                throw new InvalidRefreshTokenException("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại");
            }

            return Ok(await _tokenService.ValidRefreshToken(rfToken));
        }

        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPassword(CancellationToken cancellationToken, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"user with name/username: {email} not exists");
                throw new UserNotFoundException(email);
            }

            string host = _emailConfig.AppUrl;

            string tokenConfirm = await _userManager.GeneratePasswordResetTokenAsync(user);

            string decodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenConfirm));

            string resetPasswordUrl = $"{host}/reset-password?email={email}&token={decodedToken}";


            string body = $"Để reset password của bạn vui lòng click link sau: <a href=\"{resetPasswordUrl}\">Bấm vào đây để đổi lại mật khẩu mới</a>";


            await _emailService.SendMailAsync(cancellationToken, new SendMailRequest
            {
                To = user.Email,
                Subject = "Reset Your Password ",
                Content = body,
            });

            return Ok(new { message = "Vui lòng kiểm tra email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"user with name/username: {request.Email} not exists");
                throw new UserNotFoundException(request.Email);
            }

            if (string.IsNullOrEmpty(request.Token))
            {
                _logger.LogWarning("empty token");
                throw new InvalidTokenException("Thông tin người dùng không hợp lệ. Vui lòng thử lại sau");
            }

            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var identityResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);

            if (identityResult.Succeeded)
            {
                return Ok(new { message = "Reset password thành công" });
            }
            else
            {
                _logger.LogError("Create user failed: {Errors}", string.Join(", ", identityResult.Errors.Select(e => e.Description)));
                throw new ResetPasswordFailedException("Reset Password thất bại. Vui lòng thử lại sau");
            }
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

