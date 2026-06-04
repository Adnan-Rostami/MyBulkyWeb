using BulkyWeb.Models.DTO.User;
namespace BulkyWeb.Services.Users


{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO model);
        Task<AuthResult> LoginAsync(LoginDTO model);
        Task<UserInfoDTO> GetUserInfoAsync(string userId);
        Task<AuthResult> RefreshTokenAsync(string token);

    }
}
