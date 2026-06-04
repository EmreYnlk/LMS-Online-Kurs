using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.ViewModels
{
    public class JetonPaketi
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int Jeton { get; set; }
        public decimal Fiyat { get; set; }
        public string? Etiket { get; set; }
        public bool PopulerMi { get; set; }
    }

    public class JetonSatinAlViewModel
    {
        public int PaketId { get; set; }

        [Required(ErrorMessage = "Kart üzerindeki isim zorunludur.")]
        [Display(Name = "Kart Üzerindeki İsim")]
        public string KartSahibi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
        [Display(Name = "Kart Numarası")]
        public string KartNumarasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "AA/YY formatında giriniz.")]
        [Display(Name = "Son Kullanma Tarihi")]
        public string SonKullanma { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV zorunludur.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV 3 haneli olmalıdır.")]
        [Display(Name = "CVV")]
        public string Cvv { get; set; } = string.Empty;
    }

    public static class JetonPaketleri
    {
        public static List<JetonPaketi> Listesi => new()
        {
            new JetonPaketi { Id = 1, Ad = "Başlangıç",  Jeton = 100, Fiyat = 14.99m,  Etiket = null,        PopulerMi = false },
            new JetonPaketi { Id = 2, Ad = "Standart",   Jeton = 250, Fiyat = 29.99m,  Etiket = "En Popüler", PopulerMi = true  },
            new JetonPaketi { Id = 3, Ad = "Premium",    Jeton = 600, Fiyat = 59.99m,  Etiket = "En Avantajlı", PopulerMi = false },
        };

        public static JetonPaketi? Bul(int id) => Listesi.FirstOrDefault(p => p.Id == id);
    }
}
