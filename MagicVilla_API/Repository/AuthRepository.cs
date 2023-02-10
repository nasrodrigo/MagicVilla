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
    public class AuthRepository : IAuthRepository
    {
        internal ApplicationDBContext _db;
        private readonly ILogger<AuthRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string? secretKey;

        private readonly ApplicationUserToUserDTOMapper applicationUserToLocalUserDTOMapper;
        private readonly UserDTOToApplicationUserMapper userDTOToApplicationUserMapper;

        public AuthRepository(ApplicationDBContext db, ILogger<AuthRepository> logger,
            IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            applicationUserToLocalUserDTOMapper = new();
            userDTOToApplicationUserMapper = new();
        }

        public async Task<bool> IsExistingUser(String userName)
        {
            try
            {
                return null != await _db.ApplicationUser.FirstOrDefaultAsync(user => userName.ToLower() == user.UserName!.ToLower());
            }
            catch (Exception e)
            {
                _logger.LogError("EXCEPTION: {message}", e.Message);
            }

            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                ApplicationUser? user = await _db.ApplicationUser.FirstOrDefaultAsync(user =>
                loginRequestDTO.UserName!.ToLower() == user.UserName!.ToLower());

                if (null != user)
                {
                    bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password!);

                    if (!isPasswordValid)
                    {
                        return new LoginResponseDTO();

                    }

                    var roles = await _userManager.GetRolesAsync(user);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(secretKey!);

                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.Name, user.UserName!.ToLower()),
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

                    return new LoginResponseDTO
                    {
                        User = applicationUserToLocalUserDTOMapper.CreateMap(user),
                        Roles = roles.ToList<string>(),
                        Token = tokenHandler.WriteToken(token)
                    };
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError("EXCEPTION: {message}", e.Message);
            }

            return new LoginResponseDTO();
        }

        public async Task Register(UserDTO user)
        {
            try
            {
                ApplicationUser applicationUser = userDTOToApplicationUserMapper.CreateMap(user);
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
            catch (Exception e)
            {
                _logger.LogError("EXCEPTION: {message}", e.Message);
            }
        }
    }
}
