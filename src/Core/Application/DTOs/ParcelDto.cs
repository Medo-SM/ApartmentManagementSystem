using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ParcelDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tenant ID must be a positive number.")]
        public int TenantId { get; set; }

        [StringLength(100, ErrorMessage = "Courier name cannot exceed 100 characters.")]
        public string? CourierName { get; set; }

        public DateTime? PickupTimestamp { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending Pickup|Picked Up)$", ErrorMessage = "Status must be one of: 'Pending Pickup', 'Picked Up'.")]
        public string Status { get; set; } = "Pending Pickup";
    }
}
