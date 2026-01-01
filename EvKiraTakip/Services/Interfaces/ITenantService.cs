using EvKiraTakip.DTOs;

namespace EvKiraTakip.Services.Interfaces;

public interface ITenantService
{
    Task<List<TenantResponseDto>> GetAllTenantAsync();
    Task<TenantResponseDto?> GetTenantByIdAsync(int id);
    Task<TenantResponseDto?> CreateTenantAsync(TenantCreateDto dto);
    Task<bool> UpdateTenantAsync(int id, TenantUpdateDto dto);
    Task<bool> DeleteTenantAsync(int id);
}