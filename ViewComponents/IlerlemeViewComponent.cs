using LMSPlatform.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMSPlatform.Models;

namespace LMSPlatform.ViewComponents
{
    public class IlerlemeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IlerlemeViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int kursId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return View((object)0);

            var kurs = await _context.Kurslar
                .Include(k => k.Dersler)
                .FirstOrDefaultAsync(k => k.Id == kursId);

            if (kurs == null || !kurs.Dersler.Any())
                return View((object)0);

            var toplamDers = kurs.Dersler.Count;
            var dersIds = kurs.Dersler.Select(d => d.Id).ToList();
            var tamamlanan = await _context.DersIlerlemeleri
                .CountAsync(i => i.OgrenciId == user.Id &&
                            dersIds.Contains(i.DersId) &&
                            i.Tamamlandi);

            var yuzde = toplamDers > 0 ? (tamamlanan * 100 / toplamDers) : 0;
            ViewBag.Tamamlanan = tamamlanan;
            ViewBag.Toplam = toplamDers;
            return View((object)yuzde);
        }
    }
}
