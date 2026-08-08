using ApartmentManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    interface IIssueRepository
    {
        void Add(Issue issue);
        void Update(Issue issue);
        void Delete(int Id);
        Issue? GetById(int Id);
        List<Issue> GetAll();
    }
}
