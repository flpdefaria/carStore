---
type: guide
created: 2026-07-22
updated: 2026-07-22
tags:
  - note
  - type/guide
  - area/engineering
  - component/application
---

# Application Services

The orchestration pattern used by `AuthorService`, `BookService`, and `CustomerService` in `CarStore.Application`.

## Context

Application services sit between controllers and the domain. They load aggregates, delegate to domain methods, and persist changes. They do not contain business rules.

## Responsibilities

1. Load aggregates from `CarStoreContext` (usually with `.Include(...)`).
2. Call domain factory or update methods (`Brand.Create`, `Car.Update`, etc.).
3. Persist with `_db.SaveChangesAsync()`.
4. Return the result or `null`/`false` when an entity is not found.

## Loading related data

List methods eager-load and order results:

```csharp
// Src/CarStore.Application/Services/AuthorService.cs
return await _db.Brands
    .Include(a => a.Cars)
    .OrderBy(a => a.Name)
    .ToListAsync();
```

```csharp
// Src/CarStore.Application/Services/BookService.cs
return await _db.Cars
    .Include(b => b.Brand)
    .OrderBy(b => b.Title)
    .ToListAsync();
```

`Customer` has no navigation properties, so `CustomerService` skips `.Include(...)`:

```csharp
// Src/CarStore.Application/Services/CustomerService.cs
return await _db.Customers
    .OrderBy(c => c.FullName)
    .ToListAsync();
```

## Create pattern

```csharp
// Src/CarStore.Application/Services/BookService.cs
public async Task<Car> CreateAsync(Car car, int numberOfPages)
{
    var authorExists = await _db.Brands.AnyAsync(a => a.Id == car.AuthorId);
    if (!authorExists)
        throw new DomainException("Brand does not exist.");

    var entity = Car.Create(car.Title, car.Vin, /* ... */ car.AuthorId, numberOfPages);
    _db.Cars.Add(entity);
    await _db.SaveChangesAsync();
    return entity;
}
```

## Update pattern

```csharp
// Src/CarStore.Application/Services/AuthorService.cs
var existing = await _db.Brands.FirstOrDefaultAsync(a => a.Id == id);
if (existing is null)
    return null;

existing.Update(brand.Name, brand.Bio, brand.Country, brand.FoundedDate);
await _db.SaveChangesAsync();
return existing;
```

## Delete pattern

```csharp
// Src/CarStore.Application/Services/AuthorService.cs
if (existing is null)
    return false;

existing.EnsureCanBeDeleted();
_db.Brands.Remove(existing);
await _db.SaveChangesAsync();
return true;
```

`CustomerService.DeleteAsync` removes directly - `Customer` has no `EnsureCanBeDeleted()` guard:

```csharp
// Src/CarStore.Application/Services/CustomerService.cs
if (existing is null)
    return false;

_db.Customers.Remove(existing);
await _db.SaveChangesAsync();
return true;
```

## What not to do

- Do not set entity properties directly from the service.
- Do not duplicate domain validation logic.
- Do not materialize the full table to paginate (see [[Pagination]]).

## Related

- [[Architecture-Overview]]
- [[Pagination]]
- [[Controllers]]
- [[Domain-Overview]]
