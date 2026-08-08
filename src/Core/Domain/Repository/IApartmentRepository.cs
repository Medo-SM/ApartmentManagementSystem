using ApartmentManagmentSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IApartmentRepository
    {
        void Add(Apartment apartment);
        void Update(Apartment apartment);
        void Delete(int Id);
        Apartment? GetById(int Id);
        List<Apartment> GetAll();
    }
}
