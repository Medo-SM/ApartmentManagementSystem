using Application.DTOs;
using Application.Interfaces;
using ApartmentManagementSystem.Core.Domain.Entities;
using AutoMapper;
using Domain.Repository;
using System;
using System.Collections.Generic;

namespace Application.ServiceImpl
{
    public class ParcelServiceImpl : IParcelService
    {
        private readonly IParcelRepository _parcelRepository;
        private readonly IMapper _mapper;

        public ParcelServiceImpl(IParcelRepository parcelRepository, IMapper mapper)
        {
            _parcelRepository = parcelRepository;
            _mapper = mapper;
        }

        public void CreateParcel(ParcelDto parcelDto)
        {
            if (parcelDto == null) throw new ArgumentNullException(nameof(parcelDto));
            var parcelEntity = _mapper.Map<Parcel>(parcelDto);
            _parcelRepository.Add(parcelEntity);
        }

        public ParcelDto? GetParcelById(int id)
        {
            var parcelEntity = _parcelRepository.GetById(id);
            if (parcelEntity == null) return null;
            return _mapper.Map<ParcelDto>(parcelEntity);
        }

        public IEnumerable<ParcelDto> GetAllParcels()
        {
            var parcelEntities = _parcelRepository.GetAll();
            return _mapper.Map<IEnumerable<ParcelDto>>(parcelEntities);
        }

        public void UpdateParcel(ParcelDto parcelDto)
        {
            if (parcelDto == null) throw new ArgumentNullException(nameof(parcelDto));
            var parcelEntity = _mapper.Map<Parcel>(parcelDto);
            _parcelRepository.Update(parcelEntity);
        }

        public void DeleteParcel(int id)
        {
            _parcelRepository.Delete(id);
        }
    }
}
