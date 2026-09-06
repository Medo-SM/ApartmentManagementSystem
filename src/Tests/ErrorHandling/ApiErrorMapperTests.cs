using System;
using Application.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.ErrorHandling
{
    public class ApiErrorMapperTests
    {
        [Fact]
        public void DuplicateApartmentUnitNumber_MapsTo409_WithSpecificMessage()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("Cannot insert duplicate key row in object 'dbo.Apartments' with unique index 'IX_Apartments_UnitNumber'. The duplicate key value is (A101)."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("An apartment with this Unit Number already exists.", result.Message);
        }

        [Fact]
        public void DuplicateUserUsername_MapsTo409_WithSpecificMessage()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Username'. The duplicate key value is (jdoe)."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("A user with this Username already exists.", result.Message);
        }

        [Fact]
        public void DuplicateUserEmail_MapsTo409_WithSpecificMessage()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("Cannot insert duplicate key row in object 'dbo.Users' with unique constraint 'IX_Users_Email'. The duplicate key value is (jdoe@example.com)."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("A user with this Email already exists.", result.Message);
        }

        [Fact]
        public void DuplicateUserTenantLink_MapsTo409_WithSpecificMessage()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_TenantId'. The duplicate key value is (3)."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("A user is already linked to this Tenant.", result.Message);
        }

        [Fact]
        public void DuplicateRoleName_MapsTo409_WithSpecificMessage()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("Cannot insert duplicate key row in object 'dbo.Roles' with unique index 'IX_Roles_RoleName'. The duplicate key value is (Building Owner)."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("A role with this name already exists.", result.Message);
        }

        [Fact]
        public void Duplicate_UnknownIndex_MapsTo409_WithGenericMessage()
        {
            var ex = new Exception("Cannot insert duplicate key row in object 'dbo.SomeTable' with unique index 'IX_Foo_Bar'. The duplicate key value is (X).");

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("A record with the same unique value already exists.", result.Message);
        }

        [Fact]
        public void ForeignKeyViolation_MapsTo400()
        {
            var ex = new DbUpdateException("An error occurred while saving the entity changes.",
                new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_PaymentRecords_Apartments_ApartmentId\". The conflict occurred in database \"ApartmentManagementDB\", table \"dbo.Apartments\", column 'Id'."));

            var result = ApiErrorMapper.TryMap(ex);

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("One or more referenced records do not exist. Please check the supplied Tenant/Apartment/Role IDs.", result.Message);
        }

        [Fact]
        public void UnrelatedException_ReturnsNull()
        {
            var ex = new InvalidOperationException("Object reference not set to an instance of an object.");

            var result = ApiErrorMapper.TryMap(ex);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(null)]
        public void NullException_ReturnsNull(Exception ex)
        {
            var result = ApiErrorMapper.TryMap(ex);

            Assert.Null(result);
        }
    }
}