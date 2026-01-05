namespace EvKiraTakip.DTOs;

public class HouseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    
    public List<TenantResponseDto> Tenants { get; set; }

}