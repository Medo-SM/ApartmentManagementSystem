using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.Repository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class IssueServiceImpl : IIssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IMapper _mapper;

        public IssueServiceImpl(IIssueRepository issueRepository, IMapper mapper)
        {
            _issueRepository = issueRepository;
            _mapper = mapper;
        }

        public void CreateIssue(IssueDto issueDto)
        {
            if (issueDto == null) throw new ArgumentNullException(nameof(issueDto));
            var issueEntity = _mapper.Map<Issue>(issueDto);
            _issueRepository.Add(issueEntity);
        }

        public IssueDto? GetIssueById(int id)
        {
            var issueEntity = _issueRepository.GetById(id);
            if (issueEntity == null) return null;
            return _mapper.Map<IssueDto>(issueEntity);
        }

        public IEnumerable<IssueDto> GetAllIssues()
        {
            var issueEntities = _issueRepository.GetAll();
            return _mapper.Map<IEnumerable<IssueDto>>(issueEntities);
        }

        public void UpdateIssue(IssueDto issueDto)
        {
            if (issueDto == null) throw new ArgumentNullException(nameof(issueDto));
            var issueEntity = _mapper.Map<Issue>(issueDto);
            _issueRepository.Update(issueEntity);
        }

        public void DeleteIssue(int id)
        {
            _issueRepository.Delete(id);
        }
    }
}
