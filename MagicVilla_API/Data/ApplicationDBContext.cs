using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options){ }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaNumber> VillaNumber { get; set; }
    }
}
