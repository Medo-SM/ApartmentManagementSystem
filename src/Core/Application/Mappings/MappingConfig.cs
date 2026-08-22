using Application.DTOs;
using ApartmentManagement.Core.Domain.Entities;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Apartment, ApartmentDto>().ReverseMap();
            CreateMap<Tenant, TenantDto>().ReverseMap();
            CreateMap<Issue, IssueDto>().ReverseMap();
            CreateMap<Parcel, ParcelDto>().ReverseMap();
            CreateMap<PaymentRecord, PaymentRecordDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
