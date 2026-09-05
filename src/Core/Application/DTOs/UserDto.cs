using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Role ID must be a positive number.")]
        public int RoleId { get; set; }

        public int? TenantId { get; set; } = null;
        public bool IsActive { get; set; } = false;
    }
}
