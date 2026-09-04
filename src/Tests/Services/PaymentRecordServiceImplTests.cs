using System;
using System.Linq;
using ApartmentManagement.Core.Domain.Entities;
using Application.ServiceImpl;
using AutoMapper;
using Domain.IRepository;
using Moq;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class PaymentRecordServiceImplTests
    {
        private readonly Mock<IPaymentRecordRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly PaymentRecordServiceImpl _service;

        public PaymentRecordServiceImplTests()
        {
            _mockRepo = new Mock<IPaymentRecordRepository>();
            _mapper = TestFixtures.CreateMapper();
            _service = new PaymentRecordServiceImpl(_mockRepo.Object, _mapper);
        }

        [Fact]
        public void CreatePaymentRecord_ValidDto_CallsRepositoryAdd()
        {
            var dto = TestFixtures.CreateValidPaymentRecordDto();

            _service.CreatePaymentRecord(dto);

            _mockRepo.Verify(r => r.Add(It.IsAny<PaymentRecord>()), Times.Once);
        }

        [Fact]
        public void CreatePaymentRecord_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.CreatePaymentRecord(null));
        }

        [Fact]
        public void GetPaymentRecordById_ExistingId_ReturnsDto()
        {
            var entity = TestFixtures.CreateValidPaymentRecordEntity();
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(entity);

            var result = _service.GetPaymentRecordById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.TenantId, result.TenantId);
            Assert.Equal(entity.ApartmentId, result.ApartmentId);
            Assert.Equal(entity.AmountPaid, result.AmountPaid);
            Assert.Equal(entity.PaymentPeriodMonth, result.PaymentPeriodMonth);
            Assert.Equal(entity.PaymentPeriodYear, result.PaymentPeriodYear);
            Assert.Equal(entity.Status, result.Status);
        }

        [Fact]
        public void GetPaymentRecordById_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((PaymentRecord)null);

            var result = _service.GetPaymentRecordById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllPaymentRecords_ReturnsAllDtos()
        {
            var entities = TestFixtures.CreatePaymentRecordList();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);

            var result = _service.GetAllPaymentRecords().ToList();

            Assert.Equal(entities.Count, result.Count);
            Assert.Equal(entities[0].AmountPaid, result[0].AmountPaid);
            Assert.Equal(entities[1].Status, result[1].Status);
        }

        [Fact]
        public void UpdatePaymentRecord_ValidDto_CallsRepositoryUpdate()
        {
            var dto = TestFixtures.CreateValidPaymentRecordDto();

            _service.UpdatePaymentRecord(dto);

            _mockRepo.Verify(r => r.Update(It.Is<PaymentRecord>(p =>
                p.Id == dto.Id &&
                p.TenantId == dto.TenantId &&
                p.ApartmentId == dto.ApartmentId &&
                p.AmountPaid == dto.AmountPaid &&
                p.PaymentPeriodMonth == dto.PaymentPeriodMonth &&
                p.PaymentPeriodYear == dto.PaymentPeriodYear &&
                p.Status == dto.Status)), Times.Once);
        }

        [Fact]
        public void UpdatePaymentRecord_NullDto_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.UpdatePaymentRecord(null));
        }

        [Fact]
        public void DeletePaymentRecord_ValidId_CallsRepositoryDelete()
        {
            _service.DeletePaymentRecord(7);

            _mockRepo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}