
 Recommended project structure using **Clean/Onion Architecture** principles adapted for a desktop or local application (e.g., WPF, WinForms, or MAUI).

## Recommended Project Structure

Plaintext

```
ApartmentManagement/
├── src/
│   ├── Core/                           # Domain Entities & Business Interfaces
│   │   ├── Domain/
│   │   │   ├── Entities/
│   │   │   │   ├── Tenant.cs
│   │   │   │   ├── Apartment.cs
│   │   │   │   ├── PaymentRecord.cs
│   │   │   │   ├── Issue.cs
│   │   │   │   └── Parcel.cs
│   │   │   └── ValueObjects/           # e.g., Money, DateRange
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs          # Generic Local DAO interface
│   │   │   └── IDbInitializer.cs       # Local DB setup/migration
│   │   └── Exceptions/                 # Custom local handling exceptions
│   │
│   ├── Application/                    # Use Cases & Business Logic
│   │   ├── Services/
│   │   │   ├── RentCalculator.cs       # Local calculation logic
│   │   │   ├── OccupancyManager.cs
│   │   │   └── ParcelTracker.cs
│   │   └── DTOs/                       # Data Transfer Objects for UI
│   │
│   ├── Infrastructure/                 # Data Access & Local Storage
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs         # SQLite EF Core Context (or Dapper)
│   │   │   ├── Migrations/             # Database scheme updates
│   │   │   └── Repositories/           # Concrete DAO implementations
│   │   └── Logging/                    # Local file logger (e.g., Serilog to local file)
│   │
│   └── Presentation/                   # UI Layer (WPF, WinForms, or MAUI)
│       ├── ViewModels/                 # If MVVM pattern is used
│       ├── Views/
│       └── App.xaml.cs                 # Dependency Injection setup
│
└── tests/
    ├── UnitTests/                      # Test business logic & rent calculations
    └── IntegrationTests/               # Test local SQLite transactions & file operations
```

## Structural Guidelines & Best Practices

### 1. Domain Entities (`Core/Domain/Entities`)

Define standard local entities directly matching domain boundaries without external attributes:

C#

```
namespace ApartmentManagement.Core.Domain.Entities
{
    public class Parcel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ArrivalTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime? PickupTimestamp { get; set; }
        public bool IsPickedUp => PickupTimestamp.HasValue;
    }
}
```

### 2. Local Data Access & Integrity (`Infrastructure/Persistence`)

Because the system runs locally without a backend server, local transaction handling and crash resilience are critical:

- **Database Choice:** **SQLite** via **Entity Framework Core** or **Dapper** is recommended for local C# applications.
    
- **ACID Compliance:** Ensure all multi-entity operations (e.g., assigning a tenant to an apartment while logging an initial payment record) execute within a local transaction.
    

C#

```
public async Task<bool> AssignTenantAsync(Guid apartmentId, Guid tenantId)
{
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
        // 1. Update apartment status
        // 2. Associate tenant
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw; // Handle or log locally
    }
}
```

### 3. Local File Logging

To diagnose local issues or data integrity failures without network connectivity, route application logs to a local file (e.g., using `Serilog.Sinks.File`).
