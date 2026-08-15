using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface ITenantService
    {
        void CreateTenant(TenantDto tenantDto);
        TenantDto? GetTenantById(int id);
        IEnumerable<TenantDto> GetAllTenants();
        void UpdateTenant(TenantDto tenantDto);
        void DeleteTenant(int id);
    }
}
