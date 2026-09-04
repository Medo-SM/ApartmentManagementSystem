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
    public class ParcelServiceImplTests
    {
        private readonly Mock<IParcelRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ParcelServiceImpl _service;

        public ParcelServiceImplTests()
        {
            _mockRepo = new Mock<IParcelRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new ParcelServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreateParcel_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidParcelDto();

            _service.CreateParcel(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<Parcel>()), Times.Once);
        }

        [Fact]
        public void CreateParcel_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreateParcel(null));
        }

        [Fact]
        public void GetParcelById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidParcelEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetParcelById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.TenantId, result.TenantId);
            Assert.Equal(entity.CourierName, result.CourierName);
            Assert.Equal(entity.PickupTimestamp, result.PickupTimestamp);
            Assert.Equal(entity.Status, result.Status);
        }

        [Fact]
        public void GetParcelById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Parcel)null);

            var result = _service.GetParcelById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllParcels_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreateParcelList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllParcels().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].CourierName, result[0].CourierName);
            Assert.Equal(entities[1].Status, result[1].Status);
        }

        [Fact]
        public void UpdateParcel_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidParcelDto();

            _service.UpdateParcel(dto);

            _mockRepo.Verify(r => r.Update(It.Is<Parcel>(p =>
                p.Id == dto.Id &&
                p.TenantId == dto.TenantId &&
                p.CourierName == dto.CourierName &&
                p.PickupTimestamp == dto.PickupTimestamp &&
                p.Status == dto.Status)), Times.Once);
        }

        [Fact]
        public void UpdateParcel_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdateParcel(null));
        }

        [Fact]
        public void DeleteParcel_ValidId_CallsRepositoryDelete()
        {
            _service.DeleteParcel(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}