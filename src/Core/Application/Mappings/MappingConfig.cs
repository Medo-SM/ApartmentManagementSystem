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

            // Make the IDs not mapped when mapping from DTO to Entity
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ApartmentDto, Apartment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<TenantDto, Tenant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<IssueDto, Issue>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ParcelDto, Parcel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<PaymentRecordDto, PaymentRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());           
        }
    }
}
