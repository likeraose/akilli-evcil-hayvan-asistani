using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess
{
    // Veritabanı köprüsü. ": DbContext" → EF Core'un hazır DbContext sınıfından miras alıyor.
    public class PetAsistanDbContext : DbContext
    {
        // ===== TABLOLAR =====
        // Her DbSet, veritabanında bir tabloyu temsil eder.
        // "Sahipler" → tablonun adı olacak.
        public DbSet<Sahip> Sahipler => Set<Sahip>();
        public DbSet<Hayvan> Hayvanlar => Set<Hayvan>();
        public DbSet<Veteriner> Veterinerler => Set<Veteriner>();
        public DbSet<VeterinerZiyareti> VeterinerZiyaretleri => Set<VeterinerZiyareti>();
        public DbSet<AsiTanimi> AsiTanimlari => Set<AsiTanimi>();
        public DbSet<AsiKaydi> AsiKayitlari => Set<AsiKaydi>();
        public DbSet<IlacKaydi> IlacKayitlari => Set<IlacKaydi>();
        public DbSet<KiloKaydi> KiloKayitlari => Set<KiloKaydi>();
        public DbSet<Hatirlatici> Hatirlaticilar => Set<Hatirlatici>();
        public DbSet<AiSohbetKaydi> AiSohbetKayitlari => Set<AiSohbetKaydi>();

        // ===== BAĞLANTI AYARI =====
        // override: DbContext'teki boş OnConfiguring metodunu kendi ayarımızla dolduruyoruz.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AyarOkuyucu.BaglantiDizesi());
            }
        }

        // ===== TABLO YAPISI AYARLARI =====
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // KALITIM → TEK TABLO (TPH):
            // Kedi, Kopek ve Kus aynı "Hayvanlar" tablosunda saklanır.
            // Hangisi olduğunu "Tur" adlı sütun belirler.
            modelBuilder.Entity<Hayvan>()
                .HasDiscriminator<string>("Tur")
                .HasValue<Kedi>("Kedi")
                .HasValue<Kopek>("Kopek")
                .HasValue<Kus>("Kus");

            // Diğer ayarlar (uzunluklar, ilişkiler, hazır veriler) 3.4'te buraya eklenecek
        }
    }
}