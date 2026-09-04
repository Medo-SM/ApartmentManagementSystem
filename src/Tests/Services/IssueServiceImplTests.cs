using System;
using System.Linq;
using Application.ServiceImpl;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.IRepository;
using Moq;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class IssueServiceImplTests
    {
        private readonly Mock<IIssueRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly IssueServiceImpl _service;

        public IssueServiceImplTests()
        {
            _mockRepo = new Mock<IIssueRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new IssueServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateIssue_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidIssueDto();

            _service.CreateIssue(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<Issue>()), Times.Once);
        }

        [Fact]
        public void CreateIssue_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateIssue(null));
        }

        [Fact]
        public void GetIssueById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidIssueEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetIssueById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.ApartmentId, result.ApartmentId);
            Assert.Equal(entity.TenantId, result.TenantId);
            Assert.Equal(entity.Description, result.Description);
            Assert.Equal(entity.Status, result.Status);
            Assert.Equal(entity.ResolvedDate, result.ResolvedDate);
        }

        [Fact]
        public void GetIssueById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Issue)null);

            var result = _service.GetIssueById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllIssues_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateIssueList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllIssues().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].Description, result[0].Description);
            Assert.Equal(entities[1].Status, result[1].Status);
        }

        [Fact]
        public void UpdateIssue_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidIssueDto();

            _service.UpdateIssue(dto);

            _mockRepo.Verify(r => r.Update(It.Is<Issue>(i =>
                i.Id == dto.Id &&
                i.ApartmentId == dto.ApartmentId &&
                i.TenantId == dto.TenantId &&
                i.Description == dto.Description &&
                i.Status == dto.Status &&
                i.ResolvedDate == dto.ResolvedDate)), Times.Once);
        }

        [Fact]
        public void UpdateIssue_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateIssue(null));
        }

        [Fact]
        public void DeleteIssue_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteIssue(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}