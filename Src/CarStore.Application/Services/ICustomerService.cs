using CarStore.Application.Common;
using CarStore.Domain.Entities;

namespace CarStore.Application.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();
    Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer?> UpdateAsync(int id, Customer customer);
    Task<bool> DeleteAsync(int id);
}
