using LMSPlatform.Data;
using LMSPlatform.Models;
using LMSPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    public class KursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KursController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Egitmen,Admin")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            IQueryable<Kurs> query = _context.Kurslar
                .Include(k => k.Kategori)
                .Include(k => k.Abonelikler);

            if (User.IsInRole("Egitmen"))
                query = query.Where(k => k.EgitmenId == user!.Id);

            return View(await query.OrderByDescending(k => k.OlusturmaTarihi).ToListAsync());
        }

        public async Task<IActionResult> Detay(int id)
        {
            var kurs = await _context.Kurslar
                .Include(k => k.Kategori)
                .Include(k => k.Egitmen)
                .Include(k => k.Dersler.OrderBy(d => d.Sira))
                .Include(k => k.Abonelikler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool aboneVar = false;
            var ilerlemeler = new List<DersIlerleme>();

            if (user != null)
            {
                aboneVar = await _context.KursAbonelikler
                    .AnyAsync(a => a.KursId == id && a.OgrenciId == user.Id);

                if (aboneVar)
                {
                    var dersIds = kurs.Dersler.Select(d => d.Id).ToList();
                    ilerlemeler = await _context.DersIlerlemeleri
                        .Where(i => i.OgrenciId == user.Id && dersIds.Contains(i.DersId))
                        .ToListAsync();
                }
            }

            var model = new KursDetayViewModel
            {
                Kurs = kurs,
                AbonemVar = aboneVar,
                TamamlananDersSayisi = ilerlemeler.Count(i => i.Tamamlandi),
                Ilerlemeler = ilerlemeler
            };

            return View(model);
        }

        [Authorize(Roles = "Egitmen,Admin")]
        public async Task<IActionResult> Olustur()
        {
            var model = new KursViewModel
            {
                Kategoriler = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad")
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Egitmen,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur(KursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Kategoriler = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var kurs = new Kurs
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                ResimUrl = model.ResimUrl,
                Fiyat = model.Fiyat,
                KategoriId = model.KategoriId,
                EgitmenId = user!.Id
            };

            _context.Kurslar.Add(kurs);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kurs başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Egitmen,Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var kurs = await _context.Kurslar.FindAsync(id);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            var model = new KursViewModel
            {
                Id = kurs.Id,
                Baslik = kurs.Baslik,
                Aciklama = kurs.Aciklama,
                ResimUrl = kurs.ResimUrl,
                Fiyat = kurs.Fiyat,
                KategoriId = kurs.KategoriId,
                Kategoriler = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad", kurs.KategoriId)
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Egitmen,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, KursViewModel model)
        {
            var kurs = await _context.Kurslar.FindAsync(id);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            if (!ModelState.IsValid)
            {
                model.Kategoriler = new SelectList(await _context.Kategoriler.ToListAsync(), "Id", "Ad");
                return View(model);
            }

            kurs.Baslik = model.Baslik;
            kurs.Aciklama = model.Aciklama;
            kurs.ResimUrl = model.ResimUrl;
            kurs.Fiyat = model.Fiyat;
            kurs.KategoriId = model.KategoriId;

            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kurs güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Egitmen,Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var kurs = await _context.Kurslar.Include(k => k.Kategori).FirstOrDefaultAsync(k => k.Id == id);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            return View(kurs);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Egitmen,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnayla(int id)
        {
            var kurs = await _context.Kurslar.FindAsync(id);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            _context.Kurslar.Remove(kurs);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kurs silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
