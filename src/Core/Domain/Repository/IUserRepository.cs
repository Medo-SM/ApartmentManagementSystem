using ApartmentManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    interface IUserRepository
    {
        void Add(User user);
        void Update(User user);
        void Delete(int Id);
        User? GetById(int Id);
        List<User> GetAll();
    }
}
