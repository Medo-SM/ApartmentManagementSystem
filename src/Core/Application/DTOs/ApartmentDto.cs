using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    class ApartmentDto
    {
        int Id { get; set; }
        string UnitNumber { get; set; }
        int FloorNumber{ get; set; }
        int NumberOfRooms { get; set; }
        decimal MonthlyRent { get; set; }
        string OccupancyStatus { get; set; } = "Vacant";
        int? CurrentTenantId{ get; set; }
    }
}
