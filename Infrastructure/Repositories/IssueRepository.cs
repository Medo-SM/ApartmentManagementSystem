using System.Collections.Generic;
using System.Linq;
using ApartmentManagementSystem.Core.Domain.Entities;
using Domain.Repository;
using Infrastructure.Data;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly AppDbContext _context;

        public IssueRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Issue issue)
        {
            _context.Issues.Add(issue);
            _context.SaveChanges();
        }

        public void Update(Issue issue)
        {
            _context.Issues.Update(issue);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var issue = _context.Issues.Find(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
                _context.SaveChanges();
            }
        }

        public Issue? GetById(int id)
        {
            return _context.Issues.Find(id);
        }

        public List<Issue> GetAll()
        {
            return _context.Issues.ToList();
        }
    }
}