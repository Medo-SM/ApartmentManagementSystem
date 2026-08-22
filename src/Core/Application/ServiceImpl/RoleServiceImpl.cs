using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.Repository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleServiceImpl(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public void CreateRole(RoleDto roleDto)
        {
            if (roleDto == null) throw new ArgumentNullException(nameof(roleDto));
            var roleEntity = _mapper.Map<Role>(roleDto);
            _roleRepository.Add(roleEntity);
        }

        public RoleDto? GetRoleById(int id)
        {
            var roleEntity = _roleRepository.GetById(id);
            if (roleEntity == null) return null;
            return _mapper.Map<RoleDto>(roleEntity);
        }

        public IEnumerable<RoleDto> GetAllRoles()
        {
            var roleEntities = _roleRepository.GetAll();
            return _mapper.Map<IEnumerable<RoleDto>>(roleEntities);
        }

        public void UpdateRole(RoleDto roleDto)
        {
            if (roleDto == null) throw new ArgumentNullException(nameof(roleDto));
            var roleEntity = _mapper.Map<Role>(roleDto);
            _roleRepository.Update(roleEntity);
        }

        public void DeleteRole(int id)
        {
            _roleRepository.Delete(id);
        }
    }
}
