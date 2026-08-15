using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IParcelService
    {
        void CreateParcel(ParcelDto parcelDto);
        ParcelDto? GetParcelById(int id);
        IEnumerable<ParcelDto> GetAllParcels();
        void UpdateParcel(ParcelDto parcelDto);
        void DeleteParcel(int id);
    }
}
