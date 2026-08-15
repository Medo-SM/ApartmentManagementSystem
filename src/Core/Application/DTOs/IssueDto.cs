using System;

namespace Application.DTOs
{
    public class IssueDto
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public int TenantId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public DateTime? ResolvedDate { get; set; }
    }
}
