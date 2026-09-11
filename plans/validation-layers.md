# Three-Layer Validation Plan (Entity · DTO · Logic)

> **Status:** Proposed (In Review)  
> **Note:** Plan requested by Eng. Mohammed Tala'at for adding DB CHECK constraints, DTO validation, and service-level business rules.

---

## Goal

Enforce data integrity at every boundary so invalid data can never reach the database, and every failure surfaces a clear **400/409** response in Swagger.

| Layer | Owner | Mechanism | Status Today |
| :--- | :--- | :--- | :--- |
| **Entity (schema)** | Domain entities + `AppDbContext` (Infrastructure) | DataAnnotations + Fluent API → `NOT NULL`, `MaxLength`, DB **CHECK constraints** via new migration | Authored |
| **DTO (input)** | Application DTOs | **FluentValidation** validators (new dependency), auto-400 via `[ApiController]` | DataAnnotations exist; to be replaced |
| **Logic (business)** | Application services | `BusinessRuleException` + FK sanitize/existence + occupancy consistency + cross-field rules | None |

## Current State (Verified in Code)

- **Entity/DB:** `InitialCreate` maps all entity strings to nullable `nvarchar(max)`; no `MaxLength`, no `NOT NULL` for string columns, **no CHECK constraints**. `AppDbContext` (`src/Core/Infrastructure/Data/AppDbContext.cs`) only configures FK delete behaviors, `decimal(10,2)`, and 5 unique indexes.
- **DTO:** All 7 DTOs have DataAnnotations (v0.7.0). `[ApiController]` on `BaseController` auto-returns 400 for `ModelState` failures.
- **Logic/services:** All 7 service implementations are bare CRUD (null-guard + map + repo call). No FK sanitization, no FK existence checks, no occupancy rules, no status/cross-field rules.

---

## Layer A — Entity Validation (Infrastructure)

### A1. DataAnnotations on the 7 Domain entities

Use `System.ComponentModel.DataAnnotations` (built-in, **no package**). EF Core honors `[Required]` and `[MaxLength]` in `OnModelCreating`, so these become `NOT NULL` / column-length facets in the generated migration.

| Entity | Attribute changes |
| :--- | :--- |
| `Role.RoleName` | `[Required]`, `[MaxLength(50)]` |
| `Tenant.FullName` | `[Required]`, `[MaxLength(100)]` |
| `Tenant.PhoneNumber` | `[Required]`, `[MaxLength(20)]` |
| `Tenant.EmergencyContact` | `[MaxLength(20)]` (nullable) |
| `Apartment.UnitNumber` | `[Required]`, `[MaxLength(20)]` ← **decision: spec=20 vs current DTO=4** |
| `Apartment.FloorNumber` | `[Range(0, 20)]` (documentation only — no EF effect) |
| `Issue.Description` | `[Required]`, `[MaxLength(500)]` ← **decision: spec=500 vs current DTO=300** |
| `Parcel.CourierName` | `[MaxLength(100)]` |
| `PaymentRecord.AmountPaid` | `[Range(0.01, 1000000)]` (documentation only) |
| `PaymentRecord.PaymentPeriodMonth` | `[Range(1, 12)]` |
| `PaymentRecord.PaymentPeriodYear` | `[Range(2000, 2100)]` |
| `User.Username` / `User.Email` | `[Required]`, `[MaxLength(50)]` / `[MaxLength(254)]` |

> `[Range]` has no EF relational effect; the actual numeric bounds are enforced by CHECK constraints (A2).

### A2. Fluent API + DB CHECK constraints in `AppDbContext.OnModelCreating`

Restores the CHECK constraints promised in `context.md` §3.1 that never existed (entities use string statuses, so the allow-list must come from SQL):

```csharp
modelBuilder.Entity<Apartment>(entity => entity
    .ToTable(t => t.HasCheckConstraint("CK_Apartments_OccupancyStatus",
        "OccupancyStatus IN ('Vacant','Occupied','Maintenance')"))
    .ToTable(t => t.HasCheckConstraint("CK_Apartments_MonthlyRent", "MonthlyRent >= 0")));

modelBuilder.Entity<PaymentRecord>(entity => entity
    .ToTable(t => t.HasCheckConstraint("CK_PaymentRecords_Status",
        "Status IN ('Paid','Partial','Pending')"))
    .ToTable(t => t.HasCheckConstraint("CK_PaymentRecords_AmountPaid", "AmountPaid > 0"))
    .ToTable(t => t.HasCheckConstraint("CK_PaymentRecords_Month",
        "PaymentPeriodMonth BETWEEN 1 AND 12"))
    .ToTable(t => t.HasCheckConstraint("CK_PaymentRecords_Year",
        "PaymentPeriodYear BETWEEN 2000 AND 2100")));

modelBuilder.Entity<Issue>(entity => entity
    .ToTable(t => t.HasCheckConstraint("CK_Issues_Status",
        "Status IN ('Open','In Progress','Resolved')")));

modelBuilder.Entity<Parcel>(entity => entity
    .ToTable(t => t.HasCheckConstraint("CK_Parcels_Status",
        "Status IN ('Pending Pickup','Picked Up')")));
```

Existing config (unique indexes, `decimal(10,2)`, FK delete behaviors) stays unchanged.

### A3. Migration

- `dotnet ef migrations add AddEntityValidationConstraints`
- `dotnet ef database update`
- **Risk:** turning nullable columns into `NOT NULL` fails if existing rows contain NULLs. Assume empty/local dev DB; verify row counts first.

---

## Layer B — DTO Validation (FluentValidation)

### B1. Dependencies (net5.0-compatible)

| Project | Package | Version |
| :--- | :--- | :--- |
| `Application.csproj` | `FluentValidation` | 10.3.x |
| `ApartmentManagement.API.csproj` | `FluentValidation.AspNetCore` | 10.3.x |
| `Tests.csproj` | `FluentValidation` | 10.3.x |

### B2. New validators — `src/Core/Application/Validators/`

Seven validator classes mirroring the field rules that currently live on DTOs, **standardized** and with fixes:

- `TenantDtoValidator`, `ApartmentDtoValidator`, `UserDtoValidator`, `PaymentRecordDtoValidator`, `IssueDtoValidator`, `ParcelDtoValidator`, `RoleDtoValidator`
- Status allow-lists via `.Must(...)` for the 4 string status fields:
  - `OccupancyStatus`: `Vacant | Occupied | Maintenance`
  - `PaymentRecord.Status`: `Paid | Partial | Pending`
  - `Issue.Status`: `Open | In Progress | Resolved`
  - `Parcel.Status`: `Pending Pickup | Picked Up`
- Fixes over current DTOs:
  - Drop pointless "required" checks on non-nullable value types (rely on `.GreaterThan(0)` / `.InclusiveBetween(...)`).
  - `UserDto.TenantId`: must be null or `> 0` (currently unvalidated).
  - Standardize UnitNumber / Description / RoleName lengths per the decisions below.
- **No cross-field rules here** — cross-field and business rules live in services (Layer C).

### B3. Registration & removal of old enforcement

```csharp
// Program.cs — replaces the current plain AddControllers()
services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssembly(typeof(TenantDtoValidator).Assembly));
```

- Mapped into `ModelState` during model binding → `[ApiController]` auto-returns **400** (same client UX as today).
- **Remove** the now-redundant DataAnnotations from the 7 DTOs.
- ⚠️ **Tradeoff:** Swashbuckle renders DataAnnotations in the Swagger schema; FluentValidation rules are not rendered. Clearing of 400/409 message bodies still works. Optional follow-up (not in this plan): a Swashbuckle schema filter, or keep doc-only attributes on DTOs.

---

## Layer C — Logic Validation (services)

### C1. Shared exception + error-path

```csharp
// src/Core/Application/ErrorHandling/BusinessRuleException.cs
namespace Application.ErrorHandling
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
```

Extend `ApiErrorMapper.TryMap` (in `src/Core/Application/ErrorHandling/ApiErrorMapper.cs`):

```csharp
if (ex is BusinessRuleException businessRule)
    return new ErrorMapping { StatusCode = 400, Message = businessRule.Message };
```

Because every controller funnels through `BaseController.HandleError`, all 35 endpoints inherit the mapping with **zero controller changes**.

### C2. FK sanitization + existence (Phase 5)

Services gain repository dependencies (all are already DI-registered in `Program.cs`; service **interfaces unchanged**).

| Service | Added repo deps | Rules applied on Create **and** Update |
| :--- | :--- | :--- |
| `ApartmentServiceImpl` | `ITenantRepository` | `CurrentTenantId <= 0 → null`; if set, tenant must exist |
| `UserServiceImpl` | `IRoleRepository`, `ITenantRepository` | `TenantId <= 0 → null`; `RoleId` must exist; tenant must exist when provided |
| `PaymentRecordServiceImpl` | `ITenantRepository`, `IApartmentRepository` | `TenantId` and `ApartmentId` must exist |
| `IssueServiceImpl` | `ITenantRepository`, `IApartmentRepository` | `TenantId` and `ApartmentId` must exist |
| `ParcelServiceImpl` | `ITenantRepository` | `TenantId` must exist |

Missing reference → `throw new BusinessRuleException("...")` → 400.

### C3. Occupancy consistency (Phase 6)

`ApartmentServiceImpl` — shared sanitize/sync helper (matches master plan §7.1):

```csharp
private void SanitizeOccupancy(ApartmentDto dto)
{
    if (dto.CurrentTenantId.HasValue && dto.CurrentTenantId.Value <= 0)
        dto.CurrentTenantId = null;

    if (dto.CurrentTenantId.HasValue)
        dto.OccupancyStatus = "Occupied";
    else if (dto.OccupancyStatus != "Maintenance")
        dto.OccupancyStatus = "Vacant";
}
```

- **CreateApartment:** sanitize before map/add.
- **UpdateApartment:** load existing unit; if it is `Occupied` and `dto.CurrentTenantId` is a different tenant → `BusinessRuleException("Unit is currently occupied by another tenant.")` (*"tenants cannot be assigned to non-vacant units"*).

`TenantServiceImpl`:
- Inject `IApartmentRepository`.
- `DeleteTenant(id)`: after deletion, load apartments with `CurrentTenantId == id` and set `OccupancyStatus = "Vacant"` (the `SetNull` FK already clears `CurrentTenantId` at the DB).

### C4. Cross-field value rules (services only)

- **Issue** (Create + Update; Update loads existing first):
  - `Status == "Resolved"` → `ResolvedDate` required.
  - `Status != "Resolved"` → `ResolvedDate` must be null (cleared).
- **Parcel** (Create + Update):
  - `Status == "Picked Up"` → `PickupTimestamp` required.
  - `Status == "Pending Pickup"` → `PickupTimestamp` must be null.
- **PaymentRecord** ← **decision:** optional `Status = "Partial"` requires `AmountPaid < Apartment.MonthlyRent` (needs `IApartmentRepository` + rent lookup); default skip.

---

## Tests

- **Update the 7 existing service test classes** for the new constructor dependencies (add the extra mocked repos).
- **New `src/Tests/Validators/`** — one test file per validator (null/empty, length, range, format, status allow-list).
- **New service-logic tests:**
  - FK sanitization: `currentTenantId: 0` → stored null.
  - FK existence: unknown `tenantId`/`apartmentId`/`roleId` → `BusinessRuleException`.
  - Occupancy: tenant present → `Occupied`; none + non-Maintenance → `Vacant`; occupied unit reassigned → throws.
  - `DeleteTenant` flips affected apartments to `Vacant`.
  - Issue/Parcel cross-field rules.
- **`ApiErrorMapperTests`:** add `BusinessRuleException` → 400 case.
- Verify: `dotnet build ApartmentManagementSystem.sln` (0 errors) and `dotnet test src/Tests/` (all green).

---

## Documents to Update

- Write this plan → **`plans/validation-layers.md`** (this file).
- **`context.md`:** §4 (DB CHECK constraints now enforced; columns no longer all-nullable), §5 (DTO validation via FluentValidation), §7 (mark validation work implemented).

## Files Summary

| Action | File |
| :--- | :--- |
| Modify ×7 | `src/Core/Domain/Entities/*.cs` (DataAnnotations) |
| Modify | `src/Core/Infrastructure/Data/AppDbContext.cs` (CHECK constraints) |
| **New** | `src/Core/Infrastructure/Migrations/*AddEntityValidationConstraints*` |
| Modify ×7 | `src/Core/Application/DTOs/*.cs` (remove DataAnnotations) |
| **New** ×7 | `src/Core/Application/Validators/*DtoValidator.cs` |
| **New** | `src/Core/Application/ErrorHandling/BusinessRuleException.cs` |
| Modify | `src/Core/Application/ErrorHandling/ApiErrorMapper.cs` (400 mapping) |
| Modify ×7 | `src/Core/Application/ServiceImpl/*.cs` (logic validation + repo deps) |
| Modify | `src/Core/ApartmentManagement.API/Program.cs` (FluentValidation registration) |
| Modify ×3 | `.csproj` files (FluentValidation 10.3.x) |
| Modify ×7 + new | `src/Tests/Services/*.cs` (ctor + logic tests), `src/Tests/Validators/*.cs`, `ApiErrorMapperTests.cs` |
| Modify ×2 | `context.md`, `plans/validation-layers.md` |
| **New** | `plans/` doc (master plan reference entry if desired) |

## Decisions Required Before Implementation

1. **UnitNumber / Description / RoleName lengths** — standardize to spec (20 / 500 / 50) or keep current DTO values (4 / 300 / 100)?
2. **Remove DTO DataAnnotations** entirely (FluentValidation becomes the single source of truth, accepting the Swagger constraint-display loss) or keep doc-only attributes?
3. **PaymentRecord `Partial` vs `MonthlyRent`** cross-check — implement or skip?

## Constraints & Notes

- Clean Architecture preserved: entity enforcement in Infrastructure/Domain, input validation in Application DTOs, business rules in Application services; controllers/API unchanged except `Program.cs`.
- Statuses remain **strings** (status-enum work is a separate planned item); CHECK constraints + validator `.Must(...)` compensate.
- Zero-budget constraint relaxed here by design choice: FluentValidation is an MIT/open-source dependency (user-approved).
- New migration must not be edited after generation; future schema changes use new migrations.