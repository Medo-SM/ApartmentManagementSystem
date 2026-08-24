# Backend Architecture Explained (A Beginner's Guide)

> **Welcome!** If you only know basic coding syntax (variables, loops, methods, and simple classes) and feel lost looking at terms like *Clean Architecture*, *DTOs*, *Repositories*, *Services*, *DbContext*, and *Dependency Injection*, this guide is designed specifically for you.

---

## 📑 Table of Contents
1. [The Big Picture: Why Do We Need Architecture?](#1-the-big-picture-why-do-we-need-architecture)
2. [The 4 Layers of Our Architecture](#2-the-4-layers-of-our-architecture)
   - [Layer 1: Domain (The Blueprint & Core Rules)](#layer-1-domain-the-blueprint--core-rules)
   - [Layer 2: Application (The Brain & Business Workflows)](#layer-2-application-the-brain--business-workflows)
   - [Layer 3: Infrastructure (The Database Heavy-Lifter)](#layer-3-infrastructure-the-database-heavy-lifter)
   - [Layer 4: Presentation / API (The Front Desk)](#layer-4-presentation--api-the-front-desk)
3. [Core Concepts Explained in Plain English](#3-core-concepts-explained-in-plain-english)
   - [Why do we use Interfaces? (The Wall Socket Analogy)](#why-do-we-use-interfaces-the-wall-socket-analogy)
   - [What is a DTO and why do we use AutoMapper?](#what-is-a-dto-and-why-do-we-use-automapper)
   - [What is Dependency Injection (DI)?](#what-is-dependency-injection-di)
4. [Step-by-Step Walkthrough: Registering a New Tenant](#4-step-by-step-walkthrough-registering-a-new-tenant)
5. [Codebase Map (Where to Find What)](#5-codebase-map-where-to-find-what)
6. [Beginner Cheat Sheet & Glossary](#6-beginner-cheat-sheet--glossary)

---

## 1. The Big Picture: Why Do We Need Architecture?

When you first learn programming, it is natural to write everything in one single file:

```csharp
// The beginner "All-in-One" file
void Main()
{
    // 1. Read input from user
    // 2. Validate data
    // 3. Connect to SQL database
    // 4. Save to table
    // 5. Print success message
}
```

### The Problem: The "Overworked Restaurant Owner" Analogy
Imagine a restaurant where **one single person** is the waiter, the master chef, the dishwasher, the grocery buyer, and the cashier at the same time:
* If the cashier system crashes, nobody can cook.
* If you want to change a recipe in the kitchen, you might accidentally break the billing register.
* It is impossible to test, messy to maintain, and completely falls apart when the project grows.

### The Solution: Clean / Layered Architecture
In a well-run restaurant, duties are strictly separated into **specialized roles**:
1. **The Waiter (API / Controllers):** Greets customers at the front desk, takes orders, and returns food.
2. **The Head Chef (Application Services):** Takes the order, validates recipes, and coordinates preparation.
3. **The Kitchen & Pantry Staff (Infrastructure / Repositories):** Goes into the pantry (database) to store and retrieve ingredients.
4. **The Menu & Ingredients (Domain Entities):** The core definitions of what items and ingredients exist.

---

## 2. The 4 Layers of Our Architecture

Here is how our solution ([`ApartmentManagementSystem.sln`](ApartmentManagementSystem.sln)) is structured:

```
┌────────────────────────────────────────────────────────┐
│ 1. Presentation Layer (ApartmentManagement.API)        │  <-- Receives HTTP requests (Swagger / Web API)
└──────────────────────────┬─────────────────────────────┘
                           │ Calls
┌──────────────────────────▼─────────────────────────────┐
│ 2. Application Layer (src/Core/Application)            │  <-- Business logic, Services, DTOs, AutoMapper
└──────────────────────────┬─────────────────────────────┘
                           │ Calls
┌──────────────────────────▼─────────────────────────────┐
│ 3. Infrastructure Layer (src/Core/Infrastructure)      │  <-- EF Core, AppDbContext, SQL Database queries
└──────────────────────────┬─────────────────────────────┘
                           │ Implements & references
┌──────────────────────────▼─────────────────────────────┐
│ 4. Domain Layer (src/Core/Domain)                      │  <-- Entities (Tenant, Apartment) & Interfaces
└────────────────────────────────────────────────────────┘
```

---

### Layer 1: Domain (`src/Core/Domain`)
> **Summary:** The core foundation. It defines **what** exists in our world, with zero external dependencies.

Domain does not care about databases, Web APIs, or user interfaces. It only contains the pure rules of our business.

#### 1. Entities (The Data Models)
An **Entity** represents a real-world object that will be stored in our database.

Look at [`src/Core/Domain/Entities/Tenant.cs`](src/Core/Domain/Entities/Tenant.cs):
```csharp
public class Tenant : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }

    // Relationships to other entities
    public List<Apartment> Apartments { get; set; } = new List<Apartment>();
    public List<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
    public List<Issue> Issues { get; set; } = new List<Issue>();
    public List<Parcel> Parcels { get; set; } = new List<Parcel>();
}
```
*Notice:* Every entity inherits from [`BaseEntity.cs`](src/Core/Domain/Entities/Base/BaseEntity.cs), giving each record an `Id`, `CreatedAt`, and `UpdatedAt` automatically.

#### 2. Repository Interfaces (The Job Descriptions)
Domain also defines **Interfaces** (contracts) like [`ITenantRepository.cs`](src/Core/Domain/Repository/ITenantRepository.cs):
```csharp
public interface ITenantRepository
{
    void Add(Tenant tenant);
    void Update(Tenant tenant);
    void Delete(int id);
    Tenant? GetById(int id);
    List<Tenant> GetAll();
}
```
An interface does **not** contain code. It is simply a list of required operations. It says: *"Whoever implements me must provide Add, Update, Delete, GetById, and GetAll."*

---

### Layer 2: Application (`src/Core/Application`)
> **Summary:** The Brain. It handles business logic, converts data, and organizes operations.

Application connects the outside world (API) with the inner domain.

#### 1. DTOs (Data Transfer Objects)
Why don't we send raw database entities directly to the API?
- Entities contain private database links (`List<PaymentRecord>`, password hashes, internal IDs).
- Sending an entire entity can cause circular reference crashes and security leaks.

Instead, we use a lightweight **DTO** ([`src/Core/Application/DTOs/TenantDto.cs`](src/Core/Application/DTOs/TenantDto.cs)):
```csharp
public class TenantDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
}
```
It carries **only** the data needed by the user.

#### 2. AutoMapper (`MappingConfig.cs`)
Instead of manually typing:
```csharp
var tenant = new Tenant();
tenant.FullName = tenantDto.FullName;
tenant.PhoneNumber = tenantDto.PhoneNumber;
```
We use **AutoMapper** ([`src/Core/Application/Mappings/MappingConfig.cs`](src/Core/Application/Mappings/MappingConfig.cs)) to copy all matching fields in one line:
```csharp
CreateMap<Tenant, TenantDto>().ReverseMap();
```

#### 3. Services (`TenantServiceImpl.cs`)
The **Service** executes the actual workflow ([`src/Core/Application/ServiceImpl/TenantServiceImpl.cs`](src/Core/Application/ServiceImpl/TenantServiceImpl.cs)):
```csharp
public class TenantServiceImpl : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    // We ask for our helpers via Constructor Injection
    public TenantServiceImpl(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public void CreateTenant(TenantDto tenantDto)
    {
        // 1. Guard check
        if (tenantDto == null) throw new ArgumentNullException(nameof(tenantDto));

        // 2. Convert DTO -> Database Entity
        var tenantEntity = _mapper.Map<Tenant>(tenantDto);

        // 3. Ask Repository to save to database
        _tenantRepository.Add(tenantEntity);
    }
}
```

---

### Layer 3: Infrastructure (`src/Core/Infrastructure`)
> **Summary:** The Database Worker. It writes and reads real records in SQL Server using **Entity Framework Core (EF Core)**.

Infrastructure implements the interfaces defined in Domain.

#### 1. `AppDbContext.cs` (The Database Session)
In [`src/Core/Infrastructure/Data/AppDbContext.cs`](src/Core/Infrastructure/Data/AppDbContext.cs), EF Core links C# classes to SQL tables:
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<PaymentRecord> PaymentRecords { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Parcel> Parcels { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
}
```
Each `DbSet<T>` corresponds to a table in [`ApartmentManagmentSchema.sql`](ApartmentManagmentSchema.sql).

#### 2. Repository Implementations (`TenantRepository.cs`)
This class writes the actual EF Core code to talk to SQL ([`src/Core/Infrastructure/Repositories/TenantRepository.cs`](src/Core/Infrastructure/Repositories/TenantRepository.cs)):
```csharp
public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Tenant tenant)
    {
        _context.Tenants.Add(tenant); // Prepares the INSERT command
        _context.SaveChanges();      // Executes SQL in the database
    }

    public Tenant? GetById(int id)
    {
        return _context.Tenants.Find(id); // Runs SELECT WHERE Id = id
    }
}
```

---

### Layer 4: Presentation / API (`ApartmentManagement.API`)
> **Summary:** The Receptionist. It exposes HTTP REST endpoints that outside clients (web pages, mobile apps, desktop UIs, Swagger) can talk to.

* **Controllers:** Listen for incoming network requests (such as `GET /api/tenants` or `POST /api/tenants`).
* **Swagger UI:** A built-in web page (`/swagger`) that allows you to click buttons and test your API right in your browser.

---

## 3. Core Concepts Explained in Plain English

### Why do we use Interfaces? (The Wall Socket Analogy)
Think of an electrical wall outlet in your home:
* The wall socket is an **Interface** (`ITenantRepository`). It specifies the shape of the plug and voltage.
* The device plugged in is the **Implementation** (`TenantRepository`). It could be a lamp, a fan, or a phone charger.

Because your house is built with standard socket interfaces, you can replace a lamp with a fan without breaking your walls.

Similarly, in our code:
`TenantServiceImpl` only knows `ITenantRepository`. If tomorrow we decide to switch from Microsoft SQL Server to SQLite, PostgreSQL, or an in-memory testing database, we only write a new repository class. **We do not have to rewrite any business logic!**

---

### What is a DTO and why do we use AutoMapper?

| Database Entity (`Tenant.cs`) | Data Transfer Object (`TenantDto.cs`) |
| :--- | :--- |
| Represents the raw database table. | Represents data sent to or received from the API. |
| Includes table foreign keys, complex relations, and internal fields. | Contains only the clean, safe fields the user needs. |
| Stays strictly inside backend layers (`Domain` / `Infrastructure`). | Safely passed across the network (`Application` / `API`). |

**AutoMapper** automatically copies matching properties between `Tenant` and `TenantDto` so you don't have to write 20 lines of manual assignment code for every request.

---

### What is Dependency Injection (DI)?

Without Dependency Injection (tightly coupled):
```csharp
// Bad practice: creating dependencies directly
public class TenantServiceImpl
{
    private TenantRepository _repo = new TenantRepository(); // Glued forever!
}
```

With Dependency Injection (clean & decoupled):
```csharp
// Good practice: asking for the dependency via constructor
public class TenantServiceImpl
{
    private readonly ITenantRepository _repo;

    public TenantServiceImpl(ITenantRepository repo)
    {
        _repo = repo;
    }
}
```
In `Startup.cs`, .NET is told once: *"Whenever any class asks for `ITenantRepository`, create and supply a `TenantRepository`."* .NET takes care of creating and passing it automatically.

---

## 4. Step-by-Step Walkthrough: Registering a New Tenant

Let's trace the complete lifecycle of adding a new tenant:

```
[1. User / Swagger UI]
         │ Sends HTTP POST with JSON: { "fullName": "Ahmad Ali", "phoneNumber": "777123456" }
         ▼
[2. API Controller]
         │ Receives data as TenantDto and calls _tenantService.CreateTenant(tenantDto)
         ▼
[3. TenantServiceImpl (Application Layer)]
         │ 1. Validates input
         │ 2. Converts TenantDto -> Tenant Entity using AutoMapper
         │ 3. Calls _tenantRepository.Add(tenantEntity)
         ▼
[4. TenantRepository (Infrastructure Layer)]
         │ 1. Adds entity to _context.Tenants
         │ 2. Calls _context.SaveChanges()
         ▼
[5. EF Core & SQL Server Database]
         │ Executes: INSERT INTO Tenants (FullName, PhoneNumber) VALUES ('Ahmad Ali', '777123456')
         ▼
[6. Response]
           Returns HTTP 200 OK / 201 Created back to the user!
```

---

## 5. Codebase Map (Where to Find What)

| Layer / Folder | Purpose | What is inside? |
| :--- | :--- | :--- |
| **`src/Core/Domain/Entities/`** | Database Entities | `Tenant.cs`, `Apartment.cs`, `PaymentRecord.cs`, `Issue.cs`, `Parcel.cs`, `User.cs`, `Role.cs` |
| **`src/Core/Domain/Repository/`** | Storage Interfaces | `ITenantRepository.cs`, `IApartmentRepository.cs`, `IPaymentRecordRepository.cs`, etc. |
| **`src/Core/Application/DTOs/`** | Transfer Models | `TenantDto.cs`, `ApartmentDto.cs`, `PaymentRecordDto.cs`, etc. |
| **`src/Core/Application/Interfaces/`** | Service Interfaces | `ITenantService.cs`, `IApartmentService.cs`, `IPaymentRecordService.cs`, etc. |
| **`src/Core/Application/ServiceImpl/`** | Business Logic | `TenantServiceImpl.cs`, `ApartmentServiceImpl.cs`, etc. |
| **`src/Core/Application/Mappings/`** | AutoMapper Config | `MappingConfig.cs` |
| **`src/Core/Infrastructure/Data/`** | Database Context | `AppDbContext.cs` |
| **`src/Core/Infrastructure/Repositories/`** | Real SQL Queries | `TenantRepository.cs`, `ApartmentRepository.cs`, etc. |
| **`ApartmentManagement.API/`** | Presentation API | `Startup.cs`, `Program.cs`, `appsettings.json`, Controllers |
| **`ApartmentManagmentSchema.sql`** | SQL Database Script | Table creation, foreign keys, and default role seeds |

---

## 6. Beginner Cheat Sheet & Glossary

* **Entity:** A C# class directly matching a database table (e.g., `Tenant`).
* **DTO (Data Transfer Object):** A clean C# class containing only the data sent over the network (e.g., `TenantDto`).
* **Interface (`I...`):** A contract listing methods that a class must implement, without writing the code inside (e.g., `ITenantRepository`).
* **Repository:** The class that handles storing, finding, and deleting data from the database (e.g., `TenantRepository`).
* **Service:** The class that executes business logic and orchestrates repositories and DTOs (e.g., `TenantServiceImpl`).
* **DbContext (`AppDbContext`):** The Entity Framework Core tool that translates C# code into SQL queries and executes them.
* **AutoMapper:** A helper tool that copies data between DTOs and Entities automatically.
* **Dependency Injection (DI):** Passing required objects into a constructor instead of hardcoding `new SomeClass()`.
* **CRUD:** **C**reate, **R**ead, **U**pdate, **D**elete — the four basic database actions.
