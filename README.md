# Apartment Management System (AMS)

An apartment and property management system built with C# and .NET using Clean Architecture. This project was developed as part of our Software Engineering (SWE) and IT Project Management (ITPM) coursework to replace manual paper logbooks for building managers and owners.

---

## 📌 Project Overview

Managing an apartment building with paper ledgers or spreadsheets often leads to missed rent payments, lost package records, and forgotten maintenance requests. 

**Apartment Management System (AMS)** provides a centralized system to manage daily building operations:
- **Apartments & Units:** Track rooms, floor numbers, rent prices, and occupancy status (`Vacant`, `Occupied`, `Under Maintenance`).
- **Tenants:** Register and manage tenant contact info, emergency contacts, and unit assignments.
- **Rent & Payments:** Log monthly rent payments, track payment statuses (`Paid`, `Partial`, `Pending`), and calculate remaining balances.
- **Maintenance Issues:** Log maintenance and repair requests, assign priorities, and track status from `Open` to `Resolved`.
- **Package / Parcel Tracking:** Log incoming deliveries for tenants and record pickup timestamps.
- **User Roles & Auth:** Role-based access control for Building Owners, Building Managers, and Tenants.

---

## 🏛️ Project Architecture

The solution is organized following **Clean Architecture (Onion Architecture)** principles to keep business logic independent from data access and UI frameworks:

```
ApartmentManagementSystem/
├── src/
│   └── Core/
│       ├── Domain/             # Core entities & repository interfaces
│       ├── Application/        # DTOs, service interfaces, implementations & AutoMapper mappings
│       └── Infrastructure/     # EF Core, AppDbContext & repository implementations
├── ApartmentManagement.API/     # ASP.NET Core Web API & Swagger endpoints
├── documentation/              # SWE & ITPM project documentation and diagrams
└── ApartmentManagmentSchema.sql # Database schema & seed scripts
```

> 📖 **New to Clean Architecture?** Check out our beginner-friendly guide: [**BACKEND_ARCHITECTURE.md**](BACKEND_ARCHITECTURE.md) for a step-by-step explanation and code walkthrough.

---

## 🛠️ Tech Stack

- **Language:** C#
- **Framework:** .NET Core / .NET 5.0
- **Data Access:** Entity Framework Core (EF Core)
- **Object Mapping:** AutoMapper
- **API Documentation:** Swagger / Swashbuckle
- **Database:** Microsoft SQL Server / T-SQL
- **Version Control & Management:** Git, GitHub Actions, Conventional Commits

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (5.0 or later)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express / LocalDB)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) or Azure Data Studio
- [Visual Studio](https://visualstudio.microsoft.com/) or Visual Studio Code

### 1. Clone the Repository
```bash
git clone https://github.com/Medo-SM/ApartmentManagementSystem.git
cd ApartmentManagementSystem
```

### 2. Set Up the Database

You can set up the database using either **EF Core Migrations** or the provided **SQL Script**:

- **Option A (EF Core Migrations - Recommended):**
  Using Visual Studio Package Manager Console (PMC):
  ```powershell
  # Set Default project to Infrastructure
  Add-Migration InitialCreate -StartupProject ApartmentManagement.API
  Update-Database -StartupProject ApartmentManagement.API
  ```
  Or via .NET CLI:
  ```bash
  dotnet ef database update --project src/Core/Infrastructure --startup-project ApartmentManagement.API
  ```

- **Option B (Direct SQL Script):**
  Open and execute [`ApartmentManagmentSchema.sql`](ApartmentManagmentSchema.sql) in SQL Server Management Studio (SSMS) or Azure Data Studio to create `ApartmentManagementDB` and seed default roles.

### 3. Configure Database Connection & Secrets

To follow security best practices and prevent credentials from being committed to source control:

- **Option A: Secure Local Secrets (Recommended for SQL Auth with Password):**
  ```bash
  cd ApartmentManagement.API
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ApartmentManagementDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  ```

- **Option B: Windows Authentication / Integrated Security:**
  Update `ApartmentManagement.API/appsettings.json`:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=ApartmentManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
    }
  }
  ```

### 4. Build and Run
Using the .NET CLI:
```bash
# Restore dependencies and build the solution
dotnet build

# Run the API project
dotnet run --project ApartmentManagement.API
```

Once running, navigate to `https://localhost:5001/swagger` (or `http://localhost:5000/swagger`) to explore and test all RESTful endpoints (`/api/User`, `/api/Apartment`, `/api/Tenant`, `/api/Issue`, `/api/Parcel`, `/api/PaymentRecord`, `/api/Role`) via Swagger UI.

---

## 📂 Project Documentation & Diagrams

All project planning, requirements, and engineering diagrams are located in the `documentation/` folder:

- **Software Engineering (SWE):**
  - [SRS Document](documentation/SWE/SRS%20Document.docx) (IEEE standard format)
  - [Use Case Diagram](documentation/SWE/UseCaseDiagram/UseCaseDiagram.png)
  - [Activity Diagram](documentation/SWE/ActivityDiagrams/ActivityDiagram-RegisterNewTenant(UC-001)/ActivityDiagram-RegisterNewTenant(UC-001).png)
  - [Sequence Diagram](documentation/SWE/SequenceDiagram/SequenceDiagram.drawio.png)
  - [Swimlane Diagram](documentation/SWE/SwimlaneDiagram/SwimlaneDiagram.png)
  - [State Diagram](documentation/SWE/stateDiagram/stateDiagram.drawio.png)

- **IT Project Management (ITPM):**
  - Project Charter & Team Charter
  - Work Breakdown Structure (WBS)
  - Scope Statement & Business Case
  - RACI Matrix & Stakeholder Register
  - Risk Register & Issue Log
  - Change Log & Formal Change Requests
