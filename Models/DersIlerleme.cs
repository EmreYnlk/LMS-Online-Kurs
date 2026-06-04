using System.ComponentModel.DataAnnotations.Schema;

namespace LMSPlatform.Models
{
    public class DersIlerleme
    {
        public int Id { get; set; }

        public int DersId { get; set; }

        [ForeignKey(nameof(DersId))]
        public Ders? Ders { get; set; }

        public string OgrenciId { get; set; } = string.Empty;

        [ForeignKey(nameof(OgrenciId))]
        public ApplicationUser? Ogrenci { get; set; }

        public bool Tamamlandi { get; set; } = false;

        public DateTime? TamamlanmaTarihi { get; set; }
    }
}
