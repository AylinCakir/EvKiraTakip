using Microsoft.EntityFrameworkCore;
using EvKiraTakip;
using EvKiraTakip.Common;
using EvKiraTakip.DTOs;
using EvKiraTakip.Models;

var  builder = WebApplication.CreateBuilder(args);

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
    var users = await db.Users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            FullName = u.Name +  " " + u.Surname,
            Email = u.Email,
        })
        .ToListAsync();
    return Results.Ok(ApiResponse<List<UserResponseDto>>.Susscess(users));
});
app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound(ApiResponse<string>.Fail("User not found"));

    var response = new UserResponseDto
    {
        Id = user.Id,
        FullName = user.Name + " " + user.Surname,
        Email = user.Email
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
    return Results.Created($"/users/{user.Id}", ApiResponse<UserResponseDto>.Susscess(response, "User created"));
});
app.MapPut("/user/{id}", async (int id, UserUpdateDto dto, AppDbContext db) =>
{
    var user =await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound(ApiResponse<string>.Fail("User not found."));

    user.Name = dto.Name;
    user.Surname = dto.Surname;
    user.Age = dto.Age;
    user.Address = dto.Address;
    user.CreatedAt = DateTime.UtcNow;
    
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<string>.Susscess(null, "User updated."));
});
app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await  db.Users.FindAsync(id);
    if (user is null) return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//House
app.MapGet("/houses", async (AppDbContext db) => await db.Houses.ToListAsync());
app.MapGet("/house/{id}", async (int id, AppDbContext db) => await db.Houses.FindAsync(id));
app.MapPost("/houses", async (House house, AppDbContext db) =>
{
    db.Houses.Add(house);
    await db.SaveChangesAsync();
});
app.MapPut("/houses/{id}", async (int id, House inputHouse, AppDbContext db) =>
{
    var house = db.Houses.Find(id);
    if (house == null) return Results.NotFound();

    house.Title = inputHouse.Title;
    house.Address = inputHouse.Address;
    
    await db.SaveChangesAsync();
    
    return Results.NoContent();
});
app.MapDelete("/houses/{id}", async (int id, AppDbContext db) =>
{
    var house = await  db.Houses.FindAsync(id);
    if (house is null) return Results.NotFound();
    db.Houses.Remove(house);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Tenant
app.MapGet("/tenants", async (AppDbContext db) => await db.Tenants.ToListAsync());
app.MapGet("/tenants/{id}", async (int id, AppDbContext db) => await db.Tenants.FindAsync(id));
app.MapPost("/tenants", async (Tenant tenant, AppDbContext db) =>
{
    db.Tenants.Add(tenant);
    await db.SaveChangesAsync();
});
app.MapPut("/tenants/{id}", async (int id, Tenant inputTenant, AppDbContext db) =>
{
    var tenant = db.Tenants.Find(id);
    if (tenant == null) return Results.NotFound();

    tenant.Name = inputTenant.Name;
    tenant.Phone = inputTenant.Phone;

    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/tenants/{id}", async (int id, AppDbContext db) =>
{
    var tenant = await db.Tenants.FindAsync(id);
    if (tenant is null) return Results.NotFound();
    db.Tenants.Remove(tenant);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//RentPayment
app.MapGet("/rentPayments", async (AppDbContext db) => await db.RentPayments.ToListAsync());
app.MapGet("/rentPayments/{id}", async (int id, AppDbContext db) => await db.RentPayments.FindAsync(id));
app.MapPost("/rentPayments", async (RentPayment rentPayment, AppDbContext db) =>
{
    db.RentPayments.Add(rentPayment);
    await db.SaveChangesAsync();
});
app.MapPut("/rentPayments/{id}", async (int id, RentPayment inputRentPayment, AppDbContext db) =>
{
    var rentPayment = db.RentPayments.Find(id);
    if (rentPayment == null) return Results.NotFound();

    rentPayment.Amount = inputRentPayment.Amount;
    rentPayment.PaymentDate = inputRentPayment.PaymentDate;

    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/rentPayments/{id}", async (int id, AppDbContext db) =>
{
    var rentPayment = db.RentPayments.Find(id);
    if (rentPayment is null) return Results.NotFound();
    db.RentPayments.Remove(rentPayment);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

