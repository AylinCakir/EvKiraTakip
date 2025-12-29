namespace EvKiraTakip.DTOs;

public class TenantResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    
    public List<RentPaymentResponseDto> RentPayments { get; set; }
}