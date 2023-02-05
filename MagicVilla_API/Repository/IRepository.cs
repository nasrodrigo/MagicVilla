﻿using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetByAsync(Expression<Func<T, bool>> filter = null, bool tracked = false);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> PatchAsync(Expression<Func<T, bool>> filter, JsonPatchDocument<T> entityPatch);
        Task SaveAsync();

    }
}