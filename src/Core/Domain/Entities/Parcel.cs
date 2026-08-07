namespace ApartmentManagementSystem.Core.Domain.Entities
{
    using System;
    using ApartmentManagementSystem.Core.Domain.Entites.Base;

    public class Parcel : BaseEntity
    {
        // Foreign Key properties
        public int TenantId { get; set; }
    
        public string? CourierName { get; set; }
        public DateTime? PickupTimestamp { get; set; }
        public string Status { get; set; } = "Pending Pickup";
    
        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
    }
}
