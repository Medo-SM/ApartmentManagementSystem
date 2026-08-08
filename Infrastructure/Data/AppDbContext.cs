using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Core.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApartmentManagementSystem.Core.Domain.Entities;
using ApartmentManagement.Core.Domain.Entities;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments{ get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<PaymentRecord> PaymentRecords{ get; set; }
        public DbSet<Issue> Issues{ get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
