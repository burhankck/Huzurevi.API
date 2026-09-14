# Huzurevi.API

Huzurevi yönetim sistemi backend’i. Arayüz: [huzurevi-web](https://github.com/burhankck/huzurevi-web).

ASP.NET Core 10 (`net10.0`), PostgreSQL (EF Core + Npgsql), JWT.

Geliştirme: `http://localhost:5073` — Swagger `/swagger`, health `/health`.

Tohum kullanıcılar: `admin` / `Admin123!` (Yönetici), `ayse` / `Personel1!`.

### Son değişiklikler (RBAC)

Endpoint kilidi `[Yetki("izin.kodu")]` + JWT. Login sonrası `izinler` ve menü ağacı döner. Token yoksa 401, izin yoksa 403. Ayrıntı kök `README.md` içinde.

---

## Mimari

Temiz katmanlı ayrım. HTTP ince kalır; iş kuralları Application’dadır.

```
Huzurevi.API/                      # Controllers, middleware, Program.cs
  src/Huzurevi.Domain/             # Varlıklar
  src/Huzurevi.Application/         # Servisler, DTO, FluentValidation, IUygulamaDbContext
  src/Huzurevi.Infrastructure/     # EF, JWT, dosya, PDF/Excel, yedek, tohum
```

```
React  →  *Controller  →  I*Servisi  →  IUygulamaDbContext  →  PostgreSQL
```

**Bağımlılık:** API → Application + Infrastructure. Application, EF’yi arayüzle görür.

### Kurallar

- Controller DbContext kullanmaz.
- AutoMapper yok; DTO eşlemesi serviste.
- JSON zarf: `{ success, message, data, errors }`. `File()` indirmeleri zarfa girmez.
- Soft delete: `TemelVarlik.SilindiMi` + global query filter (denetim ve yedek listeleri hariç).
- FluentValidation; `HataYonetimAraKatmani` istisnaları HTTP koduna çevirir.
- Serilog: konsol + `Logs/` (git’te yok).
- CORS: `ReactIzin`.

### Kimlik ve yetki

- JWT Bearer. Rol ve `kurulusId` claim’den gelir. Yanıtta `token` ve `accessToken` aynı JWT’dir.
- `GET /api/Kimlik/profil-yetkileri` — aktif kuruluş, rol, `izinler`, izinli menü ağacı.
- `[Yetki("sakin.sil")]` politika ile endpoint kilidi (token + izin; aksi **403**).
- `[YetkiKaynak("sakin")]` HTTP metodunu CRUD izinlerine bağlar.
- `Yonetici` tüm kontrolleri geçer. `Personel` sistem kaynaklarını, narkotiği ve bazı silmeleri almaz.
- İzinler `IzinKatalogu`; `RolIzinleri` üzerinden roller ekranından atanır.

### Altyapı

- Bağlantı: `ConnectionStrings:VarsayilanBaglanti` (eski ad `DefaultConnection` da okunur).
- Açılışta `MigrateAsync` + kullanıcı/kurum tohumu.
- `Uploads/` yerel dosya deposu.
- PDF: QuestPDF. Excel: ClosedXML.
- Denetim ara katmanı: işlem, servis erişimi, sağlık (KVKK) ve 500 hataları.
- Yedek: `pg_dump` / `pg_restore` + yükleme zip. Zamanlama genel ayarlardadır.

---

## Çalıştırma

PostgreSQL’de `HuzureviDb` oluşturun; `appsettings.json` kullanıcısını doğrulayın.

```bash
cd Huzurevi.API
dotnet restore
dotnet run --project Huzurevi.API.csproj --launch-profile http
```

Migration:

```bash
dotnet ef migrations add Ad \
  --project src/Huzurevi.Infrastructure/Huzurevi.Infrastructure.csproj \
  --startup-project Huzurevi.API.csproj \
  --output-dir Persistence/Migrations
```

Port 5073 kilitliyse dinleyen süreci kapatın.

---

## Modüller

Sakin ve oda/yatak, ziyaret, sakin/kurum sağlık, kurum süreçleri, yemekhane, kütüphane, RBAC, liste Excel/PDF ve PDF şablon, sistem logları, yedekleme.

Lisans / garanti / SQL Server şartname maddeleri bu kodda yoktur; veri katmanı PostgreSQL’dir.

---

## Güvenlik

`appsettings.json` içindeki JWT anahtarı yerel geliştirme içindir. Canlıda anahtar ve bağlantıyı ortam değişkenine alın. `Logs/`, `Uploads/`, `Yedekler/` commit edilmez.
