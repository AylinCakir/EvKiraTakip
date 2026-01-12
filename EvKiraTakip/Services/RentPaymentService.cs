using EvKiraTakip.DTOs;
using EvKiraTakip.Models;
using EvKiraTakip.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvKiraTakip.Services;

public class RentPaymentService : IRentPaymentService
{
    private readonly AppDbContext _dbContext;
    public RentPaymentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RentPaymentResponseDto>> GetAllPaymentAsync()
    {
        return await  _dbContext.RentPayments
            .Select(r=> new RentPaymentResponseDto 
            {
                Id = r.Id,
                Amount = r.Amount,
                PaymentDate = r.PaymentDate
            }).ToListAsync();
    }

    public async Task<RentPaymentResponseDto?> GetPaymentByIdAsync(int id)
    {
        return await _dbContext.RentPayments
            .Where(r => r.Id == id)
            .Select(r => new RentPaymentResponseDto
            {
                Id = r.Id,
                Amount = r.Amount,
                PaymentDate = r.PaymentDate
            }).FirstOrDefaultAsync();
    }

    public async Task<RentPaymentResponseDto?> CreatePaymentAsync(RentPaymentCreateDto dto)
    {
        var exists = await _dbContext.RentPayments
            .AnyAsync(r => r.TenantId == dto.TenantId 
                           && r.PaymentDate.Month == DateTime.UtcNow.Month 
                           && r.PaymentDate.Year == DateTime.UtcNow.Year);
        if(exists) return  null;
        
        var payment = new RentPayment()
        {
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _dbContext.RentPayments.Add(payment);
        await _dbContext.SaveChangesAsync();
        return new RentPaymentResponseDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate
        };
    }

    public async Task<bool> UpdatePaymentAsync(int id, RentPaymentsUpdateDto dto)
    {
        var  payment = await _dbContext.RentPayments.FindAsync(id);
        if (payment == null) return false;
        
        payment.Amount = dto.Amount;
        payment.PaymentDate = dto.PaymentDate;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }



    public async Task<bool> DeletePaymentAsync(int id)
    {
        var payment = await _dbContext.RentPayments.FindAsync(id);
        if (payment == null) return false;
        
        payment.IsDeleted =  true;
        payment.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}