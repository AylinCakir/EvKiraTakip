# EvKiraTakip REST API

## Proje Açıklaması

EvKiraTakip, ev sahiplerinin kullanıcı, ev, kiracı ve kira ödeme süreçlerini yönetebilmesi için geliştirilmiş bir **.NET 9 tabanlı RESTful Web API** uygulamasıdır.

Proje; katmanlı mimari prensiplerine uygun olarak geliştirilmiş, **DTO yapısı**, **service–repository yaklaşımı**, **global exception handling**, **standart API response yapısı**, **logging (Serilog)** ve **soft delete** gibi modern backend pratiklerini içermektedir.

Amaç; sürdürülebilir, okunabilir ve genişletilebilir bir API mimarisi sunmaktır.

---

## Mimari Diyagram

Uygulama aşağıdaki katmanlı mimari yapıyı takip eder:

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ HTTP Requests
┌──────▼──────┐
│   API       │  (Minimal API / Endpoints)
│  Controller │
└──────┬──────┘
       │
┌──────▼──────┐
│  Services   │  (Business Logic)
└──────┬──────┘
       │
┌──────▼──────┐
│ Data Access │  (EF Core / DbContext)
└──────┬──────┘
       │
┌──────▼──────┐
│   Database  │  (SQLite)
└─────────────┘
```

Ek olarak:

* Global Exception Middleware
* Logging (Serilog)
* Global Query Filters (Soft Delete)

---

## Endpoint Listesi

### Users

| Method | Endpoint    | Açıklama                      |
| ------ | ----------- |-------------------------------|
| GET    | /users      | Tüm kullanıcıları getirir     |
| GET    | /users/{id} | Kullanıcı detayı              |
| POST   | /users      | Yeni kullanıcı oluşturur      |
| PUT    | /users/{id} | Kullanıcı günceller           |
| DELETE | /users/{id} | Kullanıcı siler (soft delete) |

### Houses

| Method | Endpoint     | Açıklama               |
| ------ | ------------ | ---------------------- |
| GET    | /houses      | Tüm evleri getirir     |
| GET    | /houses/{id} | Ev detayı              |
| POST   | /houses      | Yeni ev ekler          |
| PUT    | /houses/{id} | Ev günceller           |
| DELETE | /houses/{id} | Ev siler (soft delete) |

### Tenants

| Method | Endpoint      | Açıklama                   |
| ------ | ------------- | -------------------------- |
| GET    | /tenants      | Tüm kiracılar              |
| GET    | /tenants/{id} | Kiracı detayı              |
| POST   | /tenants      | Kiracı ekler               |
| PUT    | /tenants/{id} | Kiracı günceller           |
| DELETE | /tenants/{id} | Kiracı siler (soft delete) |

### Rent Payments

| Method | Endpoint           | Açıklama                  |
| ------ | ------------------ | ------------------------- |
| GET    | /rentPayments      | Tüm ödemeler              |
| GET    | /rentPayments/{id} | Ödeme detayı              |
| POST   | /rentPayments      | Yeni ödeme                |
| PUT    | /rentPayments/{id} | Ödeme günceller           |
| DELETE | /rentPayments/{id} | Ödeme siler (soft delete) |

---

## API Response Örnekleri

### Başarılı GET (200 OK)

```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "name": "Example"
  }
}
```
### Başarılı CREATE (201 CREATED)

```json
{
  "success": true,
  "message": "User created.",
  "data": {
    "id": 5,
    "name": "Example"
  }
}
```
### Not Found (404)

```json
{
  "success": false,
  "message": "Resource not found",
  "data": null
}
```
### Conflict (409)

```json
{
  "success": false,
  "message": "Email already exists.",
  "data": null
}
```
### Bad Request (400)

```json
{
  "success": false,
  "message": "Invalid request data.",
  "data": null
}
```
### Sunucu Hatası (500)

```json
{
  "success": false,
  "message": "Unexpected error occured",
  "data": null
}
```

Tüm response’lar **ApiResponse<T>** standardı ile dönmektedir.

---

## Kurulum Talimatları

### Gereksinimler

* .NET SDK 9
* SQLite
* Git

### Kurulum Adımları

1. Projeyi klonlayın

```bash
git clone https://github.com/AylinCakir/EvKiraTakip
```

2. Proje dizinine girin

```bash
cd EvKiraTakip
```

3. Bağımlılıkları yükleyin

```bash
dotnet restore
```

4. Veritabanını oluşturun

```bash
dotnet ef database update
```

5. Uygulamayı çalıştırın

```bash
dotnet run
```

6. Swagger arayüzüne erişin

```
https://localhost:5030/swagger
```

---

## Ek Notlar

* Silme işlemleri **soft delete** olarak uygulanmaktadır.
* Loglama işlemleri **Serilog** ile console ve dosya bazlı yapılmaktadır.
* Global exception middleware ile tüm hatalar merkezi olarak yönetilmektedir.

---

Bu proje eğitim ve ödev amaçlı geliştirilmiştir.
