using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{
public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
    
        // Navigation properties
        public List<User> Users { get; set; } = new List<User>();
    }
}