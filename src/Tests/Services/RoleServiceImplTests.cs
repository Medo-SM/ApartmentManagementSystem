using System;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs;
using Application.ServiceImpl;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.IRepository;
using Moq;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class RoleServiceImplTests
    {
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly RoleServiceImpl _service;

        public RoleServiceImplTests()
        {
            _mockRepo = new Mock<IRoleRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new RoleServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateRole_ValidDto_SeedsMissingDefaults()
        {
            var existing = new List<Role> { new Role { RoleName = "tenant" } };
            _mockRepo.Setup(r => r.GetAll()).Returns(existing);
            var dto = new RoleDto { Id = 9, RoleName = "Admin" };

            _service.CreateRole(dto);

            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Building Owner")), Times.Once);
            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Building Manager")), Times.Once);
            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Tenant")), Times.Never);
            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Admin")), Times.Once);
            _mockRepo.Verify(r => r.Add(It.IsAny<Role>()), Times.Exactly(3));
        }

        [Fact]
        public void CreateRole_AllDefaultsExist_OnlyAddsRequestedRole()
        {
            var existing = new List<Role>
            {
                new Role { RoleName = "Building Owner" },
                new Role { RoleName = "Building Manager" },
                new Role { RoleName = "Tenant" }
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(existing);
            var dto = new RoleDto { Id = 9, RoleName = "Admin" };

            _service.CreateRole(dto);

            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Building Owner")), Times.Never);
            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Building Manager")), Times.Never);
            _mockRepo.Verify(r => r.Add(It.Is<Role>(role => role.RoleName == "Tenant")), Times.Never);
            _mockRepo.Verify(r => r.Add(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public void CreateRole_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateRole(null));
        }

        [Fact]
        public void GetRoleById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidRoleEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetRoleById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.RoleName, result.RoleName);
        }

        [Fact]
        public void GetRoleById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Role)null);

            var result = _service.GetRoleById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllRoles_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateRoleList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllRoles().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].RoleName, result[0].RoleName);
            Assert.Equal(entities[1].RoleName, result[1].RoleName);
        }

        [Fact]
        public void UpdateRole_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidRoleDto();

            _service.UpdateRole(dto);

            _mockRepo.Verify(r => r.Update(It.Is<Role>(role =>
                role.Id == dto.Id &&
                role.RoleName == dto.RoleName)), Times.Once);
        }

        [Fact]
        public void UpdateRole_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateRole(null));
        }

        [Fact]
        public void DeleteRole_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteRole(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}