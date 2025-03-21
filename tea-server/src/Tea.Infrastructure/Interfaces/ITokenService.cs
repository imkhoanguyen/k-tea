using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tea.Application.DTOs.Users;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.Interfaces
{
    public interface ITokenService
    {
        public Task<(string, DateTime)> CreateAccessTokenAsync(AppUser user);
        public Task<string> CreateRefreshTokenAsync(AppUser user);
        public Task<UserLoginResponse> ValidRefreshToken(string rfToken);
        public Task ValidToken(TokenValidatedContext context);
    }
}
