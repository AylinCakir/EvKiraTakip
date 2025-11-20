using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;


public class Payment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int PropertyId { get; set; }
    public Property Property { get; set; }
    
    public string Month { get; set; }
    public decimal Amount { get; set; }
    
    public bool IsPaid { get; set; }
    public DateTime DatePaid { get; set; }
    
}