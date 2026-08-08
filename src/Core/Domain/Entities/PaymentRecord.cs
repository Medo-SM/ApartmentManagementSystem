using ApartmentManagementSystem.Core.Domain.Entites.Base;
using ApartmentManagementSystem.Core.Domain.Entities;


namespace ApartmentManagement.Core.Domain.Entities
{
public class PaymentRecord : BaseEntity
{

    // Foreign Key properties
    public int TenantId { get; set; }
    public int ApartmentId { get; set; }

    public decimal AmountPaid { get; set; }
    public int PaymentPeriodMonth { get; set; }
    public int PaymentPeriodYear { get; set; }
    public string Status { get; set; } = "Paid";

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Apartment Apartment { get; set; } = null!;
}
}