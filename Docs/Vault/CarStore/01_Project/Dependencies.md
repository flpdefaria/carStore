---
type: reference
created: 2026-07-22
updated: 2026-07-22
tags:
  - note
  - type/reference
  - area/project
  - component/domain
  - component/application
  - component/web
  - component/tests
---

# Dependencies

NuGet package and project-reference inventory for each CarStore project.

## Context

Keeping this inventory current avoids silent drift when packages are added, removed, or upgraded.

## Project references

| Project | References |
|---------|------------|
| `CarStore.Web` | `CarStore.Application`, `CarStore.Domain` |
| `CarStore.Application` | `CarStore.Domain` |
| `CarStore.Tests` | `CarStore.Domain` |
| `CarStore.Domain` | (none) |

## NuGet packages

### CarStore.Domain

| Package | Version |
|---------|---------|
| `Microsoft.EntityFrameworkCore.InMemory` | `10.0.0` |

### CarStore.Tests

| Package | Version |
|---------|---------|
| `coverlet.collector` | `6.0.4` |
| `FluentAssertions` | `8.10.0` |
| `Microsoft.NET.Test.Sdk` | `17.14.1` |
| `xunit` | `2.9.3` |
| `xunit.runner.visualstudio` | `3.1.4` |

### CarStore.Web and CarStore.Application

No direct NuGet packages beyond the .NET 10 SDK and implicit project references.

## Source files

- `Src/CarStore.Domain/CarStore.Domain.csproj`
- `Src/CarStore.Application/CarStore.Application.csproj`
- `Src/CarStore.Web/CarStore.Web.csproj`
- `Src/CarStore.Tests/CarStore.Tests.csproj`

## Related

- [[Repository-Structure]]
- [[Dotnet-Aspnetcore-Ef-Docs]]
