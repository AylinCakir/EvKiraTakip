using Microsoft.EntityFrameworkCore;
using EvKiraTakip;
using EvKiraTakip.Common;
using EvKiraTakip.DTOs;
using EvKiraTakip.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Users
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users
        .Include(u => u.Houses)
        .ThenInclude(h => h.Tenants)
        .ThenInclude(t => t.RentPayments)
        .Select(u => new UserResponseDto
        {
            Id = u.Id,
            FullName = u.Name + " " + u.Surname,
            Email = u.Email,
            Houses = u.Houses.Select(h => new HouseResponseDto
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
    return Results.Ok(ApiResponse<List<UserResponseDto>>.Susscess(users));
});
app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users
        .Include(u => u.Houses)
        .ThenInclude(h=> h.Tenants)
        .ThenInclude(r=>r.RentPayments)
        .Where(t=> t.Id == id)
        .FirstOrDefaultAsync(u => u.Id == id);
    if (user == null) return Results.NotFound(ApiResponse<string>.Fail("User not found"));

    var response = new UserResponseDto
    {
        Id = user.Id,
        FullName = user.Name + " " + user.Surname,
        Email = user.Email,
        Houses = user.Houses.Select(h => new HouseResponseDto
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
    };
    return Results.Ok(ApiResponse<UserResponseDto>.Susscess(response));
});
app.MapPost("/users", async (UserCreateDto dto, AppDbContext db) =>
{
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
    db.Users.Add(user);
    await db.SaveChangesAsync();

    var response = new UserResponseDto
    {
        Id = user.Id,
        FullName = user.Name + " " + user.Surname,
        Email = user.Email
    };
    return Results.Created($"/users/{user.Id}", ApiResponse<UserResponseDto>.Susscess(response, "User created."));
});
app.MapPut("/users/{id}", async (int id, UserUpdateDto dto, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound(ApiResponse<string>.Fail("User not found."));

    user.Name = dto.Name;
    user.Surname = dto.Surname;
    user.Email = dto.Email;
    user.Age = dto.Age;
    user.Address = dto.Address;
    user.UpdatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null, "User updated."));
});
app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await  db.Users.FindAsync(id);
    if (user is null) return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null,"Deleted successfully."));
});

//House
app.MapGet("/houses", async (AppDbContext db) =>
{
    var houses = await db.Houses
        .Include(h => h.Tenants)
        .ThenInclude(t => t.RentPayments)
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
                    Id = r.Id,
                    Amount = r.Amount,
                    PaymentDate = r.PaymentDate
                }).ToList()
            }).ToList()
        }).ToListAsync();
    
    return Results.Ok(ApiResponse<List<HouseResponseDto>>.Susscess(houses));
});
app.MapGet("/houses/{id}", async (int id, AppDbContext db) =>
{
    var houses = await db.Houses
        .Include(h => h.Tenants)
        .ThenInclude(t => t.RentPayments)
        .Where(h=> h.Id == id)
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
                    Id = r.Id,
                    Amount = r.Amount,
                    PaymentDate = r.PaymentDate
                }).ToList()
            }).ToList()
        }).FirstOrDefaultAsync();
    
    if(houses is null) return Results.NotFound(ApiResponse<string>.Fail("House not found."));
    
    return Results.Ok(ApiResponse<HouseResponseDto>.Susscess(houses));
});
app.MapPost("/houses", async (HouseCreateDto dto, AppDbContext db) =>
{
    var userExists = await  db.Users.AnyAsync(u => u.Id == dto.UserId);
    if (!userExists)
    {
        return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    }

    var house = new House()
    {
        Title = dto.Title,
        Address = dto.Address,
        UserId = dto.UserId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    
    db.Houses.Add(house);
    await db.SaveChangesAsync();

    var response = new HouseResponseDto()
    {
        Id = house.Id,
        Title = house.Title,
        Address = house.Address,
        
    };
    return Results.Created($"/houses/{house.Id}", ApiResponse<HouseResponseDto>.Susscess(response,"House  created."));
});
app.MapPut("/houses/{id}", async (int id, HouseUpdateDto dto, AppDbContext db) =>
{
    var house = db.Houses.Find(id);
    if (house == null) return Results.NotFound(ApiResponse<string>.Fail("House not found."));

    house.Title = dto.Title;
    house.Address = dto.Address;
    house.UpdatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    
    return Results.Ok(ApiResponse<string>.Susscess(null, "House updated."));
});
app.MapDelete("/houses/{id}", async (int id, AppDbContext db) =>
{
    var house = await  db.Houses.FindAsync(id);
    if (house is null) return Results.NotFound();
    db.Houses.Remove(house);
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null,"Deleted successfully."));
});

//Tenant
app.MapGet("/tenants", async (AppDbContext db) =>
{
    var tenants = await db.Tenants
        .Include(t => t.RentPayments)
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
        }).ToListAsync();
    return Results.Ok(ApiResponse<List<TenantResponseDto>>.Susscess(tenants));
});
app.MapGet("/tenants/{id}", async (int id, AppDbContext db) =>
{
    var tenants = await db.Tenants
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
    if(tenants is null) return Results.NotFound(ApiResponse<string>.Fail("Tenant not found."));
    
    return Results.Ok(ApiResponse<TenantResponseDto>.Susscess(tenants));
});
app.MapPost("/tenants", async (TenantCreateDto dto, AppDbContext db) =>
{
    var houseExists = await db.Houses.AnyAsync(h => h.Id == dto.HouseId);
    if (!houseExists)
        return Results.NotFound("House not found.");
    var tenant = new Tenant()
    {
        Name = dto.Name,
        Phone = dto.Phone,
        HouseId = dto.HouseId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Tenants.Add(tenant);
    await db.SaveChangesAsync();

    var response = new TenantResponseDto
    {
        Id = tenant.Id,
        Name = tenant.Name,
        Phone = tenant.Phone
    };
    return Results.Created($"/tenants/{tenant.Id}", ApiResponse<TenantResponseDto>.Susscess(response,"Tenant created."));
});
app.MapPut("/tenants/{id}", async (int id, TenantUpdateDto dto, AppDbContext db) =>
{
    var tenant = db.Tenants.Find(id);
    if (tenant == null) return Results.NotFound();

    tenant.Name = dto.Name;
    tenant.Phone = dto.Phone;
    tenant.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null, "Tenant updated."));
});
app.MapDelete("/tenants/{id}", async (int id, AppDbContext db) =>
{
    var tenant = await db.Tenants.FindAsync(id);
    if (tenant is null) return Results.NotFound();
    db.Tenants.Remove(tenant);
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null,"Deleted successfully."));
});

//RentPayment
app.MapGet("/rentPayments", async (AppDbContext db) =>
{
    var payment = await db.RentPayments.Select(r => new RentPaymentResponseDto
    {
        Id = r.Id,
        Amount = r.Amount,
        PaymentDate = r.PaymentDate
    }).ToListAsync();
    return Results.Ok(ApiResponse<List<RentPaymentResponseDto>>.Susscess(payment));
});
app.MapGet("/rentPayments/{id}", async (int id, AppDbContext db) =>
{
    var payments = await db.RentPayments
        .Where(r=>r.Id == id)
        .Select(r => new RentPaymentResponseDto
    {
        Id = r.Id,
        Amount = r.Amount,
        PaymentDate = r.PaymentDate
    }).FirstOrDefaultAsync();
    if (payments is null ) return  Results.NotFound(ApiResponse<string>.Fail("Payment not found."));
    return Results.Ok(ApiResponse<RentPaymentResponseDto>.Susscess(payments));
});
app.MapPost("/rentPayments", async (RentPaymentCreateDto dto, AppDbContext db) =>
{
    var tenantExists = await db.Tenants.AnyAsync(t => t.Id == dto.TenantId);
    if (!tenantExists)
        return Results.NotFound("Tenant not found.");
    var payment = new RentPayment
    {
        Amount = dto.Amount,
        TenantId = dto.TenantId,
        PaymentDate = DateTime.UtcNow,
        CreatedAt =  DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    
    db.RentPayments.Add(payment);
    await db.SaveChangesAsync();

    var response = new RentPaymentResponseDto
    {
        Id = payment.Id,
        Amount = payment.Amount,
        PaymentDate = payment.PaymentDate,
    };
    return Results.Created($"/rentPayments/{payment.Id}", ApiResponse<RentPaymentResponseDto>.Susscess(response,"Rent Payment created."));
});
app.MapPut("/rentPayments/{id}", async (int id, RentPaymentsUpdateDto dto, AppDbContext db) =>
{
    var rentPayment = db.RentPayments.Find(id);
    if (rentPayment == null) return Results.NotFound();

    rentPayment.Amount = dto.Amount;
    rentPayment.PaymentDate = dto.PaymentDate;
    rentPayment.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null, "Rent payment updated."));
});
app.MapDelete("/rentPayments/{id}", async (int id, AppDbContext db) =>
{
    var rentPayment = db.RentPayments.Find(id);
    if (rentPayment is null) return Results.NotFound();
    db.RentPayments.Remove(rentPayment);
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null,"Deleted successfully."));
});

app.Run();