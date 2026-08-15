using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IIssueService
    {
        void CreateIssue(IssueDto issueDto);
        IssueDto? GetIssueById(int id);
        IEnumerable<IssueDto> GetAllIssues();
        void UpdateIssue(IssueDto issueDto);
        void DeleteIssue(int id);
    }
}
