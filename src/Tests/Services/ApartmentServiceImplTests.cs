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
    public class ApartmentServiceImplTests
    {
        private readonly Mock<IApartmentRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ApartmentServiceImpl _service;

        public ApartmentServiceImplTests()
        {
            _mockRepo = new Mock<IApartmentRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new ApartmentServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateApartment_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidApartmentDto();

            _service.CreateApartment(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<Apartment>()), Times.Once);
        }

        [Fact]
        public void CreateApartment_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateApartment(null));
        }

        [Fact]
        public void GetApartmentById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidApartmentEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetApartmentById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.UnitNumber, result.UnitNumber);
            Assert.Equal(entity.FloorNumber, result.FloorNumber);
            Assert.Equal(entity.NumberOfRooms, result.NumberOfRooms);
            Assert.Equal(entity.MonthlyRent, result.MonthlyRent);
            Assert.Equal(entity.OccupancyStatus, result.OccupancyStatus);
            Assert.Equal(entity.CurrentTenantId, result.CurrentTenantId);
        }

        [Fact]
        public void GetApartmentById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Apartment)null);

            var result = _service.GetApartmentById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllApartments_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateApartmentList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllApartments().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].UnitNumber, result[0].UnitNumber);
            Assert.Equal(entities[1].MonthlyRent, result[1].MonthlyRent);
        }

        [Fact]
        public void UpdateApartment_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidApartmentDto();

            _service.UpdateApartment(dto);

            _mockRepo.Verify(r => r.Update(It.Is<Apartment>(a =>
                a.Id == dto.Id &&
                a.UnitNumber == dto.UnitNumber &&
                a.FloorNumber == dto.FloorNumber &&
                a.NumberOfRooms == dto.NumberOfRooms &&
                a.MonthlyRent == dto.MonthlyRent &&
                a.OccupancyStatus == dto.OccupancyStatus &&
                a.CurrentTenantId == dto.CurrentTenantId)), Times.Once);
        }

        [Fact]
        public void UpdateApartment_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateApartment(null));
        }

        [Fact]
        public void DeleteApartment_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteApartment(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}