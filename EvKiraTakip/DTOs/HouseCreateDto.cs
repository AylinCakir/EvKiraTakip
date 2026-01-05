using EvKiraTakip.Models;

namespace EvKiraTakip.DTOs;

public class HouseCreateDto
{
    public string Title { get; set; }
    public string Address { get; set; }
    public int UserId { get; set; }
}