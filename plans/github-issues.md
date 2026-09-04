# GitHub Issues

Copy and paste each issue below into GitHub.

---

## Issue 1

### Title

Add Field Validation on All API Endpoints Using Data Annotations

### Labels

`enhancement`

### Description

**Is your feature request related to a problem? Please describe.**
Currently all API endpoints accept any input without validation. Invalid data (missing fields, wrong formats, out-of-range values) can reach the service and database layers, causing runtime errors or data integrity issues.

**Describe the solution you'd like**
Add `System.ComponentModel.DataAnnotations` attributes to all 7 DTOs. Since the API already uses `[ApiController]` on `BaseController`, ASP.NET will automatically validate model state and return `400 Bad Request` with field-level error messages before any controller code runs.

**Validation Rules**

| DTO | Field | Rules |
|---|---|---|
| UserDto | `Username` | Required, 3-50 chars, alphanumeric only |
| UserDto | `Email` | Required, valid email format |
| UserDto | `RoleId` | Must be > 0 |
| UserDto | `TenantId` | Must be > 0 if provided |
| TenantDto | `FullName` | Required, 1-100 chars |
| TenantDto | `PhoneNumber` | Required, must start with `7`, exactly 9 digits |
| TenantDto | `EmergencyContact` | Must match phone format if provided |
| ApartmentDto | `UnitNumber` | Required, 1-20 chars |
| ApartmentDto | `FloorNumber` | Must be >= 0 |
| ApartmentDto | `NumberOfRooms` | Must be >= 1 |
| ApartmentDto | `MonthlyRent` | Must be > 0 |
| ApartmentDto | `OccupancyStatus` | Must be "Vacant", "Occupied", or "Maintenance" |
| ApartmentDto | `CurrentTenantId` | Must be > 0 if provided |
| PaymentRecordDto | `TenantId` | Must be > 0 |
| PaymentRecordDto | `ApartmentId` | Must be > 0 |
| PaymentRecordDto | `AmountPaid` | Must be >= 0 |
| PaymentRecordDto | `PaymentPeriodMonth` | Must be 1-12 |
| PaymentRecordDto | `PaymentPeriodYear` | Must be >= 2000 |
| PaymentRecordDto | `Status` | Must be "Paid", "Partial", or "Pending" |
| IssueDto | `ApartmentId` | Must be > 0 |
| IssueDto | `TenantId` | Must be > 0 |
| IssueDto | `Description` | Required, 1-500 chars |
| IssueDto | `Status` | Must be "Open", "In Progress", or "Resolved" |
| ParcelDto | `TenantId` | Must be > 0 |
| ParcelDto | `Status` | Must be "Pending Pickup" or "Picked Up" |
| RoleDto | `RoleName` | Required, 1-50 chars |

**Additional context**
- Full plan: `plans/field-validation.md`
- No new NuGet packages required (Data Annotations are built into .NET)
- Create a shared `AllowedValuesAttribute` custom validation attribute for enum-like string fields
- Remove redundant null-checks from controllers after validation is in place

---

## Issue 2

### Title

Add Automated Unit Tests for Application Service Layer

### Labels

`enhancement`, `testing`

### Description**

**Is your feature request related to a problem? Please describe.**
The project currently has zero tests. There is no way to verify that service logic, AutoMapper mappings, or repository delegation work correctly. regressions can be introduced silently when making changes.

**Describe the solution you'd like**
Add a test project using **xUnit** + **Moq** that unit-tests all 7 service implementations. Tests will mock repository interfaces and use the real AutoMapper `MappingProfile` to validate the full DTO-entity mapping pipeline.

**What each test verifies:**
1. Repository methods are called correctly (Add/Update/Delete/GetById/GetAll)
2. Null input throws `ArgumentNullException`
3. Return values are correct (mapped DTOs or null for not-found)
4. AutoMapper mappings produce correct results (entity <-> DTO round-trip)
5. `RoleServiceImpl` special logic: default role seeding behavior

**Services to test (8 files):**
- `UserServiceImpl` (8 tests)
- `ApartmentServiceImpl` (8 tests)
- `TenantServiceImpl` (8 tests)
- `PaymentRecordServiceImpl` (8 tests)
- `IssueServiceImpl` (8 tests)
- `ParcelServiceImpl` (8 tests)
- `RoleServiceImpl` (9 tests, includes seed logic tests)

**New project structure:**
```
src/Tests/
├── Tests.csproj
├── Fixtures/
│   └── TestFixtures.cs          # Shared test data + IMapper setup
└── Services/
    ├── UserServiceImplTests.cs
    ├── ApartmentServiceImplTests.cs
    ├── TenantServiceImplTests.cs
    ├── PaymentRecordServiceImplTests.cs
    ├── IssueServiceImplTests.cs
    ├── ParcelServiceImplTests.cs
    └── RoleServiceImplTests.cs
```

**Test project dependencies:**
- `xunit` 2.4.1
- `xunit.runner.visualstudio` 2.4.3
- `Microsoft.NET.Test.Sdk` 16.11.0
- `Moq` 4.16.1
- `AutoMapper` 10.1.1

**Additional context**
- Full plan: `plans/automated-tests.md`
- Run tests with: `dotnet test src/Tests/`
- ~60 test cases total across 7 service test files
- No database required - all tests use mocked repositories
