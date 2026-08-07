using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{

public class Issue : BaseEntity
    {    
        // Foreign Key properties
        public int ApartmentId { get; set; }
        public int TenantId { get; set; }
    
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public DateTime? ResolvedDate { get; set; }
    
        // Navigation properties
        public Apartment Apartment { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
    }
}