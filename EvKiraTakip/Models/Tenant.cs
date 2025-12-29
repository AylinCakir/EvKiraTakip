using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class Tenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    
    public int HouseId { get; set; }
    
    public House House { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}