namespace ApartmentManagement.Core.Domain.Entities;

public class PaymentRecord
{
    public int Id { get; set; }

    // Foreign Key properties
    public int TenantId { get; set; }
    public int ApartmentId { get; set; }

    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public int PaymentPeriodMonth { get; set; }
    public int PaymentPeriodYear { get; set; }
    public string Status { get; set; } = "Paid";

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Apartment Apartment { get; set; } = null!;
}
