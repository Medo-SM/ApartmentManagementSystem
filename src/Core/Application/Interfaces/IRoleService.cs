using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        void CreateRole(RoleDto roleDto);
        RoleDto? GetRoleById(int id);
        IEnumerable<RoleDto> GetAllRoles();
        void UpdateRole(RoleDto roleDto);
        void DeleteRole(int id);
    }
}
