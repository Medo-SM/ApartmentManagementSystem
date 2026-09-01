using ApartmentManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IParcelRepository
    {
        void Add(Parcel parcel);
        void Update(Parcel parcel);
        void Delete(int Id);
        Parcel? GetById(int Id);
        List<Parcel> GetAll();
    }
}
