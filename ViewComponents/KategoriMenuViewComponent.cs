using LMSPlatform.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.ViewComponents
{
    public class KategoriMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public KategoriMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var kategoriler = await _context.Kategoriler
                .Include(k => k.Kurslar)
                .OrderBy(k => k.Ad)
                .ToListAsync();

            return View(kategoriler);
        }
    }
}
