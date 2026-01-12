namespace EvKiraTakip.DTOs;

public class UserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public List<HouseResponseDto> Houses  { get; set; }
  
}