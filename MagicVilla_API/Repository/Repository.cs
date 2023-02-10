using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;
        private readonly ILogger<Repository<T>> _logger;

        public Repository(ApplicationDBContext db, ILogger<Repository<T>> logger)
        {
            _db = db;
            dbSet = _db.Set<T>();
            _logger = logger;
        }
        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();

        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null, int pageSize = 0, int pageNumber = 0)
        {
            IQueryable<T> query = dbSet;

            if (null != filter)
            {
                query = query.Where(filter);
            }

            if (0 < pageSize && 0 < pageNumber)
            {
                if (100 < pageSize)
                {
                    pageSize = 100;
                }

                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<T> GetByAsync(Expression<Func<T, bool>> filter, bool tracked = false)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (null != filter)
            {
                query = query.Where(filter);
            }

            return (await query.FirstOrDefaultAsync())!;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await SaveAsync();

            return entity;
        }

        public async Task<T> PatchAsync(Expression<Func<T, bool>> filter, JsonPatchDocument<T> entityPatch)
        {
            IQueryable<T> query = dbSet;

            query = query.AsNoTracking();
            query = query.Where(filter);

            var entity = (await query.FirstOrDefaultAsync())!;

            entityPatch.ApplyTo(entity);
            dbSet.Update(entity);
            await SaveAsync();

            return entity;

        }

    }
}
