using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IApartmentService
    {
        void CreateApartment(ApartmentDto apartmentDto);
        ApartmentDto? GetApartmentById(int id);
        IEnumerable<ApartmentDto> GetAllApartments();
        void UpdateApartment(ApartmentDto apartmentDto);
        void DeleteApartment(int id);
    }
}
