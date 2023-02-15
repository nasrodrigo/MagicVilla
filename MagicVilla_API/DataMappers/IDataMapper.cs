using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace MagicVilla_API.DataMappers
{
    public interface IDataMapper<T> where T : class
    {
        void Update(T entity);
        Task PatchAsync(Expression<Func<T, bool>> filter, JsonPatchDocument<T> entity);
    }
}
