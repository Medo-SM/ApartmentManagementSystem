using System.Collections.Generic;
using Application.DTOs;
using Application.Mappings;
using ApartmentManagement.Core.Domain.Entities;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;

namespace Tests.Fixtures
{
    public static class TestFixtures
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }

        // ---- User ----
        public static UserDto CreateValidUserDto() => new UserDto
        {
            Id = 1,
            Username = "jdoe",
            Email = "jdoe@example.com",
            RoleId = 2,
            TenantId = 3,
            IsActive = true
        };

        public static User CreateValidUserEntity() => new User
        {
            Id = 1,
            Username = "jdoe",
            Email = "jdoe@example.com",
            PasswordHash = "hashed-password",
            RoleId = 2,
            TenantId = 3,
            IsActive = true
        };

        public static List<User> CreateUserList() => new List<User>
        {
            CreateValidUserEntity(),
            new User { Id = 2, Username = "asmith", Email = "asmith@example.com", RoleId = 2, TenantId = null, IsActive = false }
        };

        // ---- Apartment ----
        public static ApartmentDto CreateValidApartmentDto() => new ApartmentDto
        {
            Id = 1,
            UnitNumber = "101",
            FloorNumber = 1,
            NumberOfRooms = 3,
            MonthlyRent = 850.50m,
            OccupancyStatus = "Occupied",
            CurrentTenantId = 5
        };

        public static Apartment CreateValidApartmentEntity() => new Apartment
        {
            Id = 1,
            UnitNumber = "101",
            FloorNumber = 1,
            NumberOfRooms = 3,
            MonthlyRent = 850.50m,
            OccupancyStatus = "Occupied",
            CurrentTenantId = 5
        };

        public static List<Apartment> CreateApartmentList() => new List<Apartment>
        {
            CreateValidApartmentEntity(),
            new Apartment { Id = 2, UnitNumber = "202", FloorNumber = 2, NumberOfRooms = 4, MonthlyRent = 1200.00m, OccupancyStatus = "Vacant", CurrentTenantId = null }
        };

        // ---- Tenant ----
        public static TenantDto CreateValidTenantDto() => new TenantDto
        {
            Id = 1,
            FullName = "Jane Doe",
            PhoneNumber = "712345678",
            EmergencyContact = "711111111"
        };

        public static Tenant CreateValidTenantEntity() => new Tenant
        {
            Id = 1,
            FullName = "Jane Doe",
            PhoneNumber = "712345678",
            EmergencyContact = "711111111"
        };

        public static List<Tenant> CreateTenantList() => new List<Tenant>
        {
            CreateValidTenantEntity(),
            new Tenant { Id = 2, FullName = "Adam Smith", PhoneNumber = "709876543", EmergencyContact = null }
        };

        // ---- PaymentRecord ----
        public static PaymentRecordDto CreateValidPaymentRecordDto() => new PaymentRecordDto
        {
            Id = 1,
            TenantId = 2,
            ApartmentId = 3,
            AmountPaid = 850.50m,
            PaymentPeriodMonth = 8,
            PaymentPeriodYear = 2026,
            Status = "Paid"
        };

        public static PaymentRecord CreateValidPaymentRecordEntity() => new PaymentRecord
        {
            Id = 1,
            TenantId = 2,
            ApartmentId = 3,
            AmountPaid = 850.50m,
            PaymentPeriodMonth = 8,
            PaymentPeriodYear = 2026,
            Status = "Paid"
        };

        public static List<PaymentRecord> CreatePaymentRecordList() => new List<PaymentRecord>
        {
            CreateValidPaymentRecordEntity(),
            new PaymentRecord { Id = 2, TenantId = 4, ApartmentId = 3, AmountPaid = 400.00m, PaymentPeriodMonth = 8, PaymentPeriodYear = 2026, Status = "Partial" }
        };

        // ---- Issue ----
        public static IssueDto CreateValidIssueDto() => new IssueDto
        {
            Id = 1,
            ApartmentId = 2,
            TenantId = 3,
            Description = "Leaky faucet in the kitchen",
            Status = "Open",
            ResolvedDate = null
        };

        public static Issue CreateValidIssueEntity() => new Issue
        {
            Id = 1,
            ApartmentId = 2,
            TenantId = 3,
            Description = "Leaky faucet in the kitchen",
            Status = "Open",
            ResolvedDate = null
        };

        public static List<Issue> CreateIssueList() => new List<Issue>
        {
            CreateValidIssueEntity(),
            new Issue { Id = 2, ApartmentId = 5, TenantId = 6, Description = "Broken window", Status = "Resolved", ResolvedDate = System.DateTime.Today }
        };

        // ---- Parcel ----
        public static ParcelDto CreateValidParcelDto() => new ParcelDto
        {
            Id = 1,
            TenantId = 2,
            CourierName = "DHL",
            PickupTimestamp = null,
            Status = "Pending Pickup"
        };

        public static Parcel CreateValidParcelEntity() => new Parcel
        {
            Id = 1,
            TenantId = 2,
            CourierName = "DHL",
            PickupTimestamp = null,
            Status = "Pending Pickup"
        };

        public static List<Parcel> CreateParcelList() => new List<Parcel>
        {
            CreateValidParcelEntity(),
            new Parcel { Id = 2, TenantId = 3, CourierName = "UPS", PickupTimestamp = System.DateTime.Today, Status = "Picked Up" }
        };

        // ---- Role ----
        public static RoleDto CreateValidRoleDto() => new RoleDto
        {
            Id = 1,
            RoleName = "Building Owner"
        };

        public static Role CreateValidRoleEntity() => new Role
        {
            Id = 1,
            RoleName = "Building Owner"
        };

        public static List<Role> CreateRoleList() => new List<Role>
        {
            CreateValidRoleEntity(),
            new Role { Id = 2, RoleName = "Building Manager" }
        };
    }
}