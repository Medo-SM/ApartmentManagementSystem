using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class IssueDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Apartment ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Apartment ID must be a positive number.")]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Tenant ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tenant ID must be a positive number.")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(300,MinimumLength = 1,  ErrorMessage = "Description cannot exceed 300 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Open|In Progress|Resolved)$",
         ErrorMessage = "Status must be one of: Open, In Progress, Resolved.")]
        public string Status { get; set; } = "Open";
        public DateTime? ResolvedDate { get; set; }
    }
}
