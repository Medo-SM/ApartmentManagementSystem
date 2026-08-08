using ApartmentManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface ITenantRepository
    {
        void Add(Tenant teneant);
        void Update(Tenant tenant);
        void Delete(int Id);
        Tenant? GetById(int Id);
        List<Tenant> GetAll();
    }
}
