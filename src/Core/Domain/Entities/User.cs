using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{
public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    
        // Foreign Key properties
        public int RoleId { get; set; }
        public int? TenantId { get; set; }
    
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
        // Navigation properties
        public Role Role { get; set; } = null!;
        public Tenant? Tenant { get; set; }
    }
}