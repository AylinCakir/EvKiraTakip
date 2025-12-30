using EvKiraTakip.DTOs;

namespace EvKiraTakip.Services.Interfaces;

public interface IRentPaymentService
{
    Task<List<RentPaymentResponseDto>> GetAllPaymentAsync();
    Task<RentPaymentResponseDto?> GetPaymentByIdAsync(int id);
    Task<RentPaymentResponseDto> CreatePaymentAsync(RentPaymentCreateDto dto);
    Task<bool> UpdatePaymentAsync(int id, RentPaymentsUpdateDto dto);
    Task<bool> DeletePaymentAsync(int id);
}