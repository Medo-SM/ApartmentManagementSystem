<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>

<!-- PROJECT SHIELDS -->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]



<!-- PROJECT HEADER -->

<br />
<div align="center">
  <h3 align="center">Apartment Management System (AMS)</h3>

  <p align="center">
    A centralized apartment and property management system replacing manual paper logbooks
    for building managers and owners.
    <br />
    <a href="https://github.com/Medo-SM/ApartmentManagmentSystem"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Medo-SM/ApartmentManagmentSystem/issues/new?labels=bug">Report Bug</a>
    &middot;
    <a href="https://github.com/Medo-SM/ApartmentManagmentSystem/issues/new?labels=enhancement">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#testing">Testing</a></li>
    <li><a href="#documentation">Documentation</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

Managing an apartment building with paper ledgers or spreadsheets often leads to missed rent payments,
lost package records, and forgotten maintenance requests. AMS provides a centralized system to manage
daily building operations:

- **Apartments & Units:** Track rooms, floor numbers, rent prices, and occupancy status (`Vacant`, `Occupied`, `Under Maintenance`).
- **Tenants:** Register and manage tenant contact info, emergency contacts, and unit assignments.
- **Rent & Payments:** Log monthly rent payments, track payment statuses (`Paid`, `Partial`, `Pending`), and calculate remaining balances.
- **Maintenance Issues:** Log maintenance and repair requests, assign priorities, and track status from `Open` to `Resolved`.
- **Package / Parcel Tracking:** Log incoming deliveries for tenants and record pickup timestamps.
- **User Roles & Auth:** Role-based access control for Building Owners, Building Managers, and Tenants.

The solution follows **Clean Architecture (Onion Architecture)** principles to keep business logic
independent from data access and UI frameworks:

```
ApartmentManagementSystem/
├── src/
│   └── Core/
│       ├── Domain/             # Core entities & repository interfaces
│       ├── Application/        # DTOs, service interfaces, implementations & AutoMapper mappings
│       └── Infrastructure/     # EF Core, AppDbContext & repository implementations
├── ApartmentManagement.API/     # ASP.NET Core Web API & Swagger endpoints
└── documentation/              # SWE & ITPM project documentation and diagrams
```

> 📖 **New to Clean Architecture?** Check out our beginner-friendly guide: [**BACKEND_ARCHITECTURE.md**](BACKEND_ARCHITECTURE.md) for a step-by-step explanation and code walkthrough.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

The major frameworks and libraries used to bootstrap this project:

* [![.NET 5][.NET 5]][dotnet-url]
* [![C#][C#]][dotnet-url]
* [![EF Core][EF Core]][efcore-url]
* [![AutoMapper][AutoMapper]][automapper-url]
* [![Swagger][Swagger]][swagger-url]
* [![SQL Server][SQL Server]][sqlserver-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

These instructions will get you a local copy of the project up and running on your machine.

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (5.0 or later)
* [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express / LocalDB)
* [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) or Azure Data Studio
* [Visual Studio](https://visualstudio.microsoft.com/) or Visual Studio Code

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Medo-SM/ApartmentManagmentSystem.git
   cd ApartmentManagmentSystem
   ```

2. Set up the database using **EF Core Migrations**:

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

3. Configure the database connection & secrets:

   **Option A: Secure Local Secrets (Recommended for SQL Auth with Password):**
   ```bash
   cd ApartmentManagement.API
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ApartmentManagementDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
   ```

   **Option B: Windows Authentication / Integrated Security:**
   Update `ApartmentManagement.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ApartmentManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
     }
   }
   ```

4. Build and run
   ```bash
   # Restore dependencies and build the solution
   dotnet build

   # Run the API project
   dotnet run --project ApartmentManagement.API
   ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Once the API is running, navigate to `https://localhost:5001/swagger` (or `http://localhost:5000/swagger`)
to explore and test all RESTful endpoints (`/api/User`, `/api/Apartment`, `/api/Tenant`, `/api/Issue`,
`/api/Parcel`, `/api/PaymentRecord`, `/api/Role`) via Swagger UI.

You can also call the API directly, for example:

**GET all users:**
```bash
curl -X 'GET' 'https://localhost:5001/api/User' -H 'accept: text/plain'
```

**Create a user (POST):**
```bash
curl -X 'POST' 'https://localhost:5001/api/User' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
    "username": "jdoe",
    "email": "jdoe@example.com",
    "roleId": 2,
    "isActive": true
  }'
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- TESTING -->
## Testing

The solution includes a unit test project (`src/Tests/`) using **xUnit**, **Moq**, and the real
**AutoMapper** mapping profile. It unit-tests all 7 service implementations — verifying repository
delegation, null handling, mapped return values, entity &lt;-&gt; DTO round-trips, and `RoleServiceImpl`'s
default role-seeding logic. No database is required; all repositories are mocked.

```bash
dotnet test src/Tests/
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- DOCUMENTATION -->
## Documentation

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

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create.
Any contributions you make are **greatly appreciated**.

Please read our [**CONTRIBUTING.md**](CONTRIBUTING.md) for details on the process for submitting pull
requests to us. If you have a suggestion that would make this project better, please fork the repo and
create a pull request. You can also simply open an issue with the tag "enhancement".

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Mohammed Alsamawi - malsmawi1@gmail.com

Project Link: [https://github.com/Medo-SM/ApartmentManagmentSystem](https://github.com/Medo-SM/ApartmentManagmentSystem)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
* [draw.io](https://app.diagrams.net/) for the engineering diagrams
* [SRS Document (IEEE)](https://www.ieee.org/)
* [Microsoft Learn - Entity Framework Core](https://learn.microsoft.com/ef/core/)
* [Img Shields](https://shields.io)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Medo-SM/ApartmentManagmentSystem.svg?style=for-the-badge
[contributors-url]: https://github.com/Medo-SM/ApartmentManagmentSystem/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Medo-SM/ApartmentManagmentSystem.svg?style=for-the-badge
[forks-url]: https://github.com/Medo-SM/ApartmentManagmentSystem/network/members
[stars-shield]: https://img.shields.io/github/stars/Medo-SM/ApartmentManagmentSystem.svg?style=for-the-badge
[stars-url]: https://github.com/Medo-SM/ApartmentManagmentSystem/stargazers
[issues-shield]: https://img.shields.io/github/issues/Medo-SM/ApartmentManagmentSystem.svg?style=for-the-badge
[issues-url]: https://github.com/Medo-SM/ApartmentManagmentSystem/issues
[license-shield]: https://img.shields.io/github/license/Medo-SM/ApartmentManagmentSystem.svg?style=for-the-badge
[license-url]: https://github.com/Medo-SM/ApartmentManagmentSystem/blob/master/LICENSE
[.NET 5]: https://img.shields.io/badge/.NET%205-512BD4?style=for-the-badge
[dotnet-url]: https://dotnet.microsoft.com/
[C#]: https://img.shields.io/badge/C%23-239120?style=for-the-badge
[EF Core]: https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge
[efcore-url]: https://learn.microsoft.com/ef/core/
[AutoMapper]: https://img.shields.io/badge/AutoMapper-525252?style=for-the-badge
[automapper-url]: https://automapper.org/
[Swagger]: https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge
[swagger-url]: https://swagger.io/
[SQL Server]: https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge
[sqlserver-url]: https://www.microsoft.com/sql-server/