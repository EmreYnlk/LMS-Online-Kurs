using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPlatform.Models
{
    public class Kurs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kurs başlığı zorunludur."), StringLength(200)]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur."), StringLength(2000)]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Resim URL")]
        public string? ResimUrl { get; set; }

        [Range(0, 10000, ErrorMessage = "Fiyat 0 ile 10000 jeton arasında olmalıdır.")]
        [Display(Name = "Fiyat (Jeton)")]
        public int Fiyat { get; set; } = 0;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Required]
        public int KategoriId { get; set; }

        [ForeignKey(nameof(KategoriId))]
        public Kategori? Kategori { get; set; }

        [Required]
        public string EgitmenId { get; set; } = string.Empty;

        [ForeignKey(nameof(EgitmenId))]
        public ApplicationUser? Egitmen { get; set; }

        public ICollection<Ders> Dersler { get; set; } = new List<Ders>();
        public ICollection<KursAbonelik> Abonelikler { get; set; } = new List<KursAbonelik>();

        public bool Ucretsiz => Fiyat == 0;
        public int AbonelikSayisi => Abonelikler?.Count ?? 0;
    }
}
