using LMSPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMSPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kurs> Kurslar { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<KursAbonelik> KursAbonelikler { get; set; }
        public DbSet<DersIlerleme> DersIlerlemeleri { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<KursAbonelik>()
                .HasIndex(k => new { k.KursId, k.OgrenciId })
                .IsUnique();

            builder.Entity<DersIlerleme>()
                .HasIndex(d => new { d.DersId, d.OgrenciId })
                .IsUnique();

            builder.Entity<Kurs>()
                .HasOne(k => k.Egitmen)
                .WithMany(u => u.Kurslar)
                .HasForeignKey(k => k.EgitmenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<KursAbonelik>()
                .HasOne(a => a.Ogrenci)
                .WithMany(u => u.Abonelikler)
                .HasForeignKey(a => a.OgrenciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DersIlerleme>()
                .HasOne(i => i.Ogrenci)
                .WithMany(u => u.DersIlerlemeleri)
                .HasForeignKey(i => i.OgrenciId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
