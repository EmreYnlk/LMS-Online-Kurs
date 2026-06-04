using System.Diagnostics;
using LMSPlatform.Data;
using LMSPlatform.Models;
using LMSPlatform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? kategoriId, string? ara)
        {
            var query = _context.Kurslar
                .Include(k => k.Kategori)
                .Include(k => k.Egitmen)
                .Include(k => k.Abonelikler)
                .AsQueryable();

            if (kategoriId.HasValue)
                query = query.Where(k => k.KategoriId == kategoriId.Value);

            if (!string.IsNullOrEmpty(ara))
                query = query.Where(k => k.Baslik.Contains(ara) || k.Aciklama.Contains(ara));

            var model = new AnaSayfaViewModel
            {
                Kurslar = await query.OrderByDescending(k => k.OlusturmaTarihi).ToListAsync(),
                Kategoriler = await _context.Kategoriler.ToListAsync(),
                SeciliKategoriId = kategoriId,
                AramaMetni = ara
            };

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
