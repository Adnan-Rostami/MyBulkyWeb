using BulkyWeb.Data;
using BulkyWeb.Exceptions;
using BulkyWeb.Models.DTO.User;
using BulkyWeb.Models.Identities;
using BulkyWeb.Models.RefreshTokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BulkyWeb.Services.Users
{
    public class AuthService : IAuthService
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext db, ILogger<AuthService> logger)
        {
            _db = db;
            _userManager = userManager;
            _config = config;
            _logger = logger;
        }

        public async Task<string> RegisterAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                //return throw new ValidationException()
                return string.Join(",", result.Errors.Select(e => e.Description));
            }

            return "User created successfully";
        }



        //public async Task<string?> LoginAsync(LoginDTO model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);

        //    if (user == null)
        //        return null;

        //    var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

        //    if (!validPassword)
        //        return null;

        //    var token = GenerateJwtToken(user);

        //    return token;
        //}
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            //        var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, user.Id),
            //    new Claim(ClaimTypes.Email, user.Email),
            //    new Claim(ClaimTypes.Name, user.UserName)
            //};
            var roles = await _userManager.GetRolesAsync(user);


            var claims = new List<Claim>
{
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                new Claim(ClaimTypes.Name, user.UserName!)

};
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var permissionClaims =
                await JwtClaimsExtension.GetPermissionClaims(
                    _db,
                    roles.ToList());

            claims.AddRange(permissionClaims);





            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                //expires: DateTime.Now.AddHours(12),
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }









        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                //Expires = DateTime.UtcNow.AddDays(7),
                Expires = DateTime.UtcNow.AddSeconds(30),
                Created = DateTime.UtcNow
                //   CreatedByIp = ipAddress
            };
        }


        public async Task<AuthResult> LoginAsync(LoginDTO model)
        {
            _logger.LogWarning("LOGIN CALLED " + DateTime.Now);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
                throw new NotFoundException("Invalid password");

            var token = await GenerateJwtToken(user);





            //try
            //{
            var oldTokens = _db.RefreshTokens.Where(rt => rt.UserId == user.Id);
            _db.RefreshTokens.RemoveRange(oldTokens);

            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            refreshToken.CreatedByIp = "0.0.0.0";
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    // این قسمت متن اصلی خطا رو بهت میده
            //    var actualError = ex.InnerException?.Message ?? ex.Message;
            //    return new AuthResult { Success = false, Message = $"خطای دیتابیس: {actualError}" };
            //}



            //user.RefreshTokens.Add(refreshToken);

            //await _userManager.UpdateAsync(user);

            return new AuthResult
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken.Token,
                Message = "Login successful"
            };
        }


        public async Task<UserInfoDTO> GetUserInfoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }



        public async Task<AuthResult> RefreshTokenAsync(string token)
        {
            _logger.LogWarning("REFRESH CALLED" + DateTime.Now);
            var refreshToken = await _db.RefreshTokens
                .SingleOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null)
                throw new NotFoundException("Invalid refresh token");

            if (refreshToken.Expires < DateTime.UtcNow)
                throw new NotFoundException("Refresh token expired");

            if (refreshToken.Revoked != null)
                throw new NotFoundException("Refresh token revoked");

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            refreshToken.Revoked = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;
            newRefreshToken.CreatedByIp = "0.0.0.0";

            _db.RefreshTokens.Add(newRefreshToken);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new NotFoundException("Token already used");
            }

            var newJwt = await GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Token = newJwt,
                RefreshToken = newRefreshToken.Token,
                Message = "Token refreshed"
            };
        }



        //public async Task<AuthResult> RefreshTokenAsync(string token)
        //{
        //    var refreshToken = await _db.RefreshTokens
        //        .SingleOrDefaultAsync(x => x.Token == token);

        //    if (refreshToken == null)
        //        throw new NotFoundException("Invalid refresh token");

        //    if (refreshToken.Expires < DateTime.UtcNow)
        //        throw new NotFoundException("Refresh token expired");

        //    if (refreshToken.Revoked != null)
        //        throw new NotFoundException("Refresh token revoked");

        //    var user = await _userManager.FindByIdAsync(refreshToken.UserId);

        //    if (user == null)
        //        throw new NotFoundException("User not found");

        //    // ✅ Rotation
        //    refreshToken.Revoked = DateTime.UtcNow;

        //    var newRefreshToken = GenerateRefreshToken();
        //    newRefreshToken.UserId = user.Id;
        //    newRefreshToken.CreatedByIp = "0.0.0.0";

        //    _db.RefreshTokens.Add(newRefreshToken);


        //    await _db.SaveChangesAsync();
        //    var newJwt = await GenerateJwtToken(user);

        //    return new AuthResult
        //    {
        //        Success = true,
        //        Token = newJwt,
        //        RefreshToken = newRefreshToken.Token,
        //        Message = "Token refreshed"
        //    };
        //}



    }
}