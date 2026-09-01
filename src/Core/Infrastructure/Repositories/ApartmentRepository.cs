using System.Collections.Generic;
using System.Linq;
using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext _context;

        public ApartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Apartment apartment)
        {
            _context.Apartments.Add(apartment);
            _context.SaveChanges();
        }

        public void Update(Apartment apartment)
        {
            _context.Apartments.Update(apartment);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var apartment = _context.Apartments.Find(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
                _context.SaveChanges();
            }
        }

        public Apartment? GetById(int id)
        {
            return _context.Apartments.Find(id);
        }

        public List<Apartment> GetAll()
        {
            return _context.Apartments.ToList();
        }
    }
}