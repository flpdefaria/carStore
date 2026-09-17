using CarStore.Application.Common;
using CarStore.Domain.Entities;

namespace CarStore.Application.Services;

public interface ICarService
{
    Task<List<Car>> GetAllAsync();
    Task<PagedResult<Car>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Car?> GetByIdAsync(int id);
    Task<Car> CreateAsync(Car car, int mileage);
    Task<Car?> UpdateAsync(int id, Car car, int mileage);
    Task<bool> DeleteAsync(int id);
}
