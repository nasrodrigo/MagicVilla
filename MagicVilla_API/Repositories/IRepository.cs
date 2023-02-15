using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace MagicVilla_API.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll(int pageSize = 0, int pageNumber = 0);
        IEnumerable<T> Find(Expression<Func<T, bool>>? filter, int pageSize = 0, int pageNumber = 0);
        void Add(T entity);
        void Remove(T entity);
    }
}
