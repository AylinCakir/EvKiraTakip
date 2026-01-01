using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class Tenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }

    public int HouseId { get; set; }
    
    public House House { get; set; }
    
    public ICollection<RentPayment> RentPayments { get; set; } = new List<RentPayment>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; } = false;
}