using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string FullName => $"{Name} {Surname}";
    public string Email { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}