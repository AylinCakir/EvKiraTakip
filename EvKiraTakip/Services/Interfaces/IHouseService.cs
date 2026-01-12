using EvKiraTakip.DTOs;
using EvKiraTakip.Enums;

namespace EvKiraTakip.Services.Interfaces;

public interface IHouseService
{
    Task<List<HouseResponseDto>> GetAllHouseAsync();
    Task<HouseResponseDto?> GetHouseByIdAsync(int id);
    Task<HouseResponseDto?> CreateHouseAsync(HouseCreateDto dto);
    Task<bool> UpdateHouseAsync(int id, HouseUpdateDto dto);
    Task<DeleteHouseResult> DeleteHouseAsync(int id);
}