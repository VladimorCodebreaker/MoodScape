﻿using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoodScape.Data.Models;

namespace MoodScape.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DatabaseContext _context;

    public GenericRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression);
    }
}

