namespace ApartmentManagementSystem.Core.Domain.Entities
{
using ApartmentManagmentSystem.Core.Domain.Entities;
using System;
using ApartmentManagementSystem.Core.Domain.Entities.Base;    
public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;
    
        // Navigation properties
        public List<User> Users { get; set; } = new List<User>();
    }
}