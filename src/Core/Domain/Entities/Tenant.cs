using System;
using ApartmentManagementSystem.Core.Domain.Entities;
namespace ApartmentManagementSystem.Core.Domain.Entities
{

public class Tenant
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
        // Foreign Key & Navigation Properties
        public List<Apartment> Apartments { get; set; } = new List<Apartment>();
        public List<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
        public List<Issue> Issues { get; set; } = new List<Issue>();
        public List<Parcel> Parcels { get; set; } = new List<Parcel>();
        public User? User { get; set; }
    }
}