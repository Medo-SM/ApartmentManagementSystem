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
    public class UserServiceImplTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly UserServiceImpl _service;

        public UserServiceImplTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new UserServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateUser_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidUserDto();

            _service.CreateUser(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void CreateUser_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateUser(null));
        }

        [Fact]
        public void GetUserById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidUserEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetUserById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Username, result.Username);
            Assert.Equal(entity.Email, result.Email);
            Assert.Equal(entity.RoleId, result.RoleId);
            Assert.Equal(entity.TenantId, result.TenantId);
            Assert.Equal(entity.IsActive, result.IsActive);
        }

        [Fact]
        public void GetUserById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((User)null);

            var result = _service.GetUserById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateUserList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllUsers().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].Username, result[0].Username);
            Assert.Equal(entities[1].Email, result[1].Email);
        }

        [Fact]
        public void UpdateUser_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidUserDto();

            _service.UpdateUser(dto);

            _mockRepo.Verify(r => r.Update(It.Is<User>(u =>
                u.Id == dto.Id &&
                u.Username == dto.Username &&
                u.Email == dto.Email &&
                u.RoleId == dto.RoleId &&
                u.IsActive == dto.IsActive)), Times.Once);
        }

        [Fact]
        public void UpdateUser_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateUser(null));
        }

        [Fact]
        public void DeleteUser_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteUser(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}