using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.DataMappers
{
    public class DataMapper<T> : IDataMapper<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        public DataMapper(ApplicationDBContext db)
        {
            _db = db;
        }
        public void Update(T entity)
        {
            _db.Update(entity);
        }
        public async Task PatchAsync(Expression<Func<T, bool>> filter, JsonPatchDocument<T> entity)
        {
            DbSet<T> dbSet = _db.Set<T>();
            IQueryable<T> query = dbSet;

            query = query.AsNoTracking();
            query = query.Where(filter);

            var entityToPatch = await query.FirstOrDefaultAsync();

            if(entityToPatch is null)
            {
                return;
            }

            entity.ApplyTo(entityToPatch);
            dbSet.Update(entityToPatch);
        }

    }
}
