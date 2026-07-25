-- Create Database
CREATE DATABASE ApartmentManagementDB;
GO

USE ApartmentManagementDB;
GO

-- 1. Tenants Table
CREATE TABLE Tenants (
    TenantID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,
    EmergencyContact VARCHAR(20) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Apartments Table
CREATE TABLE Apartments (
    ApartmentID INT IDENTITY(1,1) PRIMARY KEY,
    UnitNumber NVARCHAR(20) NOT NULL UNIQUE,
    FloorNumber INT NOT NULL,
    MonthlyRent DECIMAL(10, 2) NOT NULL CHECK (MonthlyRent >= 0),
    OccupancyStatus NVARCHAR(20) DEFAULT 'Vacant' CHECK (OccupancyStatus IN ('Vacant', 'Occupied', 'Maintenance')),
    CurrentTenantID INT NULL,
    CONSTRAINT FK_Apartments_Tenants FOREIGN KEY (CurrentTenantID) 
        REFERENCES Tenants(TenantID) ON DELETE SET NULL
);

-- 3. PaymentRecords Table
CREATE TABLE PaymentRecords (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    TenantID INT NOT NULL,
    ApartmentID INT NOT NULL,
    AmountPaid DECIMAL(10, 2) NOT NULL CHECK (AmountPaid > 0),
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentPeriodMonth INT NOT NULL CHECK (PaymentPeriodMonth BETWEEN 1 AND 12),
    PaymentPeriodYear INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Paid' CHECK (Status IN ('Paid', 'Partial', 'Pending')),
    CONSTRAINT FK_PaymentRecords_Tenants FOREIGN KEY (TenantID) 
        REFERENCES Tenants(TenantID),
    CONSTRAINT FK_PaymentRecords_Apartments FOREIGN KEY (ApartmentID) 
        REFERENCES Apartments(ApartmentID)
);

-- 4. Issues Table (Maintenance/Service Requests)
CREATE TABLE Issues (
    IssueID INT IDENTITY(1,1) PRIMARY KEY,
    ApartmentID INT NOT NULL,
    TenantID INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Open' CHECK (Status IN ('Open', 'In Progress', 'Resolved')),
    LoggedDate DATETIME DEFAULT GETDATE(),
    ResolvedDate DATETIME NULL,
    CONSTRAINT FK_Issues_Apartments FOREIGN KEY (ApartmentID) 
        REFERENCES Apartments(ApartmentID),
    CONSTRAINT FK_Issues_Tenants FOREIGN KEY (TenantID) 
        REFERENCES Tenants(TenantID)
);

-- 5. Parcels Table
CREATE TABLE Parcels (
    ParcelID INT IDENTITY(1,1) PRIMARY KEY,
    TenantID INT NOT NULL,
    CourierName NVARCHAR(100) NULL,
    ArrivalTimestamp DATETIME DEFAULT GETDATE(),
    PickupTimestamp DATETIME NULL,
    Status NVARCHAR(20) DEFAULT 'Pending Pickup' CHECK (Status IN ('Pending Pickup', 'Picked Up')),
    CONSTRAINT FK_Parcels_Tenants FOREIGN KEY (TenantID) 
        REFERENCES Tenants(TenantID)
);
GO
