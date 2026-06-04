using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.ViewModels
{
    public class DersViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ders başlığı zorunludur."), StringLength(200)]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Video URL zorunludur."), StringLength(500)]
        [Display(Name = "YouTube Video URL")]
        public string VideoUrl { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Sıra Numarası")]
        public int Sira { get; set; } = 1;

        public int KursId { get; set; }
        public string? KursBaslik { get; set; }
    }
}
