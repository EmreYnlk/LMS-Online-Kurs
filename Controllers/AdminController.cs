using LMSPlatform.Data;
using LMSPlatform.Models;
using LMSPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                ToplamKullanici = await _context.Users.CountAsync(),
                ToplamKurs = await _context.Kurslar.CountAsync(),
                BekleyenEgitmen = await _context.Users.CountAsync(u => u.EgitmenTalebi && !u.EgitmenOnaylandi),
                ToplamAbonelik = await _context.KursAbonelikler.CountAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Kullanicilar()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<KullaniciListeViewModel>();
            foreach (var user in users)
            {
                model.Add(new KullaniciListeViewModel
                {
                    Kullanici = user,
                    Roller = await _userManager.GetRolesAsync(user)
                });
            }
            return View(model);
        }

        public async Task<IActionResult> BekleyenEgitmenler()
        {
            var bekleyenler = await _userManager.Users
                .Where(u => u.EgitmenTalebi && !u.EgitmenOnaylandi)
                .ToListAsync();
            return View(bekleyenler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitmenOnayla(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.EgitmenOnaylandi = true;
            await _userManager.UpdateAsync(user);

            if (!await _userManager.IsInRoleAsync(user, "Egitmen"))
                await _userManager.AddToRoleAsync(user, "Egitmen");

            TempData["Basari"] = $"{user.TamAd} eğitmen olarak onaylandı.";
            return RedirectToAction(nameof(BekleyenEgitmenler));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitmenReddet(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.EgitmenTalebi = false;
            user.EgitmenOnaylandi = false;
            await _userManager.UpdateAsync(user);

            TempData["Bilgi"] = $"{user.TamAd} başvurusu reddedildi.";
            return RedirectToAction(nameof(BekleyenEgitmenler));
        }

        /// <summary>
        /// Kullanıcıya eğitmen rolü ekle
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitmenRolEkle(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.EgitmenTalebi = true;
            user.EgitmenOnaylandi = true;
            await _userManager.UpdateAsync(user);

            if (!await _userManager.IsInRoleAsync(user, "Egitmen"))
                await _userManager.AddToRoleAsync(user, "Egitmen");

            TempData["Basari"] = $"{user.TamAd} kullanıcısına Eğitmen yetkisi verildi.";
            return RedirectToAction(nameof(Kullanicilar));
        }

        /// <summary>
        /// Kullanıcıdan eğitmen rolünü kaldır
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitmenRolKaldir(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.EgitmenOnaylandi = false;
            user.EgitmenTalebi = false;
            await _userManager.UpdateAsync(user);

            if (await _userManager.IsInRoleAsync(user, "Egitmen"))
                await _userManager.RemoveFromRoleAsync(user, "Egitmen");

            TempData["Bilgi"] = $"{user.TamAd} kullanıcısının Eğitmen yetkisi kaldırıldı.";
            return RedirectToAction(nameof(Kullanicilar));
        }

        /// <summary>
        /// Kullanıcı sil
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciSil(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Admin kendini silemez
            if (currentUser?.Id == userId)
            {
                TempData["Hata"] = "Kendinizi silemezsiniz.";
                return RedirectToAction(nameof(Kullanicilar));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Diğer Admin'leri silmeyi engelle
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Hata"] = "Başka bir admin kullanıcısını silemezsiniz.";
                return RedirectToAction(nameof(Kullanicilar));
            }

            var tamAd = user.TamAd;

            try
            {
                // 1. Ders ilerlemeleri (OgrenciId → RESTRICT)
                var ilerlemeler = _context.DersIlerlemeleri.Where(i => i.OgrenciId == userId);
                _context.DersIlerlemeleri.RemoveRange(ilerlemeler);

                // 2. Kurs abonelikleri (OgrenciId → RESTRICT)
                var abonelikler = _context.KursAbonelikler.Where(a => a.OgrenciId == userId);
                _context.KursAbonelikler.RemoveRange(abonelikler);

                // 3. Kullanıcının kursları (EgitmenId → RESTRICT)
                //    Önce o kurslara ait abonelikler ve ilerlemeleri de temizle
                var kursIds = await _context.Kurslar
                    .Where(k => k.EgitmenId == userId)
                    .Select(k => k.Id)
                    .ToListAsync();

                if (kursIds.Any())
                {
                    var kursAbonelikler = _context.KursAbonelikler.Where(a => kursIds.Contains(a.KursId));
                    _context.KursAbonelikler.RemoveRange(kursAbonelikler);

                    var kursIlerlemeleri = _context.DersIlerlemeleri
                        .Where(i => _context.Dersler
                            .Where(d => kursIds.Contains(d.KursId))
                            .Select(d => d.Id)
                            .Contains(i.DersId));
                    _context.DersIlerlemeleri.RemoveRange(kursIlerlemeleri);

                    var kurslar = _context.Kurslar.Where(k => k.EgitmenId == userId);
                    _context.Kurslar.RemoveRange(kurslar);
                }

                // 4. Jeton işlemleri (CASCADE ile silinir ama önceden silmek daha güvenli)
                var jetonIslemleri = _context.JetonIslemleri.Where(j => j.KullaniciId == userId);
                _context.JetonIslemleri.RemoveRange(jetonIslemleri);

                await _context.SaveChangesAsync();

                // 5. Artık kullanıcıyı sil
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Basari"] = $"{tamAd} kullanıcısı ve tüm ilişkili verileri silindi.";
                }
                else
                {
                    TempData["Hata"] = "Kullanıcı silinirken hata: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Silme işlemi başarısız: {ex.Message}";
            }

            return RedirectToAction(nameof(Kullanicilar));
        }

        public async Task<IActionResult> JetonEkle(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var model = new JetonEkleViewModel
            {
                KullaniciId = user.Id,
                KullaniciAdi = user.TamAd,
                MevcutJeton = user.JetonMiktari
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JetonEkle(JetonEkleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.KullaniciId);
            if (user == null) return NotFound();

            user.JetonMiktari += model.EklenecekJeton;
            await _userManager.UpdateAsync(user);

            // Jeton ekle kaydı
            _context.JetonIslemleri.Add(new JetonIslem
            {
                KullaniciId = user.Id,
                Tur = JetonIslemTuru.Satin,
                Miktar = model.EklenecekJeton,
                Aciklama = $"Admin tarafından eklendi ({model.EklenecekJeton} jeton)"
            });
            await _context.SaveChangesAsync();

            TempData["Basari"] = $"{user.TamAd} hesabına {model.EklenecekJeton} jeton eklendi.";
            return RedirectToAction(nameof(Kullanicilar));
        }

        public async Task<IActionResult> TumKurslar()
        {
            var kurslar = await _context.Kurslar
                .Include(k => k.Kategori)
                .Include(k => k.Egitmen)
                .Include(k => k.Abonelikler)
                .OrderByDescending(k => k.OlusturmaTarihi)
                .ToListAsync();
            return View(kurslar);
        }
    }
}
