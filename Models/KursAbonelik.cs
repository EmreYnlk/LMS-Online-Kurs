using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPlatform.Models
{
    public class KursAbonelik
    {
        public int Id { get; set; }

        public int KursId { get; set; }

        [ForeignKey(nameof(KursId))]
        public Kurs? Kurs { get; set; }

        public string OgrenciId { get; set; } = string.Empty;

        [ForeignKey(nameof(OgrenciId))]
        public ApplicationUser? Ogrenci { get; set; }

        public DateTime AbonelikTarihi { get; set; } = DateTime.Now;

        public int OdenenFiyat { get; set; } = 0;
    }
}
