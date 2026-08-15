using System;

namespace Application.DTOs
{
    public class ApartmentDto
    {
        public int Id { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal MonthlyRent { get; set; }
        public string OccupancyStatus { get; set; } = "Vacant";
        public int? CurrentTenantId { get; set; }
    }
}
