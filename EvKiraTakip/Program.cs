using Microsoft.EntityFrameworkCore;
using EvKiraTakip;
using EvKiraTakip.Common;
using EvKiraTakip.DTOs;
using EvKiraTakip.Models;
using EvKiraTakip.Services;
using EvKiraTakip.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHouseService, HouseService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IRentPaymentService, RentPaymentService>();

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
app.MapGet("/users", async (IUserService userService) =>
{
    var user = await userService.GetAllUserAsync();
    return Results.Ok(ApiResponse<List<UserResponseDto>>.Susscess(user));
});
app.MapGet("/users/{id}", async (int id, IUserService userService) =>
{
    var user = await userService.GetUserByIdAsync(id);
    if(user == null) return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    return Results.Ok(ApiResponse<UserResponseDto>.Susscess(user));
});
app.MapPost("/users", async (UserCreateDto dto, IUserService userService) =>
{
    var user = await userService.CreateUserAsync(dto);
    return Results.Created($"/users/{user.Id}", ApiResponse<UserResponseDto>.Susscess(user, "User created."));
});
app.MapPut("/users/{id}", async (int id, UserUpdateDto dto, IUserService userService) =>
{
    var user = await userService.UpdateAsync(id, dto);
    if(!user) return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    return Results.Ok(ApiResponse<string>.Susscess(null, "User updated successfully."));
});
app.MapDelete("/users/{id}", async (int id, IUserService userService) =>
{
    var user = await userService.DeleteAsync(id);
    if (!user) return Results.NotFound(ApiResponse<string>.Fail("User not found."));
    return Results.NoContent();
});

//House
app.MapGet("/houses", async (IHouseService houseService) =>
{
    var house = await houseService.GetAllHouseAsync();
    return Results.Ok(ApiResponse<List<HouseResponseDto>>.Susscess(house));
});
app.MapGet("/houses/{id}", async (int id, IHouseService houseService) =>
{
    var house = await houseService.GetHouseByIdAsync(id);
    if(house is null) return Results.NotFound(ApiResponse<string>.Fail("House not found."));
    return Results.Ok(ApiResponse<HouseResponseDto>.Susscess(house));
});
app.MapPost("/houses", async (HouseCreateDto dto, IHouseService houseService) =>
{
    var house = await houseService.CreateHouseAsync(dto);
    return Results.Created($"/houses/{house.Id}", ApiResponse<HouseResponseDto>.Susscess(house, "House created."));
});
app.MapPut("/houses/{id}", async (int id, HouseUpdateDto dto, IHouseService houseService) =>
{
    var house = await houseService.UpdateHouseAsync(id, dto);
    if(!house) return Results.NotFound(ApiResponse<string>.Fail("House not found."));
    return Results.Ok(ApiResponse<string>.Susscess(null, "House updated successfully."));
});
app.MapDelete("/houses/{id}", async (int id, IHouseService houseService) =>
{
    var house = await houseService.DeleteHouseAsync(id);
    if(!house) return Results.NotFound(ApiResponse<string>.Fail("House not found."));
    return Results.NoContent();
});

//Tenant
app.MapGet("/tenants", async (ITenantService tenantService) =>
{
    var tenant = await tenantService.GetAllTenantAsync();
    return Results.Ok(ApiResponse<List<TenantResponseDto>>.Susscess(tenant));
});
app.MapGet("/tenants/{id}", async (int id,ITenantService tenantService) =>
{
    var tenant = await tenantService.GetTenantByIdAsync(id);
    if(tenant is null) return Results.NotFound(ApiResponse<string>.Fail("Tenant not found."));
    return Results.Ok(ApiResponse<TenantResponseDto>.Susscess(tenant));
});
app.MapPost("/tenants", async (TenantCreateDto dto, ITenantService tenantService) =>
{
    var tenant = await tenantService.CreateTenantAsync(dto);
    return Results.Created($"/tenants/{tenant.Id}", ApiResponse<TenantResponseDto>.Susscess(tenant,"Tenant created."));
});
app.MapPut("/tenants/{id}", async (int id, TenantUpdateDto dto, ITenantService tenantService) =>
{
    var tenant = await tenantService.UpdateTenantAsync(id, dto);
    if(!tenant) return Results.NotFound(ApiResponse<string>.Fail("Tenant not found."));
    return Results.Ok(ApiResponse<string>.Susscess(null, "Tenant updated successfully."));
});
app.MapDelete("/tenants/{id}", async (int id, ITenantService tenantService) =>
{
    var tenant = await tenantService.DeleteTenantAsync(id);
    if (!tenant) return Results.NotFound(ApiResponse<string>.Fail("Tenant not found."));
    return Results.NoContent();
});

//RentPayment
app.MapGet("/rentPayments", async (IRentPaymentService rentPaymentService) =>
{
    var payment = await rentPaymentService.GetAllPaymentAsync();
    return Results.Ok(ApiResponse<List<RentPaymentResponseDto>>.Susscess(payment));
});
app.MapGet("/rentPayments/{id}", async (int id, IRentPaymentService rentPaymentService) =>
{
    var payment = await rentPaymentService.GetPaymentByIdAsync(id);
    if(payment is null) return Results.NotFound(ApiResponse<string>.Fail("Payment not found."));
    return Results.Ok(ApiResponse<RentPaymentResponseDto>.Susscess(payment));
});
app.MapPost("/rentPayments", async (RentPaymentCreateDto dto, IRentPaymentService rentPaymentService) =>
{
    var payment = await rentPaymentService.CreatePaymentAsync(dto);
    return Results.Created($"/rentPayments/{payment.Id}", ApiResponse<RentPaymentResponseDto>.Susscess(payment,"Rent Payment created."));
});
app.MapPut("/rentPayments/{id}", async (int id, RentPaymentsUpdateDto dto, IRentPaymentService rentPaymentService) =>
{
    var payment = await rentPaymentService.UpdatePaymentAsync(id, dto);
    if(!payment) return Results.NotFound(ApiResponse<string>.Fail("Rent payment not found."));
    return Results.Ok(ApiResponse<string>.Susscess(null, "Rent payment updated successfully."));
});
app.MapDelete("/rentPayments/{id}", async (int id, IRentPaymentService rentPaymentService) =>
{
    var payment = await rentPaymentService.DeletePaymentAsync(id);
    if(!payment) return Results.NotFound(ApiResponse<string>.Fail("Rent payment not found."));
    return Results.NoContent();
});

app.Run();