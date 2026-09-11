# Context & System Specification: Apartment Management System (AMS)

> **Purpose:** This document is the **living source of truth** for AI agents and developers working on the Apartment Management System. It documents the current implemented architecture, data model, and interfaces, and defines the boundaries that future work must respect.
>
> **How to use:** Where this document and the engineering artifacts differ, **this document reflects the implemented code**. The project evolves through approved changes; update this document whenever the architecture, schema, or scope changes.
>
> **CRITICAL DIRECTIVE FOR AI AGENTS:** You must strictly abide by the scope and boundaries defined in this document. **Any suggestion, implementation, or inclusion of Out-of-Scope features (e.g., cloud sync, online payment gateways, mobile apps, SMS APIs) constitutes scope creep and is strictly forbidden.**

---

## 1. Project Overview & Identity

* **Project Name:** Apartment Management System (AMS)
* **Project Type:** Local-first Property Management Software (V1.0)
* **Primary Target Users:** Local Apartment Building Managers and Building Owners
* **Target Operating Environment:** Windows 10 / 11 Desktop (100% Offline capability)
* **Core Problem Solved:** Property managers rely on manual paper ledgers, fragmented logs, and memory to track rent, vacancies, parcel deliveries, and repair requests. AMS digitizes and centralizes these operations into a single offline system with zero calculation errors and sub-minute transaction logging.
* **Current Form:** ASP.NET Core Web API backend with Swagger documentation. A desktop GUI is planned but not yet implemented.

### 1.1 Project Governance & Stakeholders

| Stakeholder Name | Role / Title | Responsibility Focus | Influence / Interest |
| :--- | :--- | :--- | :--- |
| **Eng. Rasheed Aldhaferi** | Project Sponsor / Course Instructor | Project authorization, formal reviews, and academic evaluation. | High / High |
| **Mohammed Alsamawi** | Project Manager / Product Owner / Strategic Lead | Scope definition, WBS decomposition, Charter, Business Case, and Backlog management. | High / High |
| **Aiham Alhalmi** | Operations Lead / Scrum Master / Governance Lead | Risk register, Issue log, RACI enforcement, Change Control, and Milestone tracking. | High / High |
| **Habib Sabri** | Technical Team Member | Technical implementation, database layer, and backup utilities. | Medium / Medium |
| **Building / Property Managers** | Primary End Users (External) | Operational rent logging, parcel intake, and maintenance issue tracking. | Medium / High |
| **Building Owners** | Business Owners / Investors (External) | Accurate financial reporting, overdue balances, and data integrity. | High / Medium |
| **Apartment Tenants** | Secondary Beneficiaries | Accurate receipts, timely maintenance, and secure package pickups. | Low / Medium |

---

## 2. Strict Project Boundaries & Scope Control

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                             IN-SCOPE BOUNDARY                               │
│                                                                             │
│  ┌────────────────────────┐  ┌───────────────────────┐  ┌────────────────┐  │
│  │ 1. Occupancy & Units   │  │ 2. Rent & Payments    │  │ 3. Maintenance │  │
│  │    (Vacant/Occupied)   │  │    (Cash/Dockets)     │  │    (Issues)    │  │
│  └────────────────────────┘  └───────────────────────┘  └────────────────┘  │
│  ┌────────────────────────┐  ┌───────────────────────┐  ┌────────────────┐  │
│  │ 4. Parcel Receiving    │  │ 5. Local Database     │  │ 6. PDF/Arabic  │  │
│  │    (Pickup Tracking)   │  │    (SQL Server)       │  │    (CR-002/003)│  │
│  └────────────────────────┘  └───────────────────────┘  └────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
                                      ▲
                                      │ STRICT WALL (ZERO CREEP)
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         FORBIDDEN / OUT-OF-SCOPE                            │
│  ❌ Online Payment Gateways (Jaib, OneCash, Stripe, Bank APIs) [CR-001]     │
│  ❌ Cloud Database Synchronization & Remote APIs                            │
│  ❌ Mobile Applications (iOS / Android)                                     │
│  ❌ Automated SMS / Email Gateways (Twilio, SendGrid)                       │
│  ❌ Multi-Building Enterprise SaaS Multi-Tenancy Architecture               │
└─────────────────────────────────────────────────────────────────────────────┘
```

All work implemented or proposed must stay within the 5 core modules or the approved change requests (CR-002, CR-003).

---

## 3. Actual Architecture & Tech Stack

### 3.1 Tech Stack

| Layer | Technology |
| :--- | :--- |
| Language / Runtime | C# / .NET 5.0 (`RollForward: LatestMajor` so it runs on newer .NET runtimes) |
| Framework | ASP.NET Core 5.0 Web API |
| ORM | Entity Framework Core 5.0.17 |
| Database | Microsoft SQL Server (`ApartmentManagementDB`) |
| Object Mapping | AutoMapper 10.1.1 |
| API Docs | Swashbuckle / Swagger (OpenAPI) 5.6.3 |
| Testing | xUnit 2.4.1 + Moq 4.16.1 |
| Versioning | Semantic version via `package.json` + conventional-changelog (currently v0.7.0) |

### 3.2 Clean Architecture (Layered / Onion)

```
                    +--------------------------+
                    |   Presentation Layer     |  ApartmentManagement.API
                    |   (Controllers, Swagger) |
                    +--------------------------+
                             │  uses
                             ▼
                    +--------------------------+
                    |    Application Layer     |  Application
                    |  (Services, DTOs, AutoMapper)
                    +--------------------------+
                             │  uses
                             ▼
                    +--------------------------+
                    |      Domain Layer        |  Domain (zero external deps)
                    |  (Entities, Repo Interfaces)
                    +--------------------------+
                             ▲  implements
                    +--------------------------+
                    |  Infrastructure Layer    |  Infrastructure
                    | (DbContext, EF, Repos)   |
                    +--------------------------+

                    +--------------------------+
                    |         Tests            |  Tests (xUnit + Moq)
                    +--------------------------+
```

**Key principles applied:**
1. **Dependency Inversion:** Services depend on repository *interfaces* (defined in Domain), implemented in Infrastructure, wired in `Program.cs`.
2. **DTO isolation:** raw entities never cross the API boundary; only DTOs are serialized.
3. **Primary-Key isolation:** `MappingConfig.cs` ignores `Id` on DTO→Entity maps (the DB owns identity); update services restore `Id` after mapping.
4. **Composition root:** `Program.cs` (no `Startup.cs`) wires DbContext, repositories, services, AutoMapper, controllers, and Swagger, plus repository/services DI registration.

**Project layout:**

```
src/
├── Core/
│   ├── ApartmentManagement.API/    # controllers, Program.cs, appsettings, Swagger
│   ├── Application/                # DTOs, Interfaces, ServiceImpl, Mappings
│   ├── Domain/                     # Entities, IRepository interfaces
│   └── Infrastructure/             # Data/AppDbContext, Migrations, Repositories
└── Tests/                          # xUnit service tests + mapping tests
```

---

## 4. Data Schema (Actual EF Core Migration)

Schema is defined by the EF Core migration `20260824121516_InitialCreate` (`src/Core/Infrastructure/Migrations/`) and `AppDbContext` Fluent API config. **Use these as schema truth** — there is no separate `.sql` schema file.

> **Important:** All tables store status fields as **nullable `nvarchar(max)` strings** (not enums) and there are **no DB CHECK constraints**. Allowed status values, ranges, and required fields are enforced **only at the API DTO layer** via Data Annotations (see §5). This means raw SQL inserts can bypass validation.

### 4.1 Relationships

```
Roles (1) ----< (N) Users ----(1:1)---- Tenants (0..1)
Tenants (1) ----< (N) Apartments
Tenants (1) ----< (N) PaymentRecords
Apartments (1) ----< (N) PaymentRecords
Tenants (1) ----< (N) Issues
Apartments (1) ----< (N) Issues
Tenants (1) ----< (N) Parcels
```

### 4.2 Tables (as migrated)

| Table | Key Columns (types) | Constraints / Behaviors |
| :--- | :--- | :--- |
| **Roles** | `Id` int IDENTITY, `RoleName` nvarchar(450) | Unique index on `RoleName` |
| **Tenants** | `Id` int IDENTITY, `FullName` nvarchar(max), `PhoneNumber` nvarchar(max), `EmergencyContact` nvarchar(max) nullable | — |
| **Apartments** | `Id` int IDENTITY, `UnitNumber` nvarchar(450), `FloorNumber` int, `NumberOfRooms` int, `MonthlyRent` decimal(10,2), `OccupancyStatus` nvarchar(max), `CurrentTenantId` int nullable | Unique index on `UnitNumber`; FK → Tenants `ON DELETE SET NULL` |
| **PaymentRecords** | `Id` int IDENTITY, `TenantId` int, `ApartmentId` int, `AmountPaid` decimal(10,2), `PaymentPeriodMonth` int, `PaymentPeriodYear` int, `Status` nvarchar(max) | FK → Tenants (Restrict); FK → Apartments (Restrict) |
| **Issues** | `Id` int IDENTITY, `ApartmentId` int, `TenantId` int, `Description` nvarchar(max), `Status` nvarchar(max), `ResolvedDate` datetime2 nullable | FK → Apartments (Restrict); FK → Tenants (Restrict) |
| **Parcels** | `Id` int IDENTITY, `TenantId` int, `CourierName` nvarchar(max) nullable, `PickupTimestamp` datetime2 nullable, `Status` nvarchar(max) | FK → Tenants (Cascade) |
| **Users** | `Id` int IDENTITY, `Username` nvarchar(450), `Email` nvarchar(450), `PasswordHash` nvarchar(max), `RoleId` int, `TenantId` int nullable, `IsActive` bit | Unique indexes on `Username`, `Email`, `TenantId`; FK → Roles (Restrict); FK → Tenants `ON DELETE SET NULL` |

### 4.3 Domain Entities

All entities derive from `BaseEntity` (`Id` int PK identity, `CreatedAt`, `UpdatedAt` auto-set to `DateTime.Now`).

* **Role** — `RoleName`. Default roles seeded on role creation: "Building Owner", "Building Manager", "Tenant".
* **Tenant** — `FullName`, `PhoneNumber`, `EmergencyContact?`. Navigations to apartments, payment records, issues, parcels, user.
* **Apartment** — `UnitNumber` (unique), `FloorNumber`, `NumberOfRooms`, `MonthlyRent` (decimal), `OccupancyStatus` (`"Vacant"` default; allowed `"Vacant" | "Occupied" | "Maintenance"`), `CurrentTenantId?`.
* **PaymentRecord** — `TenantId`, `ApartmentId`, `AmountPaid` (decimal(10,2)), `PaymentPeriodMonth`, `PaymentPeriodYear`, `Status` (`"Paid"` default; allowed `"Paid" | "Partial" | "Pending"`).
* **Issue** — `ApartmentId`, `TenantId`, `Description`, `Status` (`"Open"` default; allowed `"Open" | "In Progress" | "Resolved"`), `ResolvedDate?`.
* **Parcel** — `TenantId`, `CourierName?`, `PickupTimestamp?`, `Status` (`"Pending Pickup"` default; allowed `"Pending Pickup" | "Picked Up"`).
* **User** — `Username`, `Email`, `PasswordHash`, `RoleId`, `TenantId?` (1:1 user–tenant link), `IsActive` (default `true`).

---

## 5. DTO Validation (API Input Boundary)

Validation uses **Data Annotations on DTOs**. Because `BaseController` is `[ApiController]`, ASP.NET Core auto-validates `ModelState` and returns **HTTP 400** with field-level errors before service code runs. Swagger reflects these rules.

| DTO | Field | Rules |
| :--- | :--- | :--- |
| **UserDto** | `Username` | `[Required]`, `[StringLength(50)]`, `^[a-zA-Z0-9]*$` |
| | `Email` | `[Required]`, `[EmailAddress]` |
| | `RoleId` | `[Required]`, `[Range(1, int.MaxValue)]` |
| | `TenantId` / `IsActive` | no attributes (optional / default) |
| **TenantDto** | `FullName` | `[Required]`, `[StringLength(100)]` |
| | `PhoneNumber` | `[Required]`, `^7\d{8}$` (Yemeni 9-digit) |
| | `EmergencyContact` | `^7\d{8}$` |
| **ApartmentDto** | `UnitNumber` | `[Required]`, `[StringLength(4)]` |
| | `FloorNumber` | `[Required]`, `[Range(1, 20)]` |
| | `NumberOfRooms` | `[Range(1, 25)]` |
| | `MonthlyRent` | `[Range(0, double.MaxValue)]` |
| | `OccupancyStatus` | `[Required]`, `^(Vacant\|Occupied\|Maintenance)$` |
| | `CurrentTenantId` | `[Range(1, int.MaxValue)]` (optional) |
| **PaymentRecordDto** | `TenantId` / `ApartmentId` | `[Required]`, `[Range(1, int.MaxValue)]` |
| | `AmountPaid` | `[Required]`, `[Range(0.01, double.MaxValue)]` |
| | `PaymentPeriodMonth` | `[Range(1, 12)]` |
| | `PaymentPeriodYear` | `[Range(2000, 2100)]` |
| | `Status` | `[Required]`, `^(Paid\|Partial\|Pending)$` |
| **IssueDto** | `ApartmentId` / `TenantId` | `[Required]`, `[Range(1, int.MaxValue)]` |
| | `Description` | `[Required]`, `[StringLength(300, MinimumLength = 1)]` |
| | `Status` | `[Required]`, `^(Open\|In Progress\|Resolved)$` |
| | `ResolvedDate` | nullable, no attribute |
| **ParcelDto** | `TenantId` | `[Range(1, int.MaxValue)]` |
| | `CourierName` | `[StringLength(100)]`, nullable |
| | `PickupTimestamp` | nullable, no attribute |
| | `Status` | `[Required]`, `^(Pending Pickup\|Picked Up)$` |
| **RoleDto** | `RoleName` | `[Required]`, `[StringLength(100, MinimumLength = 2)]` |

**Note:** statuses are validated by regex on **strings**, not enums. Submitting e.g. an enum-looking value `"InProgress"` is valid only if it matches the exact allowed string.

---

## 6. API Surface

All controllers inherit from `BaseController` (`[ApiController]`, route `api/[controller]`). Response envelope: `200 OK` with data on success, `404 { message, success: false }` when a resource is not found, `500 { message, success: false, error }` on exceptions (`BaseController.HandleResponse` / `HandleError`).

| Route | Controller | HTTP Methods |
| :--- | :--- | :--- |
| `/api/User` | UserController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/Apartment` | ApartmentController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/Tenant` | TenantController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/PaymentRecord` | PaymentRecordController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/Issue` | IssueController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/Parcel` | ParcelController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |
| `/api/Role` | RoleController | POST, GET, GET/{id}, PUT/{id}, DELETE/{id} |

**Total: 7 resource controllers × 5 CRUD methods = 35 REST endpoints**, plus Swagger UI at `/swagger`.

Referential creation order (FK dependencies): **Roles → Tenants → Apartments → Users → PaymentRecords → Issues → Parcels**.

---

## 7. Implemented vs. Planned Feature Matrix

| Feature / Module | Status | Notes |
| :--- | :--- | :--- |
| Apartment & occupancy CRUD | ✅ Implemented | 5 endpoints; no occupancy consistency enforcement (planned) |
| Rent / payment logging CRUD | ✅ Implemented | decimal(10,2) money; month/year docket fields |
| Maintenance issue CRUD | ✅ Implemented | status + ResolvedDate; no priority/cost fields |
| Parcel receiving / pickup CRUD | ✅ Implemented | status + PickupTimestamp |
| User / role CRUD + default role seeding | ✅ Implemented | 3 default roles seeded on role create |
| DTO input validation | ✅ Implemented | Data Annotations, auto-400 (v0.7.0) |
| AutoMapper PK isolation | ✅ Implemented | `Id` ignored on writes (v0.5.2) |
| Unit tests (64 service + mapping tests) | ✅ Implemented | xUnit + Moq, `dotnet test src/Tests/` |
| Domain status enums + Swagger dropdowns | ❌ Planned | statuses still strings; regex-validated |
| Service-layer FK sanitization | ❌ Planned | `0` nullable FKs currently map to `0`, not `null` |
| Apartment FK/occupancy consistency (service + UI) | ❌ Planned | vacant-with-tenant / occupied-without-not enforced |
| Authentication / login (password hashing in use) | ❌ Planned | only `PasswordHash` column exists; no login/JWT |
| Backup / restore utilities | ❌ Planned | none implemented |
| Overdue balance calculation | ❌ Planned | none implemented |
| Printable transaction receipts | ❌ Planned | none implemented |
| Issue priority + repair cost tracking | ❌ Planned | schema lacks the fields |
| PDF export (CR-002) | ❌ Planned | approved; not implemented |
| Arabic / English localization (CR-003) | ❌ Planned | approved; not implemented |
| Windows desktop GUI | ❌ Planned | only the Web API backend exists |

**Rule for agents:** a feature listed as ❌ **must not be assumed present**. Verify in code before relying on it; flag gaps rather than writing workarounds.

---

## 8. Non-Functional Requirements & Project Constraints

1. **100% Offline Operational Continuity:** the application must launch, read, write, query, and generate reports with **zero internet connection**.
2. **Transaction Speed & Ergonomics:** routine daily transactions (logging rent payments, parcel arrivals, repairs) must take **< 1 minute** per entry.
3. **Data Integrity & Crash Resilience (RSK-001):** the local SQL Server database enforces ACID transactions; unplanned shutdowns must cause zero corruption. A local backup mechanism (manual/automated exports) is planned.
4. **Zero Budget Constraint:** development relies strictly on open-source libraries, free toolchains, and embedded database engines without commercial licensing fees.
5. **Single-Workstation Concurrency:** concurrency is managed locally; multi-user cloud sync is **not** required.

---

## 9. Work Breakdown Structure (WBS) Reference

All codebase activities correspond to the approved WBS dictionary:

* `1.1` **Project Initiation Documents** (Problem Statement, Team Charter, Project Charter, Business Case)
* `1.2` **Stakeholder & Scope Management** (Stakeholder Register, System Boundaries, Scope Statement)
* `1.3` **Project Baseline Planning** (WBS, Milestone Plan, RACI Matrix, Risk Register, Communication Plan)
* `1.4` **Software System Architecture & Implementation**
  * `1.4.1` Occupancy Management Module
  * `1.4.2` Rent & Payment Module (`PaymentRecord`)
  * `1.4.3` Maintenance Logging Module (`Issue`)
  * `1.4.4` Package Receiving Module (`Parcel`)
  * `1.4.5` Offline Database Layer & Backup Utility
* `1.5` **Project Control, Quality & Change Management** (Quality Checklist, Issue Log, Change Control System)
* `1.6` **Agile Framework & Project Closure** (Product Backlog, Sprint Planning, Midterm Portfolio, Final Presentation)

---

## 10. Rules for AI Agents Working on This Codebase

When writing code, reviewing pull requests, generating features, or answering questions:

1. **Never Violate System Boundaries:**
   * If asked to add online payments, cloud hosting, mobile apps, web APIs, or SMS/email gateways, **refuse and cite CR-001 and §2** of this document.
2. **Adhere to the Layered Architecture:**
   * Maintain separation between **Domain** (business entities & rules), **Application** (DTOs & service interfaces/implementations), **Infrastructure** (EF Core DbContext & repositories), and **API** (controllers). Dependency direction is inward.
3. **Enforce Financial Accuracy:**
   * Always use `decimal` for monetary values (`MonthlyRent`, `AmountPaid`). Never use `float` or `double` for money.
4. **Maintain Database Integrity:**
   * Keep foreign keys, cascades, uniqueness constraints, and column types aligned with `AppDbContext` and the `InitialCreate` migration. There is no separate `.sql` schema file; the migration is authoritative. Existing migrations must not be edited — add new ones (`dotnet ef migrations add <Name>`).
5. **Never Trust Client Input:**
   * `Id` must stay ignored on writes (see `MappingConfig.cs`); update services restore `Id` from the DTO after mapping.
6. **Respect the Implemented-vs-Planned Matrix (§7):**
   * Do not claim unimplemented features exist. When a task needs a ❌/Planned feature, treat it as new work within scope (or request a change).
7. **Keep UI / Delivery Local-First:**
   * All UI workflows target local desktop presentation without web-browser hosting or cloud dependencies.