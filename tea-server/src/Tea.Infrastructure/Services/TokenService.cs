using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tea.Application.DTOs.Users;
using Tea.Domain.Constants;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.Unauthorizes;
using Tea.Infrastructure.Configurations;
using Tea.Infrastructure.Interfaces;

namespace Tea.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfig _tokenConfig;
        private readonly SymmetricSecurityKey _jwtKey;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<TokenConfig> tokenConfig, UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager, ILogger<TokenService> logger)
        {
            _tokenConfig = tokenConfig.Value;
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Key));
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<(string, DateTime)> CreateAccessTokenAsync(AppUser user)
        {
            DateTime expiredToken = DateTime.Now.AddMinutes(_tokenConfig.AccessTokenExpiredByMinutes);

            var role = await _userManager.GetRolesAsync(user);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role[0])
            };

            // Thêm các claims dựa trên role
            var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role[0]));
            userClaims.AddRange(roleClaims);

            var creadentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = expiredToken,
                SigningCredentials = creadentials,
                Issuer = _tokenConfig.Issuer,
                Audience = _tokenConfig.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(jwt);

            await _userManager.SetAuthenticationTokenAsync(user, Auth.AccessTokenProvider, Auth.AccessToken, accessToken);

            return (accessToken, expiredToken);
        }

        public async Task ValidToken(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();
            if (claims.Count == 0)
            {
                //context.Fail("This token contains no information");
                //return;
                throw new InvalidTokenException("Có lỗi xảy ra trong quá trình xác thực thông tin. Vui lòng đăng nhập lại.");
            }

            var identity = context.Principal.Identity as ClaimsIdentity;


            string userId = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    //context.Fail("This token invalid for user");
                    //return;
                    throw new InvalidTokenException("Người dùng chưa được đăng ký");
                }
            }
        }

        public async Task<string> CreateRefreshTokenAsync(AppUser user)
        {
            DateTime expiredRefreshToken = DateTime.Now.AddHours(_tokenConfig.RefreshTokenExpiredByHours);
            var role = await _userManager.GetRolesAsync(user);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role[0])
            };

            var creadentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = expiredRefreshToken,
                SigningCredentials = creadentials,
                Issuer = _tokenConfig.Issuer,
                Audience = _tokenConfig.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = tokenHandler.WriteToken(jwt);

            await _userManager.SetAuthenticationTokenAsync(user, Auth.RefreshTokenProvider, Auth.RefreshToken, refreshToken);

            return refreshToken;
        }

        public async Task<UserLoginResponse> ValidRefreshToken(string rfToken)
        {
            var claimPrinciple = new JwtSecurityTokenHandler().ValidateToken(
                rfToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Key)),
                        ValidIssuer = _tokenConfig.Issuer,
                        ValidateIssuer = true,
                        ValidAudience = _tokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = false, // ko valid thời gian của refresh (vì ko làm chức năng buộc user logout)
                        ClockSkew = TimeSpan.Zero,
                    },
                     out _
                );

            if (claimPrinciple == null)
            {
                _logger.LogError("RefreshToken Invalid when valid refresh token");
                throw new InvalidRefreshTokenException("Phiên đăng nhập đã hết hạn vui lòng đăng nhập lại.");
            }

            string userId = claimPrinciple?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            var token = await _userManager.GetAuthenticationTokenAsync(user, Auth.AccessTokenProvider, Auth.AccessToken);

            if (!string.IsNullOrEmpty(token))
            {
                (string accessToken, DateTime expiredDateAccessToken) = await CreateAccessTokenAsync(user);
                string refreshToken = await CreateRefreshTokenAsync(user);

                return new UserLoginResponse
                {
                    UserName = user.UserName,
                    FullName = user.FullName,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiredDateAccessToken = expiredDateAccessToken,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber
                };
            }

            _logger.LogError("Problem when check valid refresh token");
            throw new InvalidRefreshTokenException("Phiên đăng nhập đã hết hạn vui lòng đăng nhập lại.");
        }
    }
}
