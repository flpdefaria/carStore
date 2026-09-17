using CarStore.Application.Common;
using CarStore.Domain.Data;
using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarStore.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly CarStoreContext _db;

    public AuthorService(CarStoreContext db)
    {
        _db = db;
    }

    public async Task<List<Author>> GetAllAsync()
    {
        return await _db.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Author>> GetPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _db.Authors.Include(a => a.Books).OrderBy(a => a.Name);
        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Author> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalItems = total };
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Author> CreateAsync(Author author)
    {
        var entity = Author.Create(author.Name, author.Bio, author.Nationality, author.BirthDate);
        _db.Authors.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<Author?> UpdateAsync(int id, Author author)
    {
        var existing = await _db.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (existing is null)
            return null;

        existing.Update(author.Name, author.Bio, author.Nationality, author.BirthDate);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (existing is null)
            return false;

        existing.EnsureCanBeDeleted();
        _db.Authors.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
