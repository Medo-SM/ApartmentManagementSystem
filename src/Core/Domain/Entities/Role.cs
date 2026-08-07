using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{
public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;
    
        // Navigation properties
        public List<User> Users { get; set; } = new List<User>();
    }
}