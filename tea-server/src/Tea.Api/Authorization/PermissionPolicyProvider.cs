using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Tea.Domain.Constants;

namespace Tea.Api.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder()
           .RequireClaim(Auth.Permission, policyName)
           .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
    }
}
