using EvKiraTakip.Common;
using EvKiraTakip.DTOs;

namespace EvKiraTakip.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUserAsync();
    Task<UserResponseDto?> GetUserByIdAsync (int id);
    Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
    Task<bool> UpdateAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}