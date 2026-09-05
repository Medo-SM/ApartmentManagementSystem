using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 100 characters.")]
        public string RoleName { get; set; } = string.Empty;
    }
}
