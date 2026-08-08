using ApartmentManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IRoleRepository
    {
        void Add(Role role);
        void Update(Role role);
        void Delete(int Id);
        Role? GetById(int Id);
        List<Role> GetAll();
    }
}
