# Clear Error Messages Plan

## Problem

Every controller catches exceptions and delegates to `BaseController.HandleError`, which always returns **HTTP 500** with the raw exception message:

```json
{
  "message": "Failed to create apartment.",
  "success": false,
  "error": "Cannot insert duplicate key row in object 'dbo.Apartments' with unique index 'IX_Apartments_UnitNumber'. The duplicate key value is (A101)."
}
```

**Confirmed defects:**

1. **Duplicate unique fields produce a cryptic SQL message.** When a `POST` (or `PUT`) hits one of the 5 unique indexes, the SQL Server message is surfaced verbatim:
   - `IX_Apartments_UnitNumber` (Apartment.UnitNumber)
   - `IX_Users_Username` (User.Username)
   - `IX_Users_Email` (User.Email)
   - `IX_Users_TenantId` (User.TenantId — 1:1 user/tenant link)
   - `IX_Roles_RoleName` (Role.RoleName)
2. **Wrong status code.** Duplicates are reported as `500` instead of `409 Conflict`.
3. **FK violations are equally opaque.** Posting a `PaymentRecord`/`Issue`/`Apartment`/`User` with a non-existent `TenantId`/`ApartmentId`/`RoleId` returns a raw `547` SQL message as a 500.

When a request fails in Swagger, the response body and status should be **clear and actionable**, e.g.:

```json
{
  "message": "An apartment with this Unit Number already exists.",
  "success": false
}
```

with status **409**.

---

## Approach

Centralize error mapping in **one place**: a pure, message-based mapper (`ApiErrorMapper`) + a small change to `BaseController.HandleError`. Because all 7 controllers inherit `BaseController`, **all 35 endpoints** (POST and PUT) get the fix with zero per-controller changes.

### Why message-based (not `SqlException` type-based)

- The mapper lives in the **Application** layer, which does NOT reference the SQL Server provider, so `SqlException` is not directly available. It also makes construction in unit tests near-impossible.
- The meaningful info (index name, FK constraint, error text) is already in `GetBaseException().Message`.
- The mapper reads the SQL Server error text and is therefore fully unit-testable by simulating messages with plain `Exception` instances.

> **Caveat:** SQL Server error text is English by default. The mapper matches English keywords ("duplicate key", "Cannot insert", "FOREIGN KEY", "unique index", "unique constraint"). If the DB collation/provider changes, the regexes in `ApiErrorMapper` must be revisited.

---

## Implementation Steps

### Step 1: New file `src/Core/Application/ErrorHandling/ApiErrorMapper.cs`

Static class with one entry point:

```csharp
namespace Application.ErrorHandling
{
    public static class ApiErrorMapper
    {
        public static ErrorMapping? TryMap(Exception ex);
    }

    public sealed class ErrorMapping
    {
        public int StatusCode { get; init; }
        public string Message { get; init; }
    }
}
```

Behavior of `TryMap`:

| Exception (base message) | Detected | Status | Message |
|---|---|---|---|
| `Cannot insert duplicate key row ... unique index 'IX_Apartments_UnitNumber' ...`<br>`Cannot insert duplicate key ... unique constraint '...'`<br>single-row UPDATE duplicate | SQL 2601 / 2627 subtext | **409** | entity/field-specific (table below), else generic |
| `The INSERT statement conflicted with the FOREIGN KEY constraint ...` | SQL 547 subtext | **400** | "One or more referenced records do not exist. Please check the supplied Tenant/Apartment/Role IDs." |
| anything else | n/a | `null` (caller keeps 500) | n/a |

**Entity/field message table** (from the EF index name `IX_<Entity>_<Field>`):

| Index name | Message |
|---|---|
| `IX_Apartments_UnitNumber` | "An apartment with this Unit Number already exists." |
| `IX_Users_Username` | "A user with this Username already exists." |
| `IX_Users_Email` | "A user with this Email already exists." |
| `IX_Users_TenantId` | "A user is already linked to this Tenant." |
| `IX_Roles_RoleName` | "A role with this name already exists." |
| unknown `IX_*` | "A record with the same unique value already exists." |

Mapping implementation outline:

- `Normalize` the base-exception message (collapse whitespace) for matching.
- Regex 1 (duplicate): `Cannot insert duplicate key ... unique index '(?<idx>IX_\w+_\w+)'` OR `... unique constraint '(?<idx>\w+)'` with fallback when only the duplicate-key keyword is present.
- Regex 2 (FK): `FOREIGN KEY constraint` OR `The (INSERT|UPDATE|DELETE) statement conflicted with`.
- Unknown → `null`.

### Step 2: Modify `src/Core/ApartmentManagement.API/Controllers/BaseController.cs`

```csharp
protected IActionResult HandleError(Exception ex, string message = "An unexpected error occurred.")
{
    Logger.LogError(ex, message);

    var mapping = ApiErrorMapper.TryMap(ex);
    if (mapping != null)
    {
        return StatusCode(mapping.StatusCode, new
        {
            message = mapping.Message,
            success = false
        });
    }

    return StatusCode(500, new
    {
        message = message,
        success = false,
        error = ex.Message
    });
}
```

- Add `using Application.ErrorHandling;`.
- Unchanged behavior for unexpected errors: still 500 with raw `error` detail (local-first app, keeps debugging easy).
- All controllers keep their current `try/catch` + `HandleError(ex, "...")` calls — no controller edits.

### Step 3: Tests `src/Tests/ErrorHandling/ApiErrorMapperTests.cs`

SQL Server exceptions are constructed via inner `Exception` messages (the mapper only reads `.Message`), so no EF/DB mocking is needed. In some cases `DbUpdateException` wrapping them is used for realism — `Application` references EF Core so `DbUpdateException` is available.

| Test | Assertion |
|---|---|
| `DuplicateApartmentUnitNumber_MapsTo409_WithSpecificMessage` | 409 + "An apartment with this Unit Number already exists." |
| `DuplicateUserUsername_MapsTo409` | 409 + "A user with this Username already exists." |
| `DuplicateUserEmail_MapsTo409` | 409 + "A user with this Email already exists." |
| `DuplicateUserTenantLink_MapsTo409` | 409 + "A user is already linked to this Tenant." |
| `DuplicateRoleName_MapsTo409` | 409 + "A role with this name already exists." |
| `Duplicate_UnknownIndex_MapsTo409_Generic` | 409 + generic unique message |
| `ForeignKeyViolation_MapsTo400` | 400 + referenced-records message |
| `UnrelatedException_ReturnsNull` | `null` (caller keeps 500) |

### Step 4: Verification

```bash
dotnet build ApartmentManagementSystem.sln      # 0 errors
dotnet test src/Tests/                          # all green (existing + new)
```

Manual Swagger checklist:

1. `POST /api/Apartment` twice with the same `unitNumber` → **409** with "An apartment with this Unit Number already exists."
2. `POST /api/Role` with an existing role name → **409** with "A role with this name already exists."
3. `POST /api/User` with an existing `email` → **409** with "A user with this Email already exists."
4. `POST /api/PaymentRecord` with a non-existent `tenantId` → **400** with the referenced-records message.
5. Any other failure still returns the prior **500** shape.

---

## Files Summary

| Action | File |
|---|---|
| **New** | `src/Core/Application/ErrorHandling/ApiErrorMapper.cs` |
| **Modify** | `src/Core/ApartmentManagement.API/Controllers/BaseController.cs` |
| **New** | `src/Tests/ErrorHandling/ApiErrorMapperTests.cs` |
| **New** | `plans/clear-error-messages.md` (this document) |

No changes to: entities, DTOs, services, repositories, migrations, Swagger config, or the other controllers.

---

## Notes & Constraints

- **One shared fix point:** the mapper lives in Application so both `BaseController` (API) and the test project (Tests already references Application) can use it.
- **Coverage:** applies to `POST` and `PUT` for the 5 unique indexes + FK references. `DELETE` has no duplicate/FK-on-write path (FK references from children are `Restrict`, surfaced as 547 → now a clean 400 instead of 500).
- **Not in scope:** the planned status-enums and service-layer FK sanitization remain separate work items.