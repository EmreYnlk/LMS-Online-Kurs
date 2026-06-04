using LMSPlatform.Data;
using LMSPlatform.Models;
using LMSPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    [Authorize]
    public class OgrenciController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OgrenciController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var abonelikler = await _context.KursAbonelikler
                .Include(a => a.Kurs)
                    .ThenInclude(k => k!.Dersler)
                .Include(a => a.Kurs)
                    .ThenInclude(k => k!.Kategori)
                .Where(a => a.OgrenciId == user!.Id)
                .ToListAsync();

            var dersIds = abonelikler.SelectMany(a => a.Kurs!.Dersler.Select(d => d.Id)).ToList();
            var ilerlemeler = await _context.DersIlerlemeleri
                .Where(i => i.OgrenciId == user!.Id && dersIds.Contains(i.DersId))
                .ToListAsync();

            ViewBag.Ilerlemeler = ilerlemeler;
            ViewBag.Kullanici = user;
            return View(abonelikler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AboneOl(int kursId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var kurs = await _context.Kurslar.FindAsync(kursId);
            if (kurs == null) return NotFound();

            // Eğitmen kendi kursuna abone olamaz
            if (kurs.EgitmenId == user.Id)
            {
                TempData["Hata"] = "Kendi kursunuza abone olamazsınız.";
                return RedirectToAction("Detay", "Kurs", new { id = kursId });
            }

            var mevcutAbonelik = await _context.KursAbonelikler
                .AnyAsync(a => a.KursId == kursId && a.OgrenciId == user.Id);

            if (mevcutAbonelik)
            {
                TempData["Hata"] = "Bu kursa zaten abonesiniz.";
                return RedirectToAction("Detay", "Kurs", new { id = kursId });
            }

            if (kurs.Fiyat > 0)
            {
                if (user.JetonMiktari < kurs.Fiyat)
                {
                    TempData["Hata"] = $"Yetersiz jeton. Bu kurs {kurs.Fiyat} jeton gerektiriyor, mevcut jetonunuz: {user.JetonMiktari}.";
                    return RedirectToAction("Detay", "Kurs", new { id = kursId });
                }
                // Jeton düşme ve eğitmene %50 aktarım aynı transaction'da kaydedilir
                var dbUser = await _context.Users.FindAsync(user.Id);
                dbUser!.JetonMiktari -= kurs.Fiyat;

                var egitmen = await _context.Users.FindAsync(kurs.EgitmenId);
                if (egitmen != null)
                    egitmen.JetonMiktari += kurs.Fiyat / 2;
            }

            _context.KursAbonelikler.Add(new KursAbonelik
            {
                KursId = kursId,
                OgrenciId = user.Id,
                OdenenFiyat = kurs.Fiyat
            });

            await _context.SaveChangesAsync();

            TempData["Basari"] = kurs.Fiyat > 0
                ? $"Kursa başarıyla kaydoldunuz. {kurs.Fiyat} jeton harcandı, {kurs.Fiyat / 2} jetonu eğitmene aktarıldı."
                : "Ücretsiz kursa başarıyla kaydoldunuz.";

            return RedirectToAction("Detay", "Kurs", new { id = kursId });
        }

        [Authorize]
        public async Task<IActionResult> DersIzle(int dersId)
        {
            var user = await _userManager.GetUserAsync(User);
            var ders = await _context.Dersler
                .Include(d => d.Kurs)
                    .ThenInclude(k => k!.Dersler.OrderBy(x => x.Sira))
                .FirstOrDefaultAsync(d => d.Id == dersId);

            if (ders == null) return NotFound();

            if (!User.IsInRole("Admin") && !User.IsInRole("Egitmen"))
            {
                var abone = await _context.KursAbonelikler
                    .AnyAsync(a => a.KursId == ders.KursId && a.OgrenciId == user!.Id);
                if (!abone)
                {
                    TempData["Hata"] = "Bu dersi izlemek için kursa abone olmanız gerekmektedir.";
                    return RedirectToAction("Detay", "Kurs", new { id = ders.KursId });
                }
            }

            var ilerleme = await _context.DersIlerlemeleri
                .FirstOrDefaultAsync(i => i.DersId == dersId && i.OgrenciId == user!.Id);

            ViewBag.Tamamlandi = ilerleme?.Tamamlandi ?? false;
            ViewBag.KursDersler = ders.Kurs!.Dersler.ToList();
            return View(ders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IlerlemeGuncelle(int dersId, bool tamamlandi)
        {
            var user = await _userManager.GetUserAsync(User);
            var ilerleme = await _context.DersIlerlemeleri
                .FirstOrDefaultAsync(i => i.DersId == dersId && i.OgrenciId == user!.Id);

            if (ilerleme == null)
            {
                ilerleme = new DersIlerleme
                {
                    DersId = dersId,
                    OgrenciId = user!.Id,
                    Tamamlandi = tamamlandi,
                    TamamlanmaTarihi = tamamlandi ? DateTime.Now : null
                };
                _context.DersIlerlemeleri.Add(ilerleme);
            }
            else
            {
                ilerleme.Tamamlandi = tamamlandi;
                ilerleme.TamamlanmaTarihi = tamamlandi ? DateTime.Now : null;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DersIzle), new { dersId });
        }

        public IActionResult JetonSatinAl(int? paketId)
        {
            ViewBag.Paketler = JetonPaketleri.Listesi;
            var model = new JetonSatinAlViewModel { PaketId = paketId ?? 0 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JetonSatinAl(JetonSatinAlViewModel model)
        {
            ViewBag.Paketler = JetonPaketleri.Listesi;

            if (!ModelState.IsValid) return View(model);

            var paket = JetonPaketleri.Bul(model.PaketId);
            if (paket == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz paket seçimi.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var dbUser = await _context.Users.FindAsync(user.Id);
            dbUser!.JetonMiktari += paket.Jeton;
            await _context.SaveChangesAsync();

            TempData["Basari"] = $"Ödeme başarılı! {paket.Jeton} jeton hesabınıza eklendi.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
