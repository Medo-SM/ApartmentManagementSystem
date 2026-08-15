using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IUserService
    {
        void CreateUser(UserDto userDto);
        UserDto? GetUserById(int id);
        IEnumerable<UserDto> GetAllUsers();
        void UpdateUser(UserDto userDto);
        void DeleteUser(int id);
    }
}
