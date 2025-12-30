using EvKiraTakip.DTOs;
using EvKiraTakip.Models;
using EvKiraTakip.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvKiraTakip.Services;

public class TenantService : ITenantService
{
    private readonly AppDbContext _dbContext;
    public TenantService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TenantResponseDto>> GetAllTenantAsync()
    {
        return await  _dbContext.Tenants
            .Include(t => t.RentPayments)
            .Select(t=> new TenantResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                RentPayments = t.RentPayments.Select(r => new RentPaymentResponseDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    PaymentDate = r.PaymentDate
                }).ToList()
            }).ToListAsync();
    }

    public async Task<TenantResponseDto?> GetTenantByIdAsync(int id)
    {
        return await _dbContext.Tenants
            .Include(t => t.RentPayments)
            .Where(t=> t.Id == id)
            .Select(t => new TenantResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                RentPayments = t.RentPayments.Select(r => new RentPaymentResponseDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    PaymentDate = r.PaymentDate
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<TenantResponseDto> CreateTenantAsync(TenantCreateDto dto)
    {
        var tenant = new Tenant()
        {
            Name = dto.Name,
            Phone = dto.Phone,
            HouseId = dto.HouseId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Phone = tenant.Phone
        };
    }

    public async Task<bool> UpdateTenantAsync(int id, TenantUpdateDto dto)
    {
        var tenant = await _dbContext.Tenants.FindAsync(id);
        if (tenant == null) return false;

        tenant.Name = dto.Name;
        tenant.Phone = dto.Phone;
        tenant.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTenantAsync(int id)
    {
        var tenant = await _dbContext.Tenants.FindAsync(id);
        if (tenant == null) return false;
        
        _dbContext.Tenants.Remove(tenant);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    
}