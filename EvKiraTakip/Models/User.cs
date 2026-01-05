using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string FullName => $"{Name} {Surname}";
    public string Email { get; set; } = null!;
    public int Age { get; set; } 
    public string Address { get; set; } = null!;

    public ICollection<House> Houses { get; set; } = new List<House>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
}