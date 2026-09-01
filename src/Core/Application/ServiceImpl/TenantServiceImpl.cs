using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.IRepository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class TenantServiceImpl : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public TenantServiceImpl(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public void CreateTenant(TenantDto tenantDto)
        {
            if (tenantDto == null) throw new ArgumentNullException(nameof(tenantDto));
            var tenantEntity = _mapper.Map<Tenant>(tenantDto);
            _tenantRepository.Add(tenantEntity);
        }

        public TenantDto? GetTenantById(int id)
        {
            var tenantEntity = _tenantRepository.GetById(id);
            if (tenantEntity == null) return null;
            return _mapper.Map<TenantDto>(tenantEntity);
        }

        public IEnumerable<TenantDto> GetAllTenants()
        {
            var tenantEntities = _tenantRepository.GetAll();
            return _mapper.Map<IEnumerable<TenantDto>>(tenantEntities);
        }

        public void UpdateTenant(TenantDto tenantDto)
        {
            if (tenantDto == null) throw new ArgumentNullException(nameof(tenantDto));
            var tenantEntity = _mapper.Map<Tenant>(tenantDto);
            tenantEntity.Id = tenantDto.Id; // Ensure the ID is set for the update
            _tenantRepository.Update(tenantEntity);
        }

        public void DeleteTenant(int id)
        {
            _tenantRepository.Delete(id);
        }
    }
}
