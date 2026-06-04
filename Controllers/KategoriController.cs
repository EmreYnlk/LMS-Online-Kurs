using LMSPlatform.Data;
using LMSPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KategoriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategoriler.Include(k => k.Kurslar).ToListAsync());
        }

        public IActionResult Olustur() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur(Kategori model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Kategoriler.Add(model);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kategori oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Kategori model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Kategoriler.Update(model);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var kategori = await _context.Kategoriler.Include(k => k.Kurslar).FirstOrDefaultAsync(k => k.Id == id);
            if (kategori == null) return NotFound();

            if (kategori.Kurslar.Any())
            {
                TempData["Hata"] = "Bu kategoriye ait kurslar var, önce kursları siliniz.";
                return RedirectToAction(nameof(Index));
            }

            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
