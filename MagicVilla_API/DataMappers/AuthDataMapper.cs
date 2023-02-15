using MagicVilla_API.Models;
using MagicVilla_API.Transformers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.DataMappers
{
    public class AuthDataMapper : IAuthDataMapper
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string? secretKey;

        private readonly ApplicationUserToUserTransformer applicationUserToUserMapper;
        private readonly UserToApplicationUserTransformer userToApplicationUserMapper;

        public AuthDataMapper(
            ApplicationDBContext db,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            applicationUserToUserMapper = new();
            userToApplicationUserMapper = new();
        }

        public async Task<bool> IsExistingUser(string userName)
        {
            return null != await _db.ApplicationUser.FirstOrDefaultAsync(user => userName.ToLower() == user.UserName!.ToLower());
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            ApplicationUser? user = await _db.ApplicationUser.FirstOrDefaultAsync(user =>
                loginRequest.UserName!.ToLower() == user.UserName!.ToLower());

            bool isPasswordValid = false;

            if (null != user)
            {
                isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password!);
            }

            if (!isPasswordValid)
            {
                return new LoginResponse();

            }

            var roles = await _userManager.GetRolesAsync(user!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey!);

            List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, user!.UserName!.ToLower()),
                };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponse
            {
                User = applicationUserToUserMapper.CreateMap(user),
                Roles = roles.ToList<string>(),
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task Register(User user)
        {
            ApplicationUser applicationUser = userToApplicationUserMapper.CreateMap(user);
            var result = await _userManager.CreateAsync(applicationUser, user.Password!);

            if (result.Succeeded)
            {
                foreach (var role in user.Roles!)
                {
                    if (await _roleManager.FindByNameAsync(role.ToUpper()) is null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role.ToUpper()));
                    }

                    await _userManager.AddToRoleAsync(applicationUser, role.ToUpper());
                }

            }
        }
    }
}
