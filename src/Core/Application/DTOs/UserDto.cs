using System;

namespace Application.DTOs
{
    public class UserDto

    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int? TenantId { get; set; }
        public bool IsActive { get; set; }

    }
}
