# Apartment Management System

> A local-first, 100% offline property management desktop application built with C#, .NET 8/9, WPF, Entity Framework Core, and embedded SQLite/SQL Server database.

---

## 📖 Overview

The **Apartment Management System** is designed for property managers and building owners operating in low-resource environments with unstable or unavailable internet connectivity. It provides instant, sub-minute transaction processing for rent payment tracking, maintenance request management, parcel delivery check-in/pick-up, unit availability mapping, and secure local user authentication.

For the full detailed project specification, requirements, database schema, functional/non-functional requirements, WBS, and risk register, refer to:
- 📄 [**PROJECT_DOCUMENTATION.md**](file:///home/mohammed/Documents/projects/ApartmentManagmentSystem/PROJECT_DOCUMENTATION.md)
- 🏗️ [**ARCHITECTURE.md**](file:///home/mohammed/Documents/projects/ApartmentManagmentSystem/ARCHITECTURE.md)
- 🗄️ [**ApartmentManagmentSchema.sql**](file:///home/mohammed/Documents/projects/ApartmentManagmentSystem/ApartmentManagmentSchema.sql)

---

## 🚀 Key Modules & Capabilities

1. **Apartment & Occupancy Management:** Unit creation, room mapping, rent configuration, and real-time status tracking (`Vacant`, `Occupied`, `Under Maintenance`).
2. **Rent & Financial Tracking:** Local rent docket computation, full/partial payment logging, printable offline receipts, and overdue balance calculation.
3. **Maintenance & Issue Logging:** Repair request entry, priority management, repair cost logging, and resolution workflow (`Open`, `In Progress`, `Resolved`).
4. **Parcel Receiving & Pickup:** Courier package arrival logging and tenant pickup confirmation completed in <1 minute per entry.
5. **Local Database & Security:** 100% offline database operation with encrypted password hashing, transaction rollback protections, and automated local backup/restore tools.

---

## 🛠️ Architecture & Tech Stack

- **Architecture:** Clean Architecture (Onion Architecture)
- **UI Framework:** WPF (Windows Presentation Foundation) with `CommunityToolkit.Mvvm`
- **Backend Core:** .NET 8.0 / .NET 9.0 LTS
- **Data Access:** Entity Framework Core (EF Core)
- **Database Engine:** SQLite (Embedded) / SQL Server Express
- **Logging:** Serilog (`Serilog.Sinks.File`)

---

## 👥 Project Team & Governance

- **Project Sponsor / Instructor:** Eng. Rasheed Aldhaferi
- **Project Manager / Product Owner:** Mohammed Alsamawi
- **Operations Lead / Scrum Master:** Aiham Alhalmi
- **Team Member:** Habib Sabri