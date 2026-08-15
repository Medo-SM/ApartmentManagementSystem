using System;

namespace Application.DTOs
{
    public class PaymentRecordDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ApartmentId { get; set; }
        public decimal AmountPaid { get; set; }
        public int PaymentPeriodMonth { get; set; }
        public int PaymentPeriodYear { get; set; }
        public string Status { get; set; } = "Paid";
    }
}
