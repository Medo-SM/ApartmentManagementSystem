using Application.DTOs;
using ApartmentManagement.Core.Domain.Entities;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Tests.Fixtures;
using Xunit;

namespace Tests.Mapping
{
    public class MappingTests
    {
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _mapper = TestFixtures.CreateMapper();
        }

        [Fact]
        public void User_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidUserEntity();

            var dto = _mapper.Map<UserDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Username, dto.Username);
            Assert.Equal(entity.Email, dto.Email);
            Assert.Equal(entity.RoleId, dto.RoleId);
            Assert.Equal(entity.TenantId, dto.TenantId);
            Assert.Equal(entity.IsActive, dto.IsActive);

            var mappedEntity = _mapper.Map<User>(dto);
            Assert.Equal(dto.Username, mappedEntity.Username);
            Assert.Equal(dto.Email, mappedEntity.Email);
            Assert.Equal(dto.RoleId, mappedEntity.RoleId);
            Assert.Equal(dto.TenantId, mappedEntity.TenantId);
            Assert.Equal(dto.IsActive, mappedEntity.IsActive);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void Apartment_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidApartmentEntity();

            var dto = _mapper.Map<ApartmentDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.UnitNumber, dto.UnitNumber);
            Assert.Equal(entity.FloorNumber, dto.FloorNumber);
            Assert.Equal(entity.NumberOfRooms, dto.NumberOfRooms);
            Assert.Equal(entity.MonthlyRent, dto.MonthlyRent);
            Assert.Equal(entity.OccupancyStatus, dto.OccupancyStatus);
            Assert.Equal(entity.CurrentTenantId, dto.CurrentTenantId);

            var mappedEntity = _mapper.Map<Apartment>(dto);
            Assert.Equal(dto.UnitNumber, mappedEntity.UnitNumber);
            Assert.Equal(dto.FloorNumber, mappedEntity.FloorNumber);
            Assert.Equal(dto.NumberOfRooms, mappedEntity.NumberOfRooms);
            Assert.Equal(dto.MonthlyRent, mappedEntity.MonthlyRent);
            Assert.Equal(dto.OccupancyStatus, mappedEntity.OccupancyStatus);
            Assert.Equal(dto.CurrentTenantId, mappedEntity.CurrentTenantId);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void Tenant_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidTenantEntity();

            var dto = _mapper.Map<TenantDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.FullName, dto.FullName);
            Assert.Equal(entity.PhoneNumber, dto.PhoneNumber);
            Assert.Equal(entity.EmergencyContact, dto.EmergencyContact);

            var mappedEntity = _mapper.Map<Tenant>(dto);
            Assert.Equal(dto.FullName, mappedEntity.FullName);
            Assert.Equal(dto.PhoneNumber, mappedEntity.PhoneNumber);
            Assert.Equal(dto.EmergencyContact, mappedEntity.EmergencyContact);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void PaymentRecord_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidPaymentRecordEntity();

            var dto = _mapper.Map<PaymentRecordDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.TenantId, dto.TenantId);
            Assert.Equal(entity.ApartmentId, dto.ApartmentId);
            Assert.Equal(entity.AmountPaid, dto.AmountPaid);
            Assert.Equal(entity.PaymentPeriodMonth, dto.PaymentPeriodMonth);
            Assert.Equal(entity.PaymentPeriodYear, dto.PaymentPeriodYear);
            Assert.Equal(entity.Status, dto.Status);

            var mappedEntity = _mapper.Map<PaymentRecord>(dto);
            Assert.Equal(dto.TenantId, mappedEntity.TenantId);
            Assert.Equal(dto.ApartmentId, mappedEntity.ApartmentId);
            Assert.Equal(dto.AmountPaid, mappedEntity.AmountPaid);
            Assert.Equal(dto.PaymentPeriodMonth, mappedEntity.PaymentPeriodMonth);
            Assert.Equal(dto.PaymentPeriodYear, mappedEntity.PaymentPeriodYear);
            Assert.Equal(dto.Status, mappedEntity.Status);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void Issue_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidIssueEntity();

            var dto = _mapper.Map<IssueDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.ApartmentId, dto.ApartmentId);
            Assert.Equal(entity.TenantId, dto.TenantId);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.Status, dto.Status);
            Assert.Equal(entity.ResolvedDate, dto.ResolvedDate);

            var mappedEntity = _mapper.Map<Issue>(dto);
            Assert.Equal(dto.ApartmentId, mappedEntity.ApartmentId);
            Assert.Equal(dto.TenantId, mappedEntity.TenantId);
            Assert.Equal(dto.Description, mappedEntity.Description);
            Assert.Equal(dto.Status, mappedEntity.Status);
            Assert.Equal(dto.ResolvedDate, mappedEntity.ResolvedDate);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void Parcel_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidParcelEntity();

            var dto = _mapper.Map<ParcelDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.TenantId, dto.TenantId);
            Assert.Equal(entity.CourierName, dto.CourierName);
            Assert.Equal(entity.PickupTimestamp, dto.PickupTimestamp);
            Assert.Equal(entity.Status, dto.Status);

            var mappedEntity = _mapper.Map<Parcel>(dto);
            Assert.Equal(dto.TenantId, mappedEntity.TenantId);
            Assert.Equal(dto.CourierName, mappedEntity.CourierName);
            Assert.Equal(dto.PickupTimestamp, mappedEntity.PickupTimestamp);
            Assert.Equal(dto.Status, mappedEntity.Status);
            Assert.Equal(0, mappedEntity.Id);
        }

        [Fact]
        public void Role_RoundTrip_PreservesValues_AndIgnoresIdOnDtoToEntity()
        {
            var entity = TestFixtures.CreateValidRoleEntity();

            var dto = _mapper.Map<RoleDto>(entity);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.RoleName, dto.RoleName);

            var mappedEntity = _mapper.Map<Role>(dto);
            Assert.Equal(dto.RoleName, mappedEntity.RoleName);
            Assert.Equal(0, mappedEntity.Id);
        }
    }
}