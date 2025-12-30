using EvKiraTakip.DTOs;

namespace EvKiraTakip.Services.Interfaces;

public interface IHouseService
{
    Task<List<HouseResponseDto>> GetAllHouseAsync();
    Task<HouseResponseDto?> GetHouseByIdAsync(int id);
    Task<HouseResponseDto> CreateHouseAsync(HouseCreateDto dto);
    Task<bool> UpdateHouseAsync(int id, HouseUpdateDto dto);
    Task<bool> DeleteHouseAsync(int id);
}