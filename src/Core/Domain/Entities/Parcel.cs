using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{
public class Parcel
    {
        public int Id { get; set; }
    
        // Foreign Key properties
        public int TenantId { get; set; }
    
        public string? CourierName { get; set; }
        public DateTime ArrivalTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime? PickupTimestamp { get; set; }
        public string Status { get; set; } = "Pending Pickup";
    
        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
    }
}