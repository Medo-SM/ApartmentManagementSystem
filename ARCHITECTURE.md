# Apartment Management System - Architecture Specification

This document defines the architectural guidelines and design principles for the **Apartment Management System**. The solution is built following **Clean Architecture (Onion Architecture)** principles adapted for modern **C# / .NET** applications.

---

## Architectural Overview & Dependency Principles

The core rule of Clean Architecture in .NET is that **inner layers define domain rules and abstractions, while outer layers implement technical details**. Dependencies flow strictly inward.

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                    │
│           (WPF / MAUI / WinForms / ASP.NET Core)        │
└──────────────┬───────────────────────────┬──────────────┘
               │                           │
               ▼                           │
┌──────────────────────────────┐           │
│    Infrastructure Layer      │           │
│ (EF Core, Migrations, Serilog)│           │
└──────────────┬───────────────┘           │
               │                           │
               ▼                           ▼
┌─────────────────────────────────────────────────────────┐
│                    Application Layer                    │
│         (Use Cases, DTOs, Service Interfaces)          │
└──────────────────────────┬──────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                      Domain Layer                       │
│        (Entities, Enums, Value Objects, Exceptions)     │
└─────────────────────────────────────────────────────────┘
```

### Dependency Rules:
1. **Domain Layer**: Independent of all other layers and external libraries. Contains pure C# business entities and rules.
2. **Application Layer**: Depends only on the **Domain** layer. Defines use cases, interfaces (`IRepository`, `IUnitOfWork`, `IDbContext`), DTOs, and validation logic.
3. **Infrastructure Layer**: Depends on **Application** and **Domain**. Implements database persistence (EF Core / Dapper), file logging, and external service contracts.
4. **Presentation Layer**: Depends on **Application** (and **Infrastructure** at the composition root for Dependency Injection setup). Responsible for UI rendering and user interactions.

---

## .NET Solution & Project Structure

The physical layout maps architectural boundaries directly to C# projects (`.csproj`) inside the Visual Studio Solution (`ApartmentManagmentSystem.sln`):

```
ApartmentManagmentSystem.sln
├── src/
│   ├── Core/
│   │   ├── ApartmentManagementSystem.Domain/             # Domain Project (.csproj)
│   │   │   ├── Entities/                                 # Domain Model Entities
│   │   │   │   ├── Apartment.cs
│   │   │   │   ├── Tenant.cs
│   │   │   │   ├── PaymentRecord.cs
│   │   │   │   ├── Issue.cs
│   │   │   │   ├── Parcel.cs
│   │   │   │   ├── User.cs
│   │   │   │   └── Role.cs
│   │   │   ├── Enums/                                    # Domain Enums
│   │   │   │   ├── OccupancyStatus.cs
│   │   │   │   ├── PaymentStatus.cs
│   │   │   │   └── IssueStatus.cs
│   │   │   ├── Exceptions/                               # Core Domain Exceptions
│   │   │   └── Common/                                   # Base Entity definitions
│   │   │
│   │   └── ApartmentManagementSystem.Application/        # Application Project (.csproj)
│   │       ├── Interfaces/                               # Abstractions & Contracts
│   │       │   ├── Persistence/                          # IApplicationDbContext, IUnitOfWork
│   │       │   ├── Repositories/                         # IRepository<T>, ITenantRepository
│   │       │   └── Services/                             # IPasswordHasher, ICurrentUserService
│   │       ├── Services/                                 # Application Services (RentCalculator, etc.)
│   │       ├── DTOs/                                     # Data Transfer Objects
│   │       ├── Common/                                   # Result<T> pattern, Pagination
│   │       └── DependencyInjection.cs                    # Service Registration Extensions
│   │
│   ├── Infrastructure/
│   │   └── ApartmentManagementSystem.Infrastructure/     # Infrastructure Project (.csproj)
│   │       ├── Persistence/
│   │       │   ├── AppDbContext.cs                       # Entity Framework Core DbContext
│   │       │   ├── Configurations/                       # Fluent API Entity Configurations
│   │       │   ├── Repositories/                         # Concrete Repositories
│   │       │   └── Migrations/                           # EF Core DB Migrations
│   │       ├── Security/                                 # Password Hashing Implementation
│   │       ├── Logging/                                  # Serilog / File Logger configuration
│   │       └── DependencyInjection.cs                    # Infrastructure Service Registration
│   │
│   └── Presentation/
│       └── ApartmentManagementSystem.Desktop/           # Desktop / API Project (.csproj)
│           ├── ViewModels/                               # MVVM ViewModels
│           ├── Views/                                    # XAML Views & Controls
│           ├── App.xaml / Program.cs                     # Application Bootstrapper & DI Host
│           └── appsettings.json                          # Database Connection Strings & Settings
│
└── tests/
    ├── ApartmentManagementSystem.Domain.UnitTests/       # Domain Logic Tests
    ├── ApartmentManagementSystem.Application.UnitTests/  # Application Logic & Service Tests
    └── ApartmentManagementSystem.Infrastructure.Tests/    # Repository & Database Integration Tests
```

---

## Detailed Layer Responsibilities

### 1. Domain Layer (`Core/ApartmentManagementSystem.Domain`)
- **Responsibility:** Represents business concepts, entities, rules, and logic.
- **Key Constraints:** Pure C# code. **No NuGet dependencies on database providers or UI frameworks.**
- **Key Components:**
  - `BaseEntity`: Common properties like `Id`, `CreatedAt`, and `UpdatedAt`.
  - `Entities`: Rich or data-backed entities (`Tenant`, `Apartment`, `PaymentRecord`, `Issue`, `Parcel`, `User`, `Role`).
  - `Enums`: Typed states (e.g., `OccupancyStatus.Vacant`, `PaymentStatus.Paid`, `IssueStatus.Open`).

### 2. Application Layer (`Core/ApartmentManagementSystem.Application`)
- **Responsibility:** Orchestrates business operations and application use cases.
- **Key Components:**
  - `Interfaces`: Contracts for repositories (`IRepository<T>`), database context (`IApplicationDbContext`), and services (`IPasswordHasher`).
  - `Services`: Business logic services (e.g., `RentCalculator`, `OccupancyManager`, `ParcelTracker`).
  - `DTOs`: Data transfer structures used for input/output between UI and business layers.
  - `Result<T>`: Uniform wrapper for operational success, warnings, or errors.

### 3. Infrastructure Layer (`Infrastructure/ApartmentManagementSystem.Infrastructure`)
- **Responsibility:** Provides concrete implementations for data access, external systems, and persistence.
- **Key Components:**
  - `AppDbContext`: Entity Framework Core DbContext mapping domain entities to relational tables.
  - `Configurations`: Entity mappings (`IEntityTypeConfiguration<T>`) defining keys, relationships, index constraints, and database column types.
  - `Repositories`: Concrete data access implementations (e.g., `GenericRepository<T>`, `TenantRepository`).
  - `Security`: Password hashing utilities (`PasswordHasher`) adhering to modern cryptographic standards.
  - `Logging`: File loggers (e.g., Serilog) for offline/desktop application logging.

### 4. Presentation Layer (`Presentation/ApartmentManagementSystem.Desktop`)
- **Responsibility:** Handles user input, state rendering, and application bootstrapping.
- **Key Patterns & Components:**
  - **MVVM Pattern**: ViewModels maintain UI state and execute commands; Views handle layout and bindings.
  - **Composition Root**: `App.xaml.cs` or `Program.cs` builds the `IHost` and configures DI containers via `IServiceCollection`.

---

## Technical Recommendations & C# Best Practices

| Concern | Recommended Technology / Library | Purpose |
| :--- | :--- | :--- |
| **Framework** | .NET 8.0 / .NET 9.0 | Long-Term Support (LTS) C# runtime |
| **ORM & Data Access** | Entity Framework Core (EF Core) | Relational database mapping & migrations |
| **Database** | SQL Server / SQLite | Database storage engine |
| **Dependency Injection** | `Microsoft.Extensions.DependencyInjection` | Built-in IoC container |
| **UI Framework** | WPF (with `CommunityToolkit.Mvvm`) or MAUI | Desktop UI application development |
| **Logging** | Serilog (`Serilog.Sinks.File`) | Structured logging to local file system |
| **Validation** | FluentValidation | Clean decoupling of validation rules |
| **Unit Testing** | xUnit, Moq, FluentAssertions | Automated unit testing framework |

---

## C# Code Examples & Architectural Patterns

### 1. Entity Definition (Domain Layer)

```csharp
namespace ApartmentManagementSystem.Core.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    public ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
    public User? User { get; set; }
}
```

### 2. Generic Repository Interface (Application Layer)

```csharp
namespace ApartmentManagementSystem.Core.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
```

### 3. DbContext Configuration (Infrastructure Layer)

```csharp
namespace ApartmentManagementSystem.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Core.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Parcel> Parcels => Set<Parcel>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

### 4. Dependency Injection Registration Root

```csharp
namespace ApartmentManagementSystem.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Core.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Infrastructure.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
```

---

## Data Integrity & Transaction Handling

Operations modifying multiple aggregate roots (such as assigning a tenant to an apartment while logging an initial deposit or payment) must be wrapped in transactional boundaries:

```csharp
public async Task<bool> AssignTenantToApartmentAsync(int apartmentId, int tenantId, decimal depositAmount)
{
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
        var apartment = await _dbContext.Apartments.FindAsync(apartmentId);
        if (apartment == null) return false;

        apartment.CurrentTenantId = tenantId;
        apartment.OccupancyStatus = "Occupied";

        var payment = new PaymentRecord
        {
            TenantId = tenantId,
            ApartmentId = apartmentId,
            AmountPaid = depositAmount,
            PaymentPeriodMonth = DateTime.UtcNow.Month,
            PaymentPeriodYear = DateTime.UtcNow.Year,
            Status = "Paid"
        };

        _dbContext.PaymentRecords.Add(payment);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```
