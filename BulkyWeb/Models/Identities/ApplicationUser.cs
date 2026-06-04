using BulkyWeb.Models.RefreshTokens;
using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Models.Identities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
