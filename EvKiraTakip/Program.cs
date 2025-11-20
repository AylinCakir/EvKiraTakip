using Microsoft.EntityFrameworkCore;
using EvKiraTakip.Models;
using EvKiraTakip;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// USERS
// Tüm kullanıcıları getir
app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());

// Tek kullanıcı getir
app.MapGet("/users/{id}", async (int id, AppDbContext db) => await db.Users.FindAsync(id));

// Yeni kullanıcı ekle
app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// Kullanıcı güncelle
app.MapPut("/users/{id}", async (int id, User inputUser, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.FullName = inputUser.FullName;
    user.Email = inputUser.Email;
    user.Role = inputUser.Role;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Kullanıcı sil
app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// PROPERTIES (DAİRELER)
app.MapGet("/properties", async (AppDbContext db) => await db.Properties.Include(p => p.Tenant).ToListAsync());

app.MapGet("/properties/{id}", async (int id, AppDbContext db) =>
    await db.Properties.Include(p => p.Tenant).FirstOrDefaultAsync(p => p.Id == id));

app.MapPost("/properties", async (Property property, AppDbContext db) =>
{
    db.Properties.Add(property);
    await db.SaveChangesAsync();
    return Results.Created($"/properties/{property.Id}", property);
});

app.MapPut("/properties/{id}", async (int id, Property inputProperty, AppDbContext db) =>
{
    var property = await db.Properties.FindAsync(id);
    if (property is null) return Results.NotFound();

    property.Address = inputProperty.Address;
    property.RentAmount = inputProperty.RentAmount;
    property.TenantId = inputProperty.TenantId;
    await db.SaveChangesAsync();

    return Results.Ok(property);
});

app.MapDelete("/properties/{id}", async (int id, AppDbContext db) =>
{
    var property = await db.Properties.FindAsync(id);
    if (property is null) return Results.NotFound();

    db.Properties.Remove(property);
    await db.SaveChangesAsync();
    return Results.Ok();
});


// TENANTS (KİRACILAR)
app.MapGet("/tenants", async (AppDbContext db) => await db.Tenants.Include(t => t.Property).ToListAsync());

app.MapGet("/tenants/{id}", async (int id, AppDbContext db) =>
    await db.Tenants.Include(t => t.Property).FirstOrDefaultAsync(t => t.Id == id));

app.MapPost("/tenants", async (Tenant tenant, AppDbContext db) =>
{
    db.Tenants.Add(tenant);
    await db.SaveChangesAsync();
    return Results.Created($"/tenants/{tenant.Id}", tenant);
});

app.MapPut("/tenants/{id}", async (int id, Tenant inputTenant, AppDbContext db) =>
{
    var tenant = await db.Tenants.FindAsync(id);
    if (tenant is null) return Results.NotFound();

    tenant.Name = inputTenant.Name;
    tenant.Phone = inputTenant.Phone;
    tenant.Email = inputTenant.Email;
    tenant.StartDate = inputTenant.StartDate;
    tenant.PropertyId = inputTenant.PropertyId;

    await db.SaveChangesAsync();
    return Results.Ok(tenant);
});

app.MapDelete("/tenants/{id}", async (int id, AppDbContext db) =>
{
    var tenant = await db.Tenants.FindAsync(id);
    if (tenant is null) return Results.NotFound();

    db.Tenants.Remove(tenant);
    await db.SaveChangesAsync();
    return Results.Ok();
});


// PAYMENTS (KİRA ÖDEMELERİ)
app.MapGet("/payments", async (AppDbContext db) => await db.Payments.Include(p => p.Property).ToListAsync());

app.MapGet("/payments/{id}", async (int id, AppDbContext db) =>
    await db.Payments.Include(p => p.Property).FirstOrDefaultAsync(p => p.Id == id));

app.MapPost("/payments", async (Payment payment, AppDbContext db) =>
{
    db.Payments.Add(payment);
    await db.SaveChangesAsync();
    return Results.Created($"/payments/{payment.Id}", payment);
});

app.MapPut("/payments/{id}", async (int id, Payment inputPayment, AppDbContext db) =>
{
    var payment = await db.Payments.FindAsync(id);
    if (payment is null) return Results.NotFound();

    payment.Month = inputPayment.Month;
    payment.Amount = inputPayment.Amount;
    payment.IsPaid = inputPayment.IsPaid;
    payment.DatePaid = inputPayment.DatePaid;
    payment.PropertyId = inputPayment.PropertyId;

    await db.SaveChangesAsync();
    return Results.Ok(payment);
});

app.MapDelete("/payments/{id}", async (int id, AppDbContext db) =>
{
    var payment = await db.Payments.FindAsync(id);
    if (payment is null) return Results.NotFound();

    db.Payments.Remove(payment);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();