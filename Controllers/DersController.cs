using LMSPlatform.Data;
using LMSPlatform.Models;
using LMSPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    [Authorize(Roles = "Egitmen,Admin")]
    public class DersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int kursId)
        {
            var kurs = await _context.Kurslar
                .Include(k => k.Dersler.OrderBy(d => d.Sira))
                .FirstOrDefaultAsync(k => k.Id == kursId);

            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            ViewBag.Kurs = kurs;
            return View(kurs.Dersler.ToList());
        }

        public async Task<IActionResult> Olustur(int kursId)
        {
            var kurs = await _context.Kurslar.FindAsync(kursId);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            var model = new DersViewModel { KursId = kursId, KursBaslik = kurs.Baslik };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur(DersViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var kurs = await _context.Kurslar.FindAsync(model.KursId);
            if (kurs == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && kurs.EgitmenId != user!.Id)
                return Forbid();

            var ders = new Ders
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                VideoUrl = model.VideoUrl,
                Sira = model.Sira,
                KursId = model.KursId
            };

            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Ders eklendi.";
            return RedirectToAction(nameof(Index), new { kursId = model.KursId });
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var ders = await _context.Dersler.Include(d => d.Kurs).FirstOrDefaultAsync(d => d.Id == id);
            if (ders == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && ders.Kurs!.EgitmenId != user!.Id)
                return Forbid();

            var model = new DersViewModel
            {
                Id = ders.Id,
                Baslik = ders.Baslik,
                Aciklama = ders.Aciklama,
                VideoUrl = ders.VideoUrl,
                Sira = ders.Sira,
                KursId = ders.KursId,
                KursBaslik = ders.Kurs!.Baslik
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, DersViewModel model)
        {
            var ders = await _context.Dersler.Include(d => d.Kurs).FirstOrDefaultAsync(d => d.Id == id);
            if (ders == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && ders.Kurs!.EgitmenId != user!.Id)
                return Forbid();

            if (!ModelState.IsValid) return View(model);

            ders.Baslik = model.Baslik;
            ders.Aciklama = model.Aciklama;
            ders.VideoUrl = model.VideoUrl;
            ders.Sira = model.Sira;

            await _context.SaveChangesAsync();
            TempData["Basari"] = "Ders güncellendi.";
            return RedirectToAction(nameof(Index), new { kursId = ders.KursId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var ders = await _context.Dersler.Include(d => d.Kurs).FirstOrDefaultAsync(d => d.Id == id);
            if (ders == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Egitmen") && ders.Kurs!.EgitmenId != user!.Id)
                return Forbid();

            var kursId = ders.KursId;
            _context.Dersler.Remove(ders);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Ders silindi.";
            return RedirectToAction(nameof(Index), new { kursId });
        }
    }
}
