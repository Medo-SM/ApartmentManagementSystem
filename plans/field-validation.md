# Field Validation Plan

## Approach

Use **Data Annotation attributes** on DTOs. The `[ApiController]` attribute on `BaseController` causes ASP.NET to **automatically validate model state** and return `400 Bad Request` with detailed error messages before controller code runs.

- **Zero manual validation code** in controllers or services
- **Zero new NuGet packages** (built into .NET)
- Validation errors automatically return structured `400` responses
- Swagger UI will show validation rules in API docs

---

## Validation Rules

### UserDto

| Field | Rules |
|---|---|
| `Username` | `[Required]`, `[StringLength(50, MinimumLength = 3)]`, `[RegularExpression("^[a-zA-Z0-9]+$")]` |
| `Email` | `[Required]`, `[EmailAddress]` |
| `RoleId` | `[Range(1, int.MaxValue)]` |
| `TenantId` | `[Range(1, int.MaxValue)]` (optional, but must be > 0 if provided) |

### TenantDto

| Field | Rules |
|---|---|
| `FullName` | `[Required]`, `[StringLength(100, MinimumLength = 1)]` |
| `PhoneNumber` | `[Required]`, `[RegularExpression("^7\\d{8}$")]` |
| `EmergencyContact` | `[RegularExpression("^7\\d{8}$")]` (optional, but must match format if provided) |

### ApartmentDto

| Field | Rules |
|---|---|
| `UnitNumber` | `[Required]`, `[StringLength(20, MinimumLength = 1)]` |
| `FloorNumber` | `[Range(0, int.MaxValue)]` |
| `NumberOfRooms` | `[Range(1, int.MaxValue)]` |
| `MonthlyRent` | `[Range(0.01, double.MaxValue)]` |
| `OccupancyStatus` | `[Required]` + custom validation (must be "Vacant"/"Occupied"/"Maintenance") |
| `CurrentTenantId` | `[Range(1, int.MaxValue)]` (optional, but must be > 0 if provided) |

### PaymentRecordDto

| Field | Rules |
|---|---|
| `TenantId` | `[Range(1, int.MaxValue)]` |
| `ApartmentId` | `[Range(1, int.MaxValue)]` |
| `AmountPaid` | `[Range(0, double.MaxValue)]` |
| `PaymentPeriodMonth` | `[Range(1, 12)]` |
| `PaymentPeriodYear` | `[Range(2000, int.MaxValue)]` |
| `Status` | `[Required]` + custom validation (must be "Paid"/"Partial"/"Pending") |

### IssueDto

| Field | Rules |
|---|---|
| `ApartmentId` | `[Range(1, int.MaxValue)]` |
| `TenantId` | `[Range(1, int.MaxValue)]` |
| `Description` | `[Required]`, `[StringLength(500, MinimumLength = 1)]` |
| `Status` | `[Required]` + custom validation (must be "Open"/"In Progress"/"Resolved") |

### ParcelDto

| Field | Rules |
|---|---|
| `TenantId` | `[Range(1, int.MaxValue)]` |
| `Status` | `[Required]` + custom validation (must be "Pending Pickup"/"Picked Up") |

### RoleDto

| Field | Rules |
|---|---|
| `RoleName` | `[Required]`, `[StringLength(50, MinimumLength = 1)]` |

---

## Implementation Steps

### Step 1: Add Data Annotations to all 7 DTO files

Add `System.ComponentModel.DataAnnotations` attributes to each DTO property.

**Example - UserDto:**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Username must contain only letters and numbers.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0.")]
        public int RoleId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TenantId must be greater than 0.")]
        public int? TenantId { get; set; }

        public bool IsActive { get; set; }
    }
}
```

Same pattern for all 7 DTOs.

### Step 2: Add custom validation for enum-like string fields

For fields like `OccupancyStatus`, `Status` (on PaymentRecord, Issue, Parcel) that accept specific string values, create a shared custom `ValidationAttribute`.

**New file:** `src/Core/Application/Validators/AllowedValuesAttribute.cs`

A simple custom attribute inheriting from `ValidationAttribute` that checks if the value is in a list of allowed strings. Usage example:

```csharp
[AllowedValues("Vacant", "Occupied", "Maintenance")]
public string OccupancyStatus { get; set; } = "Vacant";
```

### Step 3: Remove redundant null-checks from controllers

Since `[ApiController]` + `ModelState` auto-validates, the manual `if (dto == null)` checks in controllers become redundant (the framework returns 400 before reaching that code). Remove them to clean up the code. The `GetAll`, `GetById`, and `Delete` endpoints don't take DTOs so they stay unchanged.

---

## Files to Modify

| File | Change |
|---|---|
| `src/Core/Application/DTOs/UserDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/TenantDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/ApartmentDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/PaymentRecordDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/IssueDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/ParcelDto.cs` | Add Data Annotation attributes |
| `src/Core/Application/DTOs/RoleDto.cs` | Add Data Annotation attributes |
| **New:** `src/Core/Application/Validators/AllowedValuesAttribute.cs` | Custom validation attribute for enum-like strings |
| All 7 controller files | Remove redundant null-checks in Create/Update methods |

---

## Example 400 Response (automatic)

When validation fails, ASP.NET automatically returns:

```json
{
  "errors": {
    "Email": ["Invalid email address."],
    "PhoneNumber": ["The field PhoneNumber must match the regular expression '^7\\d{8}$'."]
  },
  "status": 400,
  "title": "One or more validation errors occurred."
}
```

---

## Why This Is the Easiest Way

1. **No new packages** - `System.ComponentModel.DataAnnotations` is built into .NET
2. **No manual checks** - the `[ApiController]` attribute already handles it
3. **No changes to services/repositories** - validation happens at the API boundary
4. **Automatic error responses** - structured `400` with field-level error messages
5. **Swagger integration** - validation attributes show up in Swagger UI docs
6. **1 new file** (the custom `AllowedValuesAttribute`) + edits to existing files
