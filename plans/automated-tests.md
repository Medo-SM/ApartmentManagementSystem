# Automated Tests Plan

## Strategy

Test the **Application layer** (7 service implementations) using **xUnit** + **Moq**. Each service follows the same CRUD pattern, so the test structure is consistent.

### What to verify per service:

1. **Repository is called correctly** (Add/Update/Delete/GetById/GetAll)
2. **Null input throws `ArgumentNullException`**
3. **Return values are correct** (mapped DTOs or null)
4. **AutoMapper mappings work** (entity <-> DTO round-trip)
5. **RoleServiceImpl special logic** (default role seeding)

---

## Step 1: Create the Test Project

**New directory:** `src/Tests/`

**New file:** `src/Tests/Tests.csproj`

- Target `net5.0` (matches main project)
- NuGet packages:
  - `xunit` (2.4.1) - test framework
  - `xunit.runner.visualstudio` (2.4.3) - test runner
  - `Microsoft.NET.Test.Sdk` (16.11.0) - MS Test integration
  - `Moq` (4.16.1) - mocking library
  - `AutoMapper` (10.1.1) - same version as main project, for testing mappings
- Project references:
  - `Application` (for services + DTOs)
  - `Domain` (for entities + repository interfaces)

**Modify:** `ApartmentManagementSystem.sln` - add the new test project to the solution.

---

## Step 2: Create Shared Test Fixtures

**New file:** `src/Tests/Fixtures/TestFixtures.cs`

A static helper class that provides:

- Pre-built valid DTOs for each entity (UserDto, TenantDto, ApartmentDto, etc.)
- Pre-built entity objects for testing AutoMapper
- A configured `IMapper` instance using the real `MappingProfile`

This avoids duplicating test data across every test file.

---

## Step 3: Create 7 Test Files (one per service)

Each test file follows the same structure. Here is the test plan per service:

### UserServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreateUser_ValidDto_CallsRepositoryAdd` | Mapper maps DTO to entity, `Add()` is called on mock |
| `CreateUser_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` when null |
| `GetUserById_ExistingId_ReturnsDto` | Returns mapped DTO when entity found |
| `GetUserById_NonExistingId_ReturnsNull` | Returns null when entity not found |
| `GetAllUsers_ReturnsAllDtos` | Returns mapped list of DTOs |
| `UpdateUser_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()`, sets Id correctly |
| `UpdateUser_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteUser_ValidId_CallsRepositoryDelete` | Calls `Delete()` with correct id |

### ApartmentServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreateApartment_ValidDto_CallsRepositoryAdd` | Mapper maps DTO to entity, `Add()` is called |
| `CreateApartment_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetApartmentById_ExistingId_ReturnsDto` | Returns mapped DTO when entity found |
| `GetApartmentById_NonExistingId_ReturnsNull` | Returns null when entity not found |
| `GetAllApartments_ReturnsAllDtos` | Returns mapped list |
| `UpdateApartment_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdateApartment_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteApartment_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

### TenantServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreateTenant_ValidDto_CallsRepositoryAdd` | Maps and calls `Add()` |
| `CreateTenant_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetTenantById_ExistingId_ReturnsDto` | Returns mapped DTO |
| `GetTenantById_NonExistingId_ReturnsNull` | Returns null |
| `GetAllTenants_ReturnsAllDtos` | Returns mapped list |
| `UpdateTenant_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdateTenant_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteTenant_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

### PaymentRecordServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreatePaymentRecord_ValidDto_CallsRepositoryAdd` | Maps and calls `Add()` |
| `CreatePaymentRecord_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetPaymentRecordById_ExistingId_ReturnsDto` | Returns mapped DTO |
| `GetPaymentRecordById_NonExistingId_ReturnsNull` | Returns null |
| `GetAllPaymentRecords_ReturnsAllDtos` | Returns mapped list |
| `UpdatePaymentRecord_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdatePaymentRecord_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeletePaymentRecord_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

### IssueServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreateIssue_ValidDto_CallsRepositoryAdd` | Maps and calls `Add()` |
| `CreateIssue_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetIssueById_ExistingId_ReturnsDto` | Returns mapped DTO |
| `GetIssueById_NonExistingId_ReturnsNull` | Returns null |
| `GetAllIssues_ReturnsAllDtos` | Returns mapped list |
| `UpdateIssue_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdateIssue_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteIssue_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

### ParcelServiceImplTests.cs

| Test | What it verifies |
|---|---|
| `CreateParcel_ValidDto_CallsRepositoryAdd` | Maps and calls `Add()` |
| `CreateParcel_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetParcelById_ExistingId_ReturnsDto` | Returns mapped DTO |
| `GetParcelById_NonExistingId_ReturnsNull` | Returns null |
| `GetAllParcels_ReturnsAllDtos` | Returns mapped list |
| `UpdateParcel_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdateParcel_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteParcel_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

### RoleServiceImplTests.cs (special logic)

| Test | What it verifies |
|---|---|
| `CreateRole_ValidDto_SeedsMissingDefaults` | When some defaults exist, only missing ones are added |
| `CreateRole_AllDefaultsExist_OnlyAddsRequestedRole` | When all defaults exist, only the requested role is added |
| `CreateRole_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `GetRoleById_ExistingId_ReturnsDto` | Returns mapped DTO |
| `GetRoleById_NonExistingId_ReturnsNull` | Returns null |
| `GetAllRoles_ReturnsAllDtos` | Returns mapped list |
| `UpdateRole_ValidDto_CallsRepositoryUpdate` | Maps and calls `Update()` |
| `UpdateRole_NullDto_ThrowsArgumentNullException` | Throws `ArgumentNullException` |
| `DeleteRole_ValidId_CallsRepositoryDelete` | Calls `Delete()` |

---

## Step 4: Test Pattern Examples

### Happy Path (Create)

```csharp
[Fact]
public void CreateUser_ValidDto_CallsRepositoryAdd()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var mapper = TestFixtures.CreateMapper();
    var service = new UserServiceImpl(mockRepo.Object, mapper);
    var dto = TestFixtures.CreateValidUserDto();

    // Act
    service.CreateUser(dto);

    // Assert
    mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
}
```

### Null Check

```csharp
[Fact]
public void CreateUser_NullDto_ThrowsArgumentNullException()
{
    var mockRepo = new Mock<IUserRepository>();
    var mapper = TestFixtures.CreateMapper();
    var service = new UserServiceImpl(mockRepo.Object, mapper);

    Assert.Throws<ArgumentNullException>(() => service.CreateUser(null));
}
```

### GetById (Found)

```csharp
[Fact]
public void GetUserById_ExistingId_ReturnsDto()
{
    var mockRepo = new Mock<IUserRepository>();
    var mapper = TestFixtures.CreateMapper();
    var entity = TestFixtures.CreateValidUserEntity();
    mockRepo.Setup(r => r.GetById(1)).Returns(entity);
    var service = new UserServiceImpl(mockRepo.Object, mapper);

    var result = service.GetUserById(1);

    Assert.NotNull(result);
    Assert.Equal(entity.Username, result.Username);
}
```

### GetById (Not Found)

```csharp
[Fact]
public void GetUserById_NonExistingId_ReturnsNull()
{
    var mockRepo = new Mock<IUserRepository>();
    var mapper = TestFixtures.CreateMapper();
    mockRepo.Setup(r => r.GetById(999)).Returns((User?)null);
    var service = new UserServiceImpl(mockRepo.Object, mapper);

    var result = service.GetUserById(999);

    Assert.Null(result);
}
```

---

## Step 5: Verify

```bash
dotnet test src/Tests/
```

---

## Files Summary

| Action | File |
|---|---|
| **New** | `src/Tests/Tests.csproj` |
| **New** | `src/Tests/Fixtures/TestFixtures.cs` |
| **New** | `src/Tests/Services/UserServiceImplTests.cs` |
| **New** | `src/Tests/Services/ApartmentServiceImplTests.cs` |
| **New** | `src/Tests/Services/TenantServiceImplTests.cs` |
| **New** | `src/Tests/Services/PaymentRecordServiceImplTests.cs` |
| **New** | `src/Tests/Services/IssueServiceImplTests.cs` |
| **New** | `src/Tests/Services/ParcelServiceImplTests.cs` |
| **New** | `src/Tests/Services/RoleServiceImplTests.cs` |
| **Modify** | `ApartmentManagementSystem.sln` (add test project) |

**Total: 9 new files, 1 modified file, ~60 test cases**

---

## Why This Is Effective

1. **Tests the right layer** - service logic is where business rules live
2. **No database needed** - Moq replaces all repositories, tests run in milliseconds
3. **Real AutoMapper** - validates actual DTO-entity mapping, not mocked
4. **Consistent pattern** - every service file follows the same test structure, easy to extend
5. **CI-friendly** - `dotnet test` works on any machine, no external dependencies
