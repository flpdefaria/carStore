using CarStore.Application.Common;
using CarStore.Domain.Entities;

namespace CarStore.Application.Services;

public interface IAuthorService
{
    Task<List<Author>> GetAllAsync();
    Task<PagedResult<Author>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Author?> GetByIdAsync(int id);
    Task<Author> CreateAsync(Author author);
    Task<Author?> UpdateAsync(int id, Author author);
    Task<bool> DeleteAsync(int id);
}
