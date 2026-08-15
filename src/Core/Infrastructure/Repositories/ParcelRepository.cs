
using System.Collections.Generic;
using System.Linq;
using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.Repository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ParcelRepository : IParcelRepository
    {
        private readonly AppDbContext _context;

        public ParcelRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Parcel parcel)
        {
            _context.Parcels.Add(parcel);
            _context.SaveChanges();
        }

        public void Update(Parcel parcel)
        {
            _context.Parcels.Update(parcel);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var parcel = _context.Parcels.Find(id);
            if (parcel != null)
            {
                _context.Parcels.Remove(parcel);
                _context.SaveChanges();
            }
        }

        public Parcel? GetById(int id)
        {
            return _context.Parcels.Find(id);
        }

        public List<Parcel> GetAll()
        {
            return _context.Parcels.ToList();
        }
    }
}