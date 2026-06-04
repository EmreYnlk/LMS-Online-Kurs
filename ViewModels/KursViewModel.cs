using LMSPlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.ViewModels
{
    public class KursViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur."), StringLength(200)]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur."), StringLength(2000)]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Resim URL (opsiyonel)")]
        public string? ResimUrl { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Fiyat (Jeton, 0 = Ücretsiz)")]
        public int Fiyat { get; set; } = 0;

        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }

        public SelectList? Kategoriler { get; set; }
    }

    public class KursDetayViewModel
    {
        public Kurs Kurs { get; set; } = null!;
        public bool AbonemVar { get; set; }
        public int TamamlananDersSayisi { get; set; }
        public List<DersIlerleme> Ilerlemeler { get; set; } = new();
    }

    public class AnaSayfaViewModel
    {
        public List<Kurs> Kurslar { get; set; } = new();
        public List<Kategori> Kategoriler { get; set; } = new();
        public int? SeciliKategoriId { get; set; }
        public string? AramaMetni { get; set; }
    }
}
