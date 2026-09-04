# Master Implementation Plan: Apartment Management System (AMS)

> **Document Purpose:** Master plan for hardening the AMS API against invalid client input. Primary driver: Swagger `POST` calls let users supply an `id`, which crashes with `IDENTITY_INSERT OFF`. Each phase fixes a confirmed defect, adds an example, and includes a verification step.
>
> **Constraints honored:** Clean/Onion layering, `context.md` scope limits, and schema parity with `ApartmentManagmentSchema.sql`.

---

## 📑 Table of Contents

1. [Primary Problem: POST Allows Client-Supplied id](#1-primary-problem-post-allows-client-supplied-id)
2. [Phase 1: Build & Project References Fix (COMPLETED)](#2-phase-1-build--project-references-fix-completed)
3. [Phase 2: AutoMapper Primary Key Isolation](#3-phase-2-automapper-primary-key-isolation)
4. [Phase 3: Domain Status Enums & Swagger Dropdowns](#4-phase-3-domain-status-enums--swagger-dropdowns)
5. [Phase 4: DTO DataAnnotations & API Input Validation](#5-phase-4-dto-dataannotations--api-input-validation)
6. [Phase 5: Service-Layer FK Sanitization & Business Rules](#6-phase-5-service-layer-fk-sanitization--business-rules)
7. [Phase 6: Apartment FK / Occupancy Consistency (Service-Only)](#7-phase-6-apartment-fk--occupancy-consistency-service-only)
8. [Swagger Posting Sequence](#8-swagger-posting-sequence)
9. [Verification & Testing Plan](#9-verification--testing-plan)
10. [Per-Phase Justification Summary](#10-per-phase-justification-summary)

---

## 1. Primary Problem: POST Allows Client-Supplied id

**Reproduction:** In Swagger, `POST /api/Tenant` with body `{ "id": 555, "fullName": "Ahmed", ... }` crashes:

```
Cannot insert explicit value for identity column ... when IDENTITY_INSERT is OFF.
```

**Root cause:** `MappingConfig.cs` uses `.ReverseMap()` on every entity/DTO pair, so `dto.Id` is copied onto the entity on POST. SQL Server owns primary keys via `IDENTITY(1,1)`, so a client-`id` must never reach the insert. A valid-but-wrong `id` can even silently target the wrong row on update.

**Why it matters:** the API must never trust a client-supplied `Id`. Phase 2 removes that ability at the mapping layer.

---

## 2. Phase 1: Build & Project References Fix (COMPLETED)

**Justification:** The API project had a broken `launchSettings.json`, a duplicate `ProjectReference`, and lived outside `src/Core`, violating the Clean Architecture layout. These blocked every later phase from building.

**Resolution (already merged):**

- `feab62f` — runtime roll-forward + `launchSettings` format fix
- `cb68553` — duplicate project reference removed
- `fa25f71` — `ApartmentManagement.API` moved into `src/Core`

**State:** `dotnet build ApartmentManagementSystem.sln` → 0 errors.

---

## 3. Phase 2: AutoMapper Primary Key Isolation

**Justification:** `ReverseMap()` blindly copies `id` on writes. The fix keeps one DTO per entity but splits the mapping so `Id` is never trusted on input.

### 3.1 Rewrite `src/Core/Application/Mappings/MappingConfig.cs`

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO (GET / reads): Id is included
        CreateMap<User, UserDto>();
        CreateMap<Apartment, ApartmentDto>();
        CreateMap<Tenant, TenantDto>();
        CreateMap<Issue, IssueDto>();
        CreateMap<Parcel, ParcelDto>();
        CreateMap<PaymentRecord, PaymentRecordDto>();
        CreateMap<Role, RoleDto>();

        // DTO → Entity (writes): Id is always ignored — the DB owns it
        CreateMap<UserDto, User>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<ApartmentDto, Apartment>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<TenantDto, Tenant>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<IssueDto, Issue>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<ParcelDto, Parcel>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<PaymentRecordDto, PaymentRecord>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<RoleDto, Role>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
```

### 3.2 Critical Companion Fix: Update services

The DTO→Entity map now ignores `Id`. Update services reuse that map, so the entity would get `Id = 0` and `Update()` would target the wrong row. Re-apply the id after mapping in every `Update*` service:

```csharp
// src/Core/Application/ServiceImpl/TenantServiceImpl.cs
public void UpdateTenant(TenantDto tenantDto)
{
    if (tenantDto == null) throw new ArgumentNullException(nameof(tenantDto));

    var tenantEntity = _mapper.Map<Tenant>(tenantDto);
    tenantEntity.Id = tenantDto.Id;   // restore Id: map ignores it, updates need it
    _tenantRepository.Update(tenantEntity);
}
```

Apply the same pattern to `UpdateApartment`, `UpdateUser`, `UpdateIssue`, `UpdateParcel`, `UpdatePaymentRecord`, `UpdateRole`.

### 3.3 Verification

- POST a tenant with `"id": 555` → HTTP 200, DB stores sequential `Id = 1`.
- PUT `/api/Tenant/1` with `{ "id": 1, ... }` → row 1 is updated correctly (not a new row).

---

## 4. Phase 3: Domain Status Enums & Swagger Dropdowns

**Justification:** status fields (`Apartment.OccupancyStatus`, `PaymentRecord.Status`, `Issue.Status`, `Parcel.Status`) are plain `string`s, so Swagger shows a text box and accepts any value. The real constraints (`CHECK ... IN (...)`) live only in `ApartmentManagmentSchema.sql`; nothing at the API layer prevents `"Resove"` from being sent. Enums make invalid values unrepresentable, and `JsonStringEnumConverter` renders dropdowns.

### 4.1 Create enums in `src/Core/Domain/Enums/`

Values **must** serialize to the exact strings mandated by `context.md` and the SQL schema (`'Pending Pickup'`, `'Picked Up'`, `'In Progress'`):

```csharp
// Domain/Enums/OccupancyStatus.cs
public enum OccupancyStatus { Vacant, Occupied, Maintenance }

// Domain/Enums/PaymentStatus.cs
public enum PaymentStatus { Paid, Partial, Pending }

// Domain/Enums/IssueStatus.cs
using System.Runtime.Serialization;

public enum IssueStatus
{
    Open,
    [EnumMember(Value = "In Progress")] InProgress,
    Resolved
}

// Domain/Enums/ParcelStatus.cs
using System.Runtime.Serialization;

public enum ParcelStatus
{
    [EnumMember(Value = "Pending Pickup")] PendingPickup,
    [EnumMember(Value = "Picked Up")] PickedUp
}
```

### 4.2 Change property types (Entities AND DTOs)

Swagger only renders a dropdown when the **property type** is the enum, so change every `string` status property in the 4 entities (`Apartment.cs`, `PaymentRecord.cs`, `Issue.cs`, `Parcel.cs`) **and** their 4 DTOs.

### 4.3 Store as strings to match the schema

```csharp
// src/Core/Infrastructure/Data/AppDbContext.cs — OnModelCreating
modelBuilder.Entity<Apartment>(entity =>
{
    entity.Property(e => e.OccupancyStatus).HasConversion<string>();
});
// + PaymentRecord.Status, Issue.Status, Parcel.Status ... HasConversion<string>()
```

Generate and apply a migration (a migration already exists, so this is new, not initial):

```bash
dotnet ef migrations add AddStatusEnums
dotnet ef database update
```

### 4.4 Enable string enum JSON in `Program.cs`

```csharp
services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
```

### 4.5 Verification

Swagger shows `OccupancyStatus` and `Status` as dropdowns; posting `"status": "InProgress"` still writes `'In Progress'` to the DB.

---

## 5. Phase 4: DTO DataAnnotations & API Input Validation

**Justification:** DTOs have no validation. Swagger submits defaults (`0` for month, amount, IDs), which violate FK and CHECK constraints as raw 500s.

### 5.1 Rules

- Use `[Range]` / `[StringLength]` — they do the real work.
- **Drop `[Required]` on non-nullable value types** (`int`, `decimal`, `bool`): they can never be null, always default to `0`, and `[Required]` accepts `0`. Only the `[Range]` attributes catch the bad defaults.
- Remember `[ApiController]` (already present on controllers) auto-returns **HTTP 400** with a default problem payload when `ModelState` is invalid — the shape differs from the controllers' `{ message, success }` format. Acceptable; note it in tests.

### 5.2 Example — `src/Core/Application/DTOs/PaymentRecordDto.cs`

```csharp
public class PaymentRecordDto
{
    public int Id { get; set; }   // present for reads; ignored on writes (Phase 2)

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Tenant.")]
    public int TenantId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Apartment.")]
    public int ApartmentId { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "Amount paid must be greater than 0.00.")]
    public decimal AmountPaid { get; set; }

    [Range(1, 12, ErrorMessage = "Payment period month must be between 1 and 12.")]
    public int PaymentPeriodMonth { get; set; }

    [Range(2000, 2100, ErrorMessage = "Payment period year must be a valid 4-digit year.")]
    public int PaymentPeriodYear { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
}
```

### 5.3 Apply to all 7 DTOs

- `TenantDto` — `[StringLength]` on `FullName`, `PhoneNumber`, `EmergencyContact`.
- `ApartmentDto` — `[StringLength]` on `UnitNumber`; `[Range]` on `FloorNumber`, `NumberOfRooms`, `MonthlyRent`.
- `UserDto` — `[Range]` on `RoleId`; `[StringLength]` on `Username`, `Email`.
- `IssueDto`, `ParcelDto`, `RoleDto` — analogous ranges/lengths.

### 5.4 Verification

POST a payment with `"paymentPeriodMonth": 0` → HTTP 400 (not a 500).

---

## 6. Phase 5: Service-Layer FK Sanitization & Business Rules

**Justification:** Swagger populates nullable FKs with `0` (`"currentTenantId": 0`, `"tenantId": 0`). A `0` is a real FK path that violates FK/unique constraints, where the intent is always "no value" → `null`.

### 6.1 Sanitize on Create AND Update

```csharp
// src/Core/Application/ServiceImpl/ApartmentServiceImpl.cs
public void CreateApartment(ApartmentDto apartmentDto)
{
    if (apartmentDto == null) throw new ArgumentNullException(nameof(apartmentDto));

    // 0 or negative integer means "Vacant / No Tenant" → null
    if (apartmentDto.CurrentTenantId.HasValue && apartmentDto.CurrentTenantId.Value <= 0)
    {
        apartmentDto.CurrentTenantId = null;
    }

    // Auto-sync occupancy status with tenant assignment (enum comparisons)
    if (apartmentDto.CurrentTenantId.HasValue)
        apartmentDto.OccupancyStatus = OccupancyStatus.Occupied;
    else if (apartmentDto.OccupancyStatus != OccupancyStatus.Maintenance)
        apartmentDto.OccupancyStatus = OccupancyStatus.Vacant;

    var entity = _mapper.Map<Apartment>(apartmentDto);
    _apartmentRepository.Add(entity);
}
```

`UpdateApartment` applies the same sanitization **before** mapping, then sets `entity.Id = apartmentDto.Id` (Phase 2 rule) and calls `Update(entity)`.

Same pattern for `CreateUser` / `UpdateUser` (`TenantId <= 0 → null`).

### 6.2 Verification

POST an apartment with `"currentTenantId": 0` → HTTP 200, row stores `NULL`.

---

## 7. Phase 6: Apartment FK / Occupancy Consistency (Service-Only)

**Justification:** a vacant apartment must have no tenant id (`CurrentTenantId = NULL`) — the FK is already nullable and configured with `DeleteBehavior.SetNull`, so a vacant unit already clears its FK on tenant deletion. But nothing *guarantees* the status/FK pair stays coherent: code can produce `Vacant + tenantId`, `Occupied + no tenant`, or assign one tenant to two units. This phase enforces the invariant **in the service layer only** — no DB changes, schema stays as `context.md` mandates.

**Scope decision:** service-only. A database CHECK/unique-index/trigger would be the stronger "final safety net," but adds a migration and schema churn. For this local-first app behind a single API, enforcing the rule where every write already passes (the services) is sufficient. Caveat: writes that bypass the service layer could reintroduce inconsistency.

### 7.1 Shared sanitize/sync helper

```csharp
// src/Core/Application/ServiceImpl/ApartmentServiceImpl.cs
private void SanitizeOccupancy(ApartmentDto dto)
{
    if (dto.CurrentTenantId.HasValue && dto.CurrentTenantId.Value <= 0)
        dto.CurrentTenantId = null;                     // "0" = vacant, not a real FK

    if (dto.CurrentTenantId.HasValue)
        dto.OccupancyStatus = OccupancyStatus.Occupied;
    else if (dto.OccupancyStatus != OccupancyStatus.Maintenance)
        dto.OccupancyStatus = OccupancyStatus.Vacant;
}
```

### 7.2 Enforced rules

- **CreateApartment:** call `SanitizeOccupancy`, map, add.
- **UpdateApartment:** load `existing = _apartmentRepository.GetById(dto.Id)`. **Reject** if the unit is currently `Occupied` and `dto.CurrentTenantId` is a *different* tenant — this is the `context.md` rule *"tenants cannot be assigned to non-vacant units."* Then call `SanitizeOccupancy`, set `entity.Id = dto.Id`, update.
- **TenantServiceImpl.DeleteTenant:** EF `SetNull` already clears `CurrentTenantId`; the service additionally flips the affected apartment `OccupancyStatus` to `Vacant`:
  - inject `IApartmentRepository` into `TenantServiceImpl` (registered in `Program.cs`),
  - after `_tenantRepository.Delete(id)`, `GetAll()` and `Update(...)` any apartment whose `CurrentTenantId == id` with `OccupancyStatus = Vacant`.

### 7.3 Business-rule rejections return 400, not 500

The new "already occupied" guard throws a domain error that today would land in `HandleError` → 500. Add a small branch so these read as client errors (e.g., catch `InvalidOperationException` → `BadRequest` in the apartment controller):

```csharp
catch (InvalidOperationException ex)
{
    Logger.LogWarning(ex.Message);
    return BadRequest(new { message = ex.Message, success = false });
}
```

### 7.4 Behavior after fix

| Scenario | Result |
| :--- | :--- |
| POST apartment `{vacant, currentTenantId: 0}` | 200, stored `NULL` |
| POST apartment `{vacant, currentTenantId: 5}` | Auto-corrected → `Occupied` |
| Update occupied unit → different tenant | Rejected (HTTP 400) |
| Delete tenant → its apartments | FK auto-null (`SetNull`) + status → `Vacant` |

### 7.5 Verification

- `dotnet build` → 0 errors
- Manual Swagger: the 4 scenarios above

---

## 8. Swagger Posting Sequence

Apply in dependency order (matches the FK graph in `AppDbContext` + seeded roles):

```
1. Roles          (POST /api/Role)          [Pre-seeded: 1=Owner, 2=Manager, 3=Tenant]
2. Tenants        (POST /api/Tenant)
3. Apartments     (POST /api/Apartment)     [CurrentTenantId: null = Vacant]
4. Users          (POST /api/User)          [RoleId: 1..3, optional TenantId]
5. PaymentRecords (POST /api/PaymentRecord) [TenantId & ApartmentId]
6. Issues         (POST /api/Issue)         [ApartmentId & TenantId]
7. Parcels        (POST /api/Parcel)        [TenantId]
```

---

## 9. Verification & Testing Plan

### Automated

```bash
dotnet build ApartmentManagementSystem.sln          # 0 errors expected
dotnet ef migrations add AddStatusEnums            # Phase 3, once
dotnet ef database update                          # Phase 3, once
```

### Manual Swagger tests

1. **ID isolation:** POST a tenant with `"id": 555` → saves with a sequential id.
2. **Update row:** PUT `/api/Tenant/1` ✓ edits row 1 (not row 0 / a new row).
3. **Dropdowns:** status fields render as select boxes; submitted values stay valid against the DB.
4. **FK sanitization:** apartment with `"currentTenantId": 0` → `NULL`.
5. **Occupancy guard:** assigning a different tenant to an occupied unit → HTTP 400.
6. **Validation:** payment with `"paymentPeriodMonth": 0` → HTTP 400.

---

## 10. Per-Phase Justification Summary

| Phase | Defect (confirmed in code) | Why fix it | Effort |
| :--- | :--- | :--- | :--- |
| 1 (done) | API couldn't build reliably | Blocked all later phases | already done |
| 2 | `.ReverseMap()` copies `id` from POST → `IDENTITY_INSERT` crash | API must never trust client `id`; also fixes wrong-row updates | Small |
| 3 | Statuses are free-text `string`s; DB CHECK limits bypassed | Make allowed values unrepresentable; Swagger dropdowns | Medium (+migration) |
| 4 | No input validation; Swagger `0`s → raw SQL 500s | Clear 400s before the DB layer | Medium |
| 5 | `0` nullable FKs violate FK/unique constraints | Correctly map "no value" to `NULL` | Small |
| 6 | Occupancy status ↔ FK can drift out of sync (vacant-with-tenant-id, occupied-without, one tenant in two units) | Enforce the business invariant at the service layer only | Small |