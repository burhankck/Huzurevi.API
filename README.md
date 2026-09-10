# Huzurevi API

Huzurevi yönetim sistemi için ASP.NET Core tabanlı REST API. Oda, yatak ve sakin kayıtlarını yönetir; React arayüzü (`Huzurevi.Web`) ile CORS üzerinden konuşur.

## Kullanılan mimari

Proje **katmanlı (N-Layer) mimari** ve **REST** prensipleriyle kurulmuştur. Veri tarafında **Entity Framework Core Code First** kullanılır.

Katmanlar şu an tek ASP.NET Core projesinin içinde klasörlerle ayrılmıştır:

| Katman | Klasör | Görev |
| --- | --- | --- |
| Sunum (API) | `Controllers/` | HTTP isteklerini karşılar, iş kurallarını uygular, JSON döner |
| Veri erişimi | `Data/` | `AppDbContext` ile PostgreSQL'e bağlanır |
| Domain | `Models/` | Varlıklar ve ilişkiler |
| Şema yönetimi | `Migrations/` | Veritabanı şemasını Code First ile üretir |

Akış:

```
React (Huzurevi.Web)
        │  HTTP / JSON
        ▼
Controllers  →  AppDbContext (EF Core)  →  PostgreSQL
        ▲
        └── Models (Oda, Yatak, Sakin)
```

Bu aşamada ayrı bir Repository veya Service katmanı yoktur; controller'lar `AppDbContext`'i doğrudan kullanır. Amaç, şartnamedeki temel CRUD ve iş kurallarını sade bir yapıda ayağa kaldırmaktır.

## Teknolojiler

- **ASP.NET Core 10** (`net10.0`) — Web API
- **Entity Framework Core 10** — ORM, Code First
- **PostgreSQL** — `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Swashbuckle / Swagger** — OpenAPI belgesi ve deneme arayüzü
- **System.Text.Json** — ilişki döngülerini kırmak için `ReferenceHandler.IgnoreCycles`

## Domain modeli

Tüm varlıklar `TemelVarlik` sınıfından türer:

- `Id`
- `OlusturulmaTarihi`
- `GuncellenmeTarihi`
- `SilindiMi`
- `SilinmeTarihi`

İlişkiler:

```
Oda (1) ──── (N) Yatak (1) ──── (0..1) Sakin
```

- Bir odanın birden fazla yatağı olabilir.
- Bir yatakta en fazla bir sakin kalabilir (`YatakId` unique).
- Sakinin T.C. Kimlik numarası benzersizdir.

## Uygulanan iş kuralları

Şartname maddelerine göre şu kurallar API'de uygulanır:

- **Soft delete (4.5.17 / 4.5.37):** Kayıt fiziksel olarak silinmez. `SilindiMi = true` ve `SilinmeTarihi` set edilir. EF Core **global query filter** ile silinmiş kayıtlar listelerde görünmez.
- **T.C. Kimlik mükerrer kayıt engeli (4.5.40):** Hem veritabanında unique index hem de `SakinController` içinde kontrol vardır.
- **Oda kapasitesi:** Odaya eklenen yatak sayısı `Kapasite` değerini aşamaz.
- **Yatak doluluk:** Sakin kaydı yatağa bağlanırsa yatak `DoluMu = true` olur. Sakin soft-delete edilince yatak boşaltılır.
- **Dolu yatağa yeni sakin atanamaz.**

## API uçları

Taban adres (geliştirme): `http://localhost:5073`

Swagger: `http://localhost:5073/swagger`

| Method | URL | Açıklama |
| --- | --- | --- |
| `GET` | `/api/Oda` | Odaları yataklarıyla listeler |
| `POST` | `/api/Oda` | Yeni oda ekler |
| `DELETE` | `/api/Oda/{id}` | Odayı soft-delete eder |
| `GET` | `/api/Yatak` | Yatakları odasıyla listeler |
| `POST` | `/api/Yatak` | Odaya yatak ekler (kapasite kontrolü) |
| `GET` | `/api/Sakin` | Sakinleri yatak ve oda bilgisiyle listeler |
| `POST` | `/api/Sakin` | Sakin ekler (T.C. ve yatak doluluk kontrolü) |
| `DELETE` | `/api/Sakin/{id}` | Sakini soft-delete eder, yatağı boşaltır |

## Klasör yapısı

```
Huzurevi.API/
├── Controllers/
│   ├── OdaController.cs
│   ├── YatakController.cs
│   └── SakinController.cs
├── Models/
│   ├── TemelVarlik.cs
│   ├── Oda.cs
│   ├── Yatak.cs
│   └── Sakin.cs
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextFactory.cs   # EF migration'ları için design-time factory
├── Migrations/
│   └── 20260909162035_IlkKurulum.cs
├── Program.cs                   # DI, CORS, Swagger, PostgreSQL
├── appsettings.json
└── Huzurevi.API.http            # Örnek HTTP istekleri
```

## Neler yapıldı

1. ASP.NET Core Web API projesi oluşturuldu.
2. `TemelVarlik` taban sınıfı ile ortak alanlar (id, tarihler, soft delete) merkezi hale getirildi.
3. Oda, yatak ve sakin entity'leri ve aralarındaki ilişkiler modellendi.
4. PostgreSQL bağlantısı ve EF Core `AppDbContext` kuruldu.
5. T.C. Kimlik unique index ve silinmiş kayıtları gizleyen query filter eklendi.
6. İlk migration (`IlkKurulum`) ile `Odalar`, `Yataklar`, `Sakinler` tabloları üretildi.
7. REST controller'lar yazıldı; kapasite, mükerrer T.C. ve yatak doluluk kuralları eklendi.
8. Swagger açıldı.
9. React frontend'in API'ye istek atabilmesi için CORS politikası (`ReactIzin`) eklendi.
10. JSON döngüleri (`Oda → Yatak → Oda`) `IgnoreCycles` ile engellendi.

## Çalıştırma

Gereksinimler:

- .NET 10 SDK
- PostgreSQL (varsayılan: `localhost:5432`, veritabanı `HuzureviDb`)

Bağlantı dizesi `appsettings.json` içindeki `ConnectionStrings:VarsayilanBaglanti` alanından okunur. Kendi kullanıcı/şifrenize göre güncelleyin.

```bash
cd Huzurevi.API
dotnet restore
dotnet ef database update
dotnet run
```

API: `http://localhost:5073`  
Swagger: `http://localhost:5073/swagger`
