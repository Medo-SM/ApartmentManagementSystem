using System;

namespace Application.DTOs
{
    public class ParcelDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string? CourierName { get; set; }
        public DateTime? PickupTimestamp { get; set; }
        public string Status { get; set; } = "Pending Pickup";
    }
}
