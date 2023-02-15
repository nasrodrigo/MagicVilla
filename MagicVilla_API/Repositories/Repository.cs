using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDBContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public T Get(int id)
        {
            return dbSet.Find(id)!;
        }

        public IEnumerable<T> GetAll(int pageSize = 0, int pageNumber = 0)
        {
            if (0 < pageSize && 0 < pageNumber)
            {
                if (100 < pageSize)
                {
                    pageSize = 100;
                }

                return dbSet.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            return dbSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>>? filter, int pageSize = 0, int pageNumber = 0)
        {
            if (0 < pageSize && 0 < pageNumber)
            {
                if (100 < pageSize)
                {
                    pageSize = 100;
                }

                return dbSet.Where(filter!).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return dbSet.Where(filter!).ToList();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

    }
}
