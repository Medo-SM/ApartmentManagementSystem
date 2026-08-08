using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.Repository;
using Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public User? GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }
    }
}
