using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BulkyWeb.Authorization
{
    public class DynamicPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DynamicPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();

            policy.AddRequirements(new PermissionRequirement());

            return await Task.FromResult(policy.Build());
        }
    }


}
