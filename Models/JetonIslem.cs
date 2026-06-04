namespace LMSPlatform.Models
{
    public enum JetonIslemTuru
    {
        Satin = 0,      // Jeton satın alma
        KursGeliri = 1, // Eğitmen kurs satış geliri
        KursOdeme = 2   // Öğrenci kurs satın alma harcaması
    }

    public class JetonIslem
    {
        public int Id { get; set; }

        public string KullaniciId { get; set; } = string.Empty;
        public ApplicationUser? Kullanici { get; set; }

        public JetonIslemTuru Tur { get; set; }

        /// <summary>
        /// Pozitif = jeton eklendi, Negatif = jeton harcandı
        /// </summary>
        public int Miktar { get; set; }

        /// <summary>
        /// İlgili kurs (varsa)
        /// </summary>
        public int? KursId { get; set; }
        public Kurs? Kurs { get; set; }

        /// <summary>
        /// TL tutarı (sadece satın alma için)
        /// </summary>
        public decimal? TLTutar { get; set; }

        public string? Aciklama { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
