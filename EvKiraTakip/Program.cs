using Microsoft.EntityFrameworkCore;
using EvKiraTakip;
using EvKiraTakip.Common;
using EvKiraTakip.DTOs;
using EvKiraTakip.Enums;
using EvKiraTakip.Services;
using EvKiraTakip.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Core;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHouseService, HouseService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IRentPaymentService, RentPaymentService>();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Information("Application Starting...");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        var exception =  exceptionHandlerPathFeature?.Error;
        Log.Error(exception, "Unhandled exception occured.");
        
        var response =
            ApiResponse<string>.Fail(exceptionHandlerPathFeature?.Error.Message ?? "Unexcepted error occured.");

        await context.Response.WriteAsJsonAsync(response);
    });
});

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
    if (user == null) return Results.Conflict(ApiResponse<string>.Fail("Email already exists."));
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
    if (house == null) return Results.Conflict(ApiResponse<string>.Fail("House with same title already exists for this user."));
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
    return house switch
    {
        DeleteHouseResult.NotFound => Results.NotFound(ApiResponse<string>.Fail("House not found.")),
        
        DeleteHouseResult.HasTenants => Results.Conflict(ApiResponse<string>.Fail("House has tenants, cannot deleted.")),
        
        DeleteHouseResult.Deleted => Results.NoContent(),
        
        _ => Results.StatusCode(500)
    };
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
    if(tenant == null) return  Results.Conflict(ApiResponse<string>.Fail("Tenant already exists in this house."));
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
    if(payment == null) return Results.Conflict(ApiResponse<string>.Fail("Rent already paid for this month."));
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

app.Lifetime.ApplicationStopped.Register(() =>
{
    Log.Information("Application stopped.");
});

app.Run();