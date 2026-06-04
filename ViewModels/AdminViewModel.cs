using LMSPlatform.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int ToplamKullanici { get; set; }
        public int ToplamKurs { get; set; }
        public int BekleyenEgitmen { get; set; }
        public int ToplamAbonelik { get; set; }
    }

    public class KullaniciListeViewModel
    {
        public ApplicationUser Kullanici { get; set; } = null!;
        public IList<string> Roller { get; set; } = new List<string>();
    }

    public class JetonEkleViewModel
    {
        public string KullaniciId { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public int MevcutJeton { get; set; }
        [Required(ErrorMessage = "Jeton miktarı zorunludur.")]
        [Range(1, 10000, ErrorMessage = "Jeton miktarı 1 ile 10000 arasında olmalıdır.")]
        [Display(Name = "Eklenecek Jeton")]
        public int EklenecekJeton { get; set; }
    }
}
