using System.Collections.Generic;
using System.Linq;
using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.Repository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        public void Update(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
        }

        public Role? GetById(int id)
        {
            return _context.Roles.Find(id);
        }

        public List<Role> GetAll()
        {
            return _context.Roles.ToList();
        }
    }
}