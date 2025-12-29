using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class House
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string Address { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}