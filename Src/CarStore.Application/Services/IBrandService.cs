using CarStore.Application.Common;
using CarStore.Domain.Entities;

namespace CarStore.Application.Services;

public interface IBrandService
{
    Task<List<Brand>> GetAllAsync();
    Task<PagedResult<Brand>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand> CreateAsync(Brand brand);
    Task<Brand?> UpdateAsync(int id, Brand brand);
    Task<bool> DeleteAsync(int id);
}
