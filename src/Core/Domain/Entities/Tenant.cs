using ApartmentManagement.Core.Domain.Entities;
using ApartmentManagementSystem.Core.Domain.Entites.Base;
using ApartmentManagmentSystem.Core.Domain.Entities;
using System.Collections.Generic;

namespace ApartmentManagementSystem.Core.Domain.Entities
{

public class Tenant : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; }

        // Foreign Key & Navigation Properties
        public List<Apartment> Apartments { get; set; } = new List<Apartment>();
        public List<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
        public List<Issue> Issues { get; set; } = new List<Issue>();
        public List<Parcel> Parcels { get; set; } = new List<Parcel>();
        public User? User { get; set; }
    }
}