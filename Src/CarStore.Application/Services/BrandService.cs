using CarStore.Application.Common;
using CarStore.Domain.Data;
using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarStore.Application.Services;

public class BrandService : IBrandService
{
    private readonly CarStoreContext _db;

    public BrandService(CarStoreContext db)
    {
        _db = db;
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _db.Brands
            .Include(a => a.Cars)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Brand>> GetPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _db.Brands.Include(a => a.Cars).OrderBy(a => a.Name);
        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Brand> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalItems = total };
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _db.Brands
            .Include(a => a.Cars)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Brand> CreateAsync(Brand brand)
    {
        var entity = Brand.Create(brand.Name, brand.Description, brand.Country, brand.FoundedDate);
        _db.Brands.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<Brand?> UpdateAsync(int id, Brand brand)
    {
        var existing = await _db.Brands.FirstOrDefaultAsync(a => a.Id == id);
        if (existing is null)
            return null;

        existing.Update(brand.Name, brand.Description, brand.Country, brand.FoundedDate);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Brands
            .Include(a => a.Cars)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (existing is null)
            return false;

        existing.EnsureCanBeDeleted();
        _db.Brands.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
