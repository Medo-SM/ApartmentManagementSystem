namespace ApartmentManagementSystem.Core.Domain.Entities
{
using System;
using ApartmentManagementSystem.Core.Domain.Entities;
using ApartmentManagementSystem.Core.Domain.Entities.Base;

public class User : BaseEntity
    {

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    
        // Foreign Key properties
        public int RoleId { get; set; }
        public int? TenantId { get; set; }
    
        public bool IsActive { get; set; } = true;
    
        // Navigation properties
        public Role Role { get; set; } = null!;
        public Tenant? Tenant { get; set; }
    }
}