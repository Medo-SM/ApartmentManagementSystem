using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class TenantDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;
        

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^7\\d{8}$", ErrorMessage = "Phone number must be a valid 9-digit Yemeni number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression("^7\\d{8}$", ErrorMessage = "Emergency contact must be a valid 9-digit Yemeni number.")]
        public string? EmergencyContact { get; set; } = null;
    }
}
