using LMSPlatform.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.ViewComponents
{
    public class PopulerKurslarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PopulerKurslarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int adet = 5)
        {
            var kurslar = await _context.Kurslar
                .Include(k => k.Kategori)
                .Include(k => k.Egitmen)
                .Include(k => k.Abonelikler)
                .OrderByDescending(k => k.Abonelikler.Count)
                .Take(adet)
                .ToListAsync();

            return View(kurslar);
        }
    }
}
