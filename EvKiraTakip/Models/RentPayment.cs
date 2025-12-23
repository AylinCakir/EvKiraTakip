using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class RentPayment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    
    public int TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}