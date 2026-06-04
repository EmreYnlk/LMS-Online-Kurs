using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPlatform.Models
{
    public class Ders
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ders başlığı zorunludur."), StringLength(200)]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Video URL zorunludur."), StringLength(500)]
        [Display(Name = "Video URL (YouTube)")]
        public string VideoUrl { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Sıra")]
        public int Sira { get; set; } = 1;

        [Required]
        public int KursId { get; set; }

        [ForeignKey(nameof(KursId))]
        public Kurs? Kurs { get; set; }

        public ICollection<DersIlerleme> Ilerlemeler { get; set; } = new List<DersIlerleme>();

        public string YoutubeEmbedUrl => GetEmbedUrl(VideoUrl);

        private static string GetEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (url.Contains("youtube.com/watch?v="))
                return url.Replace("youtube.com/watch?v=", "youtube.com/embed/").Split('&')[0];

            if (url.Contains("youtu.be/"))
            {
                var id = url.Split("youtu.be/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{id}";
            }

            return url;
        }
    }
}
