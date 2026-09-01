using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.IRepository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserServiceImpl(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public void CreateUser(UserDto userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));
            var userEntity = _mapper.Map<User>(userDto);
            _userRepository.Add(userEntity);
        }

        public UserDto? GetUserById(int id)
        {
            var userEntity = _userRepository.GetById(id);
            if (userEntity == null) return null;
            return _mapper.Map<UserDto>(userEntity);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var userEntities = _userRepository.GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(userEntities);
        }

        public void UpdateUser(UserDto userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));
            var userEntity = _mapper.Map<User>(userDto);
            _userRepository.Update(userEntity);
        }

        public void DeleteUser(int id)
        {
            _userRepository.Delete(id);
        }
    }
}
