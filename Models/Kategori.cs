using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.Models
{
    public class Kategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur."), StringLength(100)]
        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        public ICollection<Kurs> Kurslar { get; set; } = new List<Kurs>();
    }
}
