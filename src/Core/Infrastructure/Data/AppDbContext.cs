using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Core.Domain.Entities;
using ApartmentManagement.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apartment configuration
            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.HasIndex(e => e.UnitNumber).IsUnique();
                entity.Property(e => e.MonthlyRent).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.CurrentTenant)
                      .WithMany(t => t.Apartments)
                      .HasForeignKey(e => e.CurrentTenantId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // PaymentRecord configuration
            modelBuilder.Entity<PaymentRecord>(entity =>
            {
                entity.Property(e => e.AmountPaid).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.PaymentRecords)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Apartment)
                      .WithMany(a => a.PaymentRecords)
                      .HasForeignKey(e => e.ApartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Issue configuration
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Issues)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Apartment)
                      .WithMany(a => a.Issues)
                      .HasForeignKey(e => e.ApartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Parcel configuration
            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Parcels)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.TenantId).IsUnique();

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tenant)
                      .WithOne(t => t.User)
                      .HasForeignKey<User>(e => e.TenantId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
