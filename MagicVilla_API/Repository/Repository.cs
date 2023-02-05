﻿using MagicVilla_API.Controllers;
using MagicVilla_API.Data;
using MagicVilla_API.Mapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ILogger<VillaApiController> _logger;
        private readonly ApplicationDBContext _db;
        internal DbSet<T> dbSet;

        public Repository(ILogger<VillaApiController> Logger, ApplicationDBContext db)
        {
            _logger = Logger;
            _db = db;
            dbSet = _db.Set<T>();
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

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;

            if (null != filter)
            {
                query = query.Where(filter);
            }

            query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<T> GetByAsync(Expression<Func<T, bool>> filter = null, bool tracked = false)
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

            return await query.FirstOrDefaultAsync();
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

            T entity = await query.FirstOrDefaultAsync();

            entityPatch.ApplyTo(entity);

            dbSet.Update(entity);
            await SaveAsync();

            return entity;
        }

    }
}