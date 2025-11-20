using System.ComponentModel.DataAnnotations.Schema;

namespace EvKiraTakip.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    
}