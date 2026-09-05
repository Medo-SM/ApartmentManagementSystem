using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ApartmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Unit number is required.")]
        [StringLength(4, ErrorMessage = "Unit number must be between 4 characters like A101.")]
        public string UnitNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Floor number is required.")]
        [Range(1, 20, ErrorMessage = "Floor number must be between 1 and 20.")]
        public int FloorNumber { get; set; }

        [Range(1, 25, ErrorMessage = "Number of rooms must be between 1 and 25.")]
        public int NumberOfRooms { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Rent must be a positive number.")]
        public decimal MonthlyRent { get; set; } = 0.0m;

        [Required(ErrorMessage = "Occupancy status is required.")]
        [RegularExpression("^(Vacant|Occupied|Maintenance)$",
         ErrorMessage = "OccupancyStatus must be one of: Vacant, Occupied, Maintenance.")]
        public string OccupancyStatus { get; set; } = "Vacant";

        [Range(1, int.MaxValue, ErrorMessage = "Current tenant ID must be a positive number.")]
        public int? CurrentTenantId { get; set; } = null;
    }
}
