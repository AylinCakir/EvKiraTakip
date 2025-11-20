using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;


public class Tenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime StartDate { get; set; }
    
    public int PropertyId { get; set; }
    public Property Property { get; set; }
    
}

