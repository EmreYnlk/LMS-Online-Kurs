# EduHub — Online Kurs / LMS Platformu

ASP.NET Core MVC 8 ile geliştirilmiş Mini-Udemy tarzı bir öğrenme yönetim sistemi (LMS). Kullanıcılar öğrenci veya eğitmen olarak kayıt olabilir, eğitmenler kurs ve ders oluşturabilir, öğrenciler jeton sistemi ile kurslara abone olarak ilerlemelerini takip edebilir.

---

## Özellikler

### Kullanıcı Rolleri
| Rol | Yetkiler |
|-----|----------|
| **Admin** | Tüm kullanıcıları yönetme, eğitmen başvurularını onaylama/reddetme, jeton ekleme, kategori CRUD |
| **Eğitmen** | Kurs oluşturma/düzenleme/silme, ders ekleme (YouTube embed) |
| **Öğrenci** | Kurslara göz atma, jeton ile satın alma, ilerleme takibi (checkbox) |

### Temel İşlevler
- Kayıt sırasında **Öğrenci / Eğitmen** seçimi
- Eğitmen başvuruları **Admin onayına** tabidir
- Her kullanıcı kayıt olduğunda **500 başlangıç jetonu** alır
- Ücretli kurslar jeton ile satın alınır; ücretsiz kurslar direkt kaydolunur
- Ders videoları **YouTube embed** ile gösterilir
- Öğrenciler dersleri **checkbox** ile tamamlandı işaretler, ilerleme yüzdesi hesaplanır
- Kategori bazlı filtreleme ve kurs arama
- 3 adet **ViewComponent**: Popüler Kurslar, İlerleme Göstergesi, Kategori Menüsü

---

## Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Framework | ASP.NET Core MVC 8 |
| ORM | Entity Framework Core 8 (Code-First) |
| Veritabanı | SQLite |
| Kimlik Doğrulama | ASP.NET Core Identity |
| UI | Bootstrap 5 + Bootstrap Icons |
| Şablonlama | Razor Views + ViewComponents + ViewModel |

---

## Kurulum

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)

### Adımlar

```bash
# 1. Repoyu klonla
git clone https://github.com/emindinc/Online_Kurs-LMS_Platformu.git
cd Online_Kurs-LMS_Platformu/LMSPlatform

# 2. Veritabanını oluştur (migrations zaten mevcut)
dotnet ef database update

# 3. Uygulamayı başlat
dotnet run
```

Uygulama `http://localhost:5000` adresinde açılır.

> İlk çalıştırmada seed data otomatik yüklenir: Admin/Eğitmen/Öğrenci rolleri ve admin hesabı oluşturulur.

---

## Test Hesabı

| Alan | Değer |
|------|-------|
| E-posta | `admin@lms.com` |
| Şifre | `Admin123!` |
| Rol | Admin |

---

## Proje Yapısı

```
LMSPlatform/
├── Controllers/
│   ├── HomeController.cs         # Ana sayfa, arama, filtreleme
│   ├── AccountController.cs      # Kayıt, giriş, çıkış
│   ├── KursController.cs         # Kurs CRUD
│   ├── DersController.cs         # Ders CRUD
│   ├── OgrenciController.cs      # Abonelik, ders izleme, ilerleme
│   ├── AdminController.cs        # Admin paneli
│   └── KategoriController.cs     # Kategori CRUD
├── Models/
│   ├── ApplicationUser.cs        # Identity kullanıcı modeli (+ jeton, eğitmen durumu)
│   ├── Kurs.cs
│   ├── Ders.cs                   # YoutubeEmbedUrl dönüşümü dahil
│   ├── Kategori.cs
│   ├── KursAbonelik.cs
│   └── DersIlerleme.cs
├── ViewModels/                   # Controller → View veri taşıyıcıları
├── ViewComponents/
│   ├── PopulerKurslarViewComponent.cs
│   ├── IlerlemeViewComponent.cs
│   └── KategoriMenuViewComponent.cs
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core bağlamı
│   └── SeedData.cs               # Başlangıç verileri
├── Filters/
│   └── JetonFiltresi.cs          # Her action'da navbar jeton gösterimi
├── Migrations/                   # EF Core migration dosyaları
└── Views/                        # Razor sayfaları
```

---

## Veri Modeli

```
ApplicationUser ──< Kurs (EgitmenId)
ApplicationUser ──< KursAbonelik (OgrenciId)
ApplicationUser ──< DersIlerleme (OgrenciId)
Kategori        ──< Kurs
Kurs            ──< Ders
Kurs            ──< KursAbonelik
Ders            ──< DersIlerleme
```

---

## Şifre Kuralları

- En az **6 karakter**
- En az **1 rakam** (0-9)
