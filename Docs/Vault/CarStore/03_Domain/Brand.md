---
type: reference
created: 2026-07-22
updated: 2026-07-22
tags:
  - note
  - type/reference
  - area/domain
  - entity/brand
---

# Brand

The `Brand` domain entity: properties, computed members, factory/update methods, and invariants.

## Source

`Src/CarStore.Domain/Entities/Brand.cs`

## Properties

| Property | Type | Notes |
|----------|------|-------|
| `Id` | `int` | Database-generated identity. |
| `Name` | `string` | Required, max 150 characters, trimmed on create/update. |
| `Description` | `string` | Max 2000 characters, trimmed. |
| `FoundedDate` | `DateTime` | Cannot be in the future. |
| `Country` | `string` | Max 100 characters, trimmed. |
| `Cars` | `ICollection<Car>` | One-to-many navigation collection. |

## Computed members

- `YearsInBusiness` — `(int)Math.Floor((DateTime.UtcNow - FoundedDate).TotalDays / 365.25)`.

## Factory and mutators

- `Brand.Create(name, description, country, foundedDate)` — validates then returns a new instance.
- `Update(name, description, country, foundedDate)` — validates then mutates the existing instance.

## Invariants (exact `DomainException` messages)

| Condition | Message |
|-----------|---------|
| `Name` is null/empty/whitespace | `"Brand name is required."` |
| `Name` length > 150 | `"Brand name must be at most 150 characters."` |
| `FoundedDate` is in the future | `"Founded date cannot be in the future."` |

## Deletion rule

```csharp
public void EnsureCanBeDeleted()
{
    if (Cars.Any())
        throw new DomainException("Cannot delete a brand that still has cars.");
}
```

## EF Core mapping

In `Src/CarStore.Domain/Data/CarStoreContext.cs`:

```csharp
modelBuilder.Entity<Brand>(entity =>
{
    entity.HasKey(a => a.Id);
    entity.Property(a => a.Name).IsRequired().HasMaxLength(150);
    entity.Property(a => a.Description).HasMaxLength(2000);
    entity.Property(a => a.Country).HasMaxLength(100);
});
```

## Related

- [[Car]]
- [[Domain-Overview]]
- [[Architecture-Overview]]
