using MagicVilla_API.Mapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repository
{
    public class LocalUserRepository : ILocalUserRepository
    {
        internal ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;

        ApplicationUserToUserDTOMapper applicationUserToLocalUserDTOMapper = new ApplicationUserToUserDTOMapper();
        UserDTOToApplicationUserMapper userDTOToApplicationUserMapper = new UserDTOToApplicationUserMapper();

        public LocalUserRepository(ApplicationDBContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<bool> isExistingUser(String userName)
        {
            return null != await _db.ApplicationUser.FirstOrDefaultAsync(user => userName.ToLower() == user.UserName.ToLower());
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            ApplicationUser user = await _db.ApplicationUser.FirstOrDefaultAsync(user =>
                loginRequestDTO.UserName.ToLower() == user.UserName.ToLower());

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user is null || !isPasswordValid)
            {
                return new LoginResponseDTO
                {
                    User = null,
                    Token = ""
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName.ToLower()),
            };

            foreach(var role in roles)
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

            return new LoginResponseDTO
            {
                User = applicationUserToLocalUserDTOMapper.CreateMap(user),
                Roles = roles.ToList<string>(),
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task Register(UserDTO user)
        {
            ApplicationUser applicationUser = userDTOToApplicationUserMapper.CreateMap(user);
            var result = await _userManager.CreateAsync(applicationUser, user.Password);
            
            if(result.Succeeded)
            {
                foreach(var role in user.Roles)
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
