using ApartmentManagmentSystem.Core.Domain.Entities;
using System;
namespace ApartmentManagmentSystem.Core.Domain.Entities
{
public class Apartment
{
    public int Id { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public int NumberOfRooms { get; set; }
    public decimal MonthlyRent { get; set; }
    public string OccupancyStatus { get; set; } = "Vacant";

    // Foreign Key properties
    public int? CurrentTenantId { get; set; }

    // Navigation properties
    public Tenant? CurrentTenant { get; set; }
    public List<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
    public List<Issue> Issues { get; set; } = new List<Issue>();
}
}