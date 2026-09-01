using System.Collections.Generic;
using System.Linq;
using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _context;

        public TenantRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            _context.SaveChanges();
        }

        public void Update(Tenant tenant)
        {
            _context.Tenants.Update(tenant);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var tenant = _context.Tenants.Find(id);
            if (tenant != null)
            {
                _context.Tenants.Remove(tenant);
                _context.SaveChanges();
            }
        }

        public Tenant? GetById(int id)
        {
            return _context.Tenants.Find(id);
        }

        public List<Tenant> GetAll()
        {
            return _context.Tenants.ToList();
        }
    }
}