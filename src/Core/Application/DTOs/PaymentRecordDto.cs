using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class PaymentRecordDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tenant ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tenant ID must be a positive number.")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Apartment ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Apartment ID must be a positive number.")]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Amount paid is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount paid must be a positive number.")]
        public decimal AmountPaid { get; set; }

        [Range(1,12, ErrorMessage = "Payment period month must be a valid month (1-12).")]
        public int PaymentPeriodMonth { get; set; }

        [Range(2000, 2100, ErrorMessage = "Payment period year must be a valid year from 2000 to 2100.")]
        public int PaymentPeriodYear { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Paid|Partial|Pending)$",
         ErrorMessage = "Status must be one of: Paid, Partial, Pending.")]
        public string Status { get; set; } = "Paid";
    }
}
