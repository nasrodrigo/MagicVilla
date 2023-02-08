using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repository
{
    public class LocalUserRepository : Repository<LocalUser>, ILocalUserRepository
    {
        private string secretKey;
        internal DbSet<LocalUserRole> dbSetRole;
        private readonly ApplicationDBContext _db;
        public LocalUserRepository(ApplicationDBContext db, IConfiguration configuration) : base(db)
        {
            dbSetRole = db.Set<LocalUserRole>();
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<bool> isExistingUser(String userName)
        {
            return null != await dbSet.FirstOrDefaultAsync(user => userName.ToLower() == user.UserName.ToLower());
        }

        public async Task<LoginResponse> Login(LoginRequestDTO loginRequestDTO)
        {
            LocalUser localUser = await dbSet.FirstOrDefaultAsync(user =>
                loginRequestDTO.UserName == user.UserName && loginRequestDTO.Password == user.Password);
            if (localUser is null)
            {
                return new LoginResponse
                {
                    User = null,
                    Token = ""
                };
            }

            IQueryable<LocalUserRole> query = dbSetRole;

            query = query.Where(role => localUser.Id == role.LocalUserId);

            query.AsNoTracking();

            List<LocalUserRole> roles = await query.ToListAsync();

            localUser.Roles = roles;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, localUser.Id.ToString()),
            };

            localUser.Roles.ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role));
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponse
            {
                User = localUser,
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task Register(LocalUser localUser)
        {
            await dbSet.AddAsync(localUser);
            await _db.SaveChangesAsync();

            localUser = await dbSet.FirstOrDefaultAsync(user =>
                localUser.UserName.ToLower() == user.UserName.ToLower());

            foreach (LocalUserRole role in localUser.Roles)
            {
                role.LocalUserId = localUser.Id;
                await dbSetRole.AddAsync(role);
            }
        }
    }
}
