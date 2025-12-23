using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class House
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Address  { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}