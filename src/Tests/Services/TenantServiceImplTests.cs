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
    public class TenantServiceImplTests
    {
        private readonly Mock<ITenantRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly TenantServiceImpl _service;

        public TenantServiceImplTests()
        {
            _mockRepo = new Mock<ITenantRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new TenantServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateTenant_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidTenantDto();

            _service.CreateTenant(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<Tenant>()), Times.Once);
        }

        [Fact]
        public void CreateTenant_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateTenant(null));
        }

        [Fact]
        public void GetTenantById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidTenantEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetTenantById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.FullName, result.FullName);
            Assert.Equal(entity.PhoneNumber, result.PhoneNumber);
            Assert.Equal(entity.EmergencyContact, result.EmergencyContact);
        }

        [Fact]
        public void GetTenantById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Tenant)null);

            var result = _service.GetTenantById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllTenants_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateTenantList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllTenants().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].FullName, result[0].FullName);
            Assert.Equal(entities[1].PhoneNumber, result[1].PhoneNumber);
        }

        [Fact]
        public void UpdateTenant_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidTenantDto();

            _service.UpdateTenant(dto);

            _mockRepo.Verify(r => r.Update(It.Is<Tenant>(t =>
                t.Id == dto.Id &&
                t.FullName == dto.FullName &&
                t.PhoneNumber == dto.PhoneNumber &&
                t.EmergencyContact == dto.EmergencyContact)), Times.Once);
        }

        [Fact]
        public void UpdateTenant_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateTenant(null));
        }

        [Fact]
        public void DeleteTenant_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteTenant(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}