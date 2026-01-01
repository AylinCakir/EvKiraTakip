using EvKiraTakip.DTOs;
using EvKiraTakip.Enums;
using EvKiraTakip.Models;
using EvKiraTakip.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvKiraTakip.Services;


public class HouseService : IHouseService
{
    private readonly AppDbContext _dbContext;
    public HouseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<HouseResponseDto>> GetAllHouseAsync()
    {
        return await _dbContext.Houses
            .Include(h=> h.Tenants)
            .ThenInclude(t=> t.RentPayments)
            .Select(h=> new HouseResponseDto
            {
                Id = h.Id,
                Title = h.Title,
                Address = h.Address,
                Tenants = h.Tenants.Select(t => new TenantResponseDto
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
                }).ToList()
            }).ToListAsync();
    }

    public async Task<HouseResponseDto?> GetHouseByIdAsync(int id)
    {
        return await _dbContext.Houses
            .Include(h => h.Tenants)
            .ThenInclude(t => t.RentPayments)
            .Where(h => h.Id == id)
            .Select(h => new HouseResponseDto
            {
                Id = h.Id,
                Title = h.Title,
                Address = h.Address,
                Tenants = h.Tenants.Select(t => new TenantResponseDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Phone = t.Phone,
                    RentPayments = t.RentPayments.Select(r => new RentPaymentResponseDto
                    {
                        Amount = r.Amount,
                        PaymentDate = r.PaymentDate
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<HouseResponseDto?> CreateHouseAsync(HouseCreateDto dto)
    {
        var exists = await  _dbContext.Houses.AnyAsync(h => h.UserId == dto.UserId && h.Title == dto.Title);
        if (exists) return null;
        
        var house = new House()
        {
            Title = dto.Title,
            Address = dto.Address,
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Add(house);
        await _dbContext.SaveChangesAsync();

        return new HouseResponseDto
        {
            Id = house.Id,
            Title = house.Title,
            Address = house.Address,
        };
    }

    public async Task<bool> UpdateHouseAsync(int id,HouseUpdateDto dto)
    {
        var house = await _dbContext.Houses.FindAsync(id);
        if (house == null) return false;
        
        house.Title = dto.Title;
        house.Address = dto.Address;
        house.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<DeleteHouseResult> DeleteHouseAsync(int id)
    {
        var house = await _dbContext.Houses.FindAsync(id);
        if (house == null) return DeleteHouseResult.NotFound;

        var hasTenant = await _dbContext.Tenants.AnyAsync(t => t.HouseId == id);
        if (hasTenant) return DeleteHouseResult.HasTenants;

        _dbContext.Houses.Remove(house);
        await _dbContext.SaveChangesAsync();
        return DeleteHouseResult.Deleted;
    }
}