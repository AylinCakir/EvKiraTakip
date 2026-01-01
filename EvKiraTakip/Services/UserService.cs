using EvKiraTakip.DTOs;
using EvKiraTakip.Models;
using EvKiraTakip.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvKiraTakip.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserResponseDto>> GetAllUserAsync()
    {
        return await _dbContext.Users
            .Include(u=> u.Houses)
            .ThenInclude(h => h.Tenants)
            .ThenInclude(t => t.RentPayments)
            .Select(u=> new UserResponseDto
            {
                Id = u.Id,
                FullName = u.Name + " " + u.Surname,
                Email = u.Email,
                Houses = u.Houses.Select(h=> new HouseResponseDto
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
                }).ToList()
            })
            .ToListAsync(); 
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _dbContext.Users
            .Include(u=> u.Houses)
            .FirstOrDefaultAsync(u=> u.Id == id);
        if (user == null) return null;

        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.Name,
            Email = user.Email,
            Houses = user.Houses.Select(h => new HouseResponseDto
            {
                Id = h.Id,
                Title = h.Title,
                Address = h.Address
            }).ToList()
        };
    }

    public async Task<UserResponseDto?> CreateUserAsync(UserCreateDto dto)
    {
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists) return null;
        
        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            Age = dto.Age,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.Name + " " + user.Surname,
            Email = user.Email,
        };
    }

    public async Task<bool> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) return false;

        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Email = dto.Email;
        user.Age = dto.Age;
        user.Address = dto.Address;
        user.UpdatedAt = DateTime.UtcNow;
        
        await  _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) return false;

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}