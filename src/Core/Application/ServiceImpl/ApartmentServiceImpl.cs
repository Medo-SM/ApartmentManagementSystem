using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.Repository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class ApartmentServiceImpl : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IMapper _mapper;

        public ApartmentServiceImpl(IApartmentRepository apartmentRepository, IMapper mapper)
        {
            _apartmentRepository = apartmentRepository;
            _mapper = mapper;
        }

        public void CreateApartment(ApartmentDto apartmentDto)
        {
            if (apartmentDto == null) throw new ArgumentNullException(nameof(apartmentDto));
            var apartmentEntity = _mapper.Map<Apartment>(apartmentDto);
            _apartmentRepository.Add(apartmentEntity);
        }

        public ApartmentDto? GetApartmentById(int id)
        {
            var apartmentEntity = _apartmentRepository.GetById(id);
            if (apartmentEntity == null) return null;
            return _mapper.Map<ApartmentDto>(apartmentEntity);
        }

        public IEnumerable<ApartmentDto> GetAllApartments()
        {
            var apartmentEntities = _apartmentRepository.GetAll();
            return _mapper.Map<IEnumerable<ApartmentDto>>(apartmentEntities);
        }

        public void UpdateApartment(ApartmentDto apartmentDto)
        {
            if (apartmentDto == null) throw new ArgumentNullException(nameof(apartmentDto));
            var apartmentEntity = _mapper.Map<Apartment>(apartmentDto);
            _apartmentRepository.Update(apartmentEntity);
        }

        public void DeleteApartment(int id)
        {
            _apartmentRepository.Delete(id);
        }
    }
}
