using CarStore.Application.Services;
using CarStore.Domain.Entities;
using CarStore.Domain.Exceptions;
using CarStore.Web.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace CarStore.Web.Controllers.Api;

[ApiController]
[Route("api/brands")]
public class BrandsApiController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsApiController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BrandDto>>> Index(int page = 1, int pageSize = 10)
    {
        var result = await _brandService.GetPagedAsync(page, pageSize);
        var dto = new PagedResultDto<BrandDto>
        {
            Items = result.Items.Select(ToDto).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasPrevious = result.HasPrevious,
            HasNext = result.HasNext
        };

        return Ok(dto);
    }

    [HttpGet("options")]
    public async Task<ActionResult<List<BrandOptionDto>>> Options()
    {
        var brands = await _brandService.GetAllAsync();
        var dto = brands.Select(a => new BrandOptionDto { Id = a.Id, Name = a.Name }).ToList();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<BrandDto>> Create(CreateBrandRequest request)
    {
        try
        {
            var brand = new Brand
            {
                Name = request.Name,
                Description = request.Description,
                Country = request.Country,
                FoundedDate = request.FoundedDate
            };
            var created = await _brandService.CreateAsync(brand);
            return CreatedAtAction(nameof(Index), new { id = created.Id }, ToDto(created));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BrandDto>> Update(int id, CreateBrandRequest request)
    {
        try
        {
            var brand = new Brand
            {
                Name = request.Name,
                Description = request.Description,
                Country = request.Country,
                FoundedDate = request.FoundedDate
            };
            var updated = await _brandService.UpdateAsync(id, brand);
            if (updated is null) return NotFound();
            return Ok(ToDto(updated));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _brandService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static BrandDto ToDto(Brand a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Description = a.Description,
        Country = a.Country,
        FoundedDate = a.FoundedDate,
        YearsInBusiness = a.YearsInBusiness,
        CarsCount = a.Cars.Count,
        Cars = a.Cars
            .Select(b => new BrandCarSummaryDto { Model = b.Model, ModelYear = b.ModelYear })
            .ToList()
    };
}
