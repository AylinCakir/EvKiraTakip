using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;


public class Property
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Address { get; set; }
    public decimal RentAmount { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();


}
