using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMSPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(50)]
        public string Ad { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Soyad { get; set; } = string.Empty;

        public int JetonMiktari { get; set; } = 500;

        public bool EgitmenTalebi { get; set; } = false;

        public bool EgitmenOnaylandi { get; set; } = false;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public string TamAd => $"{Ad} {Soyad}";

        public ICollection<Kurs> Kurslar { get; set; } = new List<Kurs>();
        public ICollection<KursAbonelik> Abonelikler { get; set; } = new List<KursAbonelik>();
        public ICollection<DersIlerleme> DersIlerlemeleri { get; set; } = new List<DersIlerleme>();
    }
}
