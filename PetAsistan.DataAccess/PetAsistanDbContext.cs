using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess
{
    // Veritabanı köprüsü. ": DbContext" → EF Core'un hazır DbContext sınıfından miras alıyor.
    public class PetAsistanDbContext : DbContext
    {
        // ===== TABLOLAR =====
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AyarOkuyucu.BaglantiDizesi());
            }
        }

        // ===== GENEL KURALLAR =====
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Tüm decimal alanlar: toplam 10 basamak, virgülden sonra 2 basamak (örn: 12345678.90)
            configurationBuilder.Properties<decimal>().HavePrecision(10, 2);
        }

        // ===== TABLO YAPISI AYARLARI =====
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- 1) KALITIM → TEK TABLO (TPH) ----------
            modelBuilder.Entity<Hayvan>()
                .HasDiscriminator<string>("Tur")
                .HasValue<Kedi>("Kedi")
                .HasValue<Kopek>("Kopek")
                .HasValue<Kus>("Kus");

            // ---------- 2) SAHİP ----------
            modelBuilder.Entity<Sahip>(e =>
            {
                e.Property(s => s.Ad).HasMaxLength(50).IsRequired();
                e.Property(s => s.Soyad).HasMaxLength(50).IsRequired();
                e.Property(s => s.Telefon).HasMaxLength(11).IsRequired();
                e.Property(s => s.Eposta).HasMaxLength(100);
                e.Property(s => s.Adres).HasMaxLength(300);

                // Aynı telefon numarasıyla iki sahip kaydedilemesin
                e.HasIndex(s => s.Telefon).IsUnique();
            });

            // ---------- 3) HAYVAN ----------
            modelBuilder.Entity<Hayvan>(e =>
            {
                e.Property(h => h.Ad).HasMaxLength(50).IsRequired();
                e.Property(h => h.Cins).HasMaxLength(50);
                e.Property(h => h.Notlar).HasMaxLength(1000);
                e.Property(h => h.Kilo).HasPrecision(7, 3);   // Kuş kilosu için 3 ondalık: 0.035

                // Sahip (1) → Hayvan (N). Hayvanları olan sahip SİLİNEMEZ.
                e.HasOne(h => h.Sahip)
                 .WithMany(s => s.Hayvanlar)
                 .HasForeignKey(h => h.SahipId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- 4) VETERİNER ----------
            modelBuilder.Entity<Veteriner>(e =>
            {
                e.Property(v => v.VeterinerAdi).HasMaxLength(100).IsRequired();
                e.Property(v => v.KlinikAdi).HasMaxLength(100).IsRequired();
                e.Property(v => v.Telefon).HasMaxLength(11).IsRequired();
                e.Property(v => v.Adres).HasMaxLength(300);
            });

            // ---------- 5) VETERİNER ZİYARETİ ----------
            modelBuilder.Entity<VeterinerZiyareti>(e =>
            {
                e.Property(z => z.Sikayet).HasMaxLength(500).IsRequired();
                e.Property(z => z.Teshis).HasMaxLength(1000);
                e.Property(z => z.Tedavi).HasMaxLength(1000);

                // Hayvan silinirse ziyaretleri de silinir
                e.HasOne(z => z.Hayvan)
                 .WithMany(h => h.Ziyaretler)
                 .HasForeignKey(z => z.HayvanId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Ziyaret kaydı olan veteriner SİLİNEMEZ (sağlık geçmişi kaybolmasın)
                e.HasOne(z => z.Veteriner)
                 .WithMany(v => v.Ziyaretler)
                 .HasForeignKey(z => z.VeterinerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- 6) AŞI TANIMI ----------
            modelBuilder.Entity<AsiTanimi>(e =>
            {
                e.Property(a => a.Ad).HasMaxLength(100).IsRequired();
                e.Property(a => a.Aciklama).HasMaxLength(500);
                e.HasIndex(a => a.Ad).IsUnique();   // Aynı isimde iki aşı türü olmasın
            });

            // ---------- 7) AŞI KAYDI ----------
            modelBuilder.Entity<AsiKaydi>(e =>
            {
                e.Property(a => a.SeriNo).HasMaxLength(50);
                e.Property(a => a.Notlar).HasMaxLength(500);

                e.HasOne(a => a.Hayvan)
                 .WithMany(h => h.AsiKayitlari)
                 .HasForeignKey(a => a.HayvanId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Kullanılmış bir aşı türü SİLİNEMEZ
                e.HasOne(a => a.AsiTanimi)
                 .WithMany(t => t.AsiKayitlari)
                 .HasForeignKey(a => a.AsiTanimiId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Veteriner silinirse aşı kaydı kalır, sadece veteriner alanı boşalır
                e.HasOne(a => a.Veteriner)
                 .WithMany()
                 .HasForeignKey(a => a.VeterinerId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------- 8) İLAÇ KAYDI ----------
            modelBuilder.Entity<IlacKaydi>(e =>
            {
                e.Property(i => i.IlacAdi).HasMaxLength(100).IsRequired();
                e.Property(i => i.Doz).HasMaxLength(100).IsRequired();
                e.Property(i => i.Notlar).HasMaxLength(500);

                e.HasOne(i => i.Hayvan)
                 .WithMany(h => h.IlacKayitlari)
                 .HasForeignKey(i => i.HayvanId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Ziyaret silinirse ilaç kaydı kalır, ziyaret bağlantısını EF Core boşaltır.
                // (SQL Server burada veritabanı tarafında zincirleme işleme izin vermediği için ClientSetNull)
                e.HasOne(i => i.VeterinerZiyareti)
                 .WithMany()
                 .HasForeignKey(i => i.VeterinerZiyaretiId)
                 .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ---------- 9) KİLO KAYDI ----------
            modelBuilder.Entity<KiloKaydi>(e =>
            {
                e.Property(k => k.Kilo).HasPrecision(7, 3);
                e.Property(k => k.Notlar).HasMaxLength(500);

                e.HasOne(k => k.Hayvan)
                 .WithMany(h => h.KiloKayitlari)
                 .HasForeignKey(k => k.HayvanId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- 10) HATIRLATICI ----------
            modelBuilder.Entity<Hatirlatici>(e =>
            {
                e.Property(h => h.Baslik).HasMaxLength(100).IsRequired();
                e.Property(h => h.Aciklama).HasMaxLength(500);

                e.HasOne(h => h.Hayvan)
                 .WithMany(hy => hy.Hatirlaticilar)
                 .HasForeignKey(h => h.HayvanId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- 11) AI SOHBET KAYDI ----------
            modelBuilder.Entity<AiSohbetKaydi>(e =>
            {
                e.Property(a => a.Soru).HasMaxLength(2000).IsRequired();
                // Cevap uzun olabileceği için sınır koymuyoruz

                // Hayvan silinirse sohbet geçmişi kalır, hayvan bağlantısı boşalır
                e.HasOne(a => a.Hayvan)
                 .WithMany(h => h.AiSohbetKayitlari)
                 .HasForeignKey(a => a.HayvanId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------- 12) HAZIR VERİLER: AŞI TÜRLERİ ----------
            // Sabit bir tarih kullanıyoruz (DateTime.Now kullanılırsa her migration'da değişir)
            DateTime eklenmeTarihi = new DateTime(2026, 1, 1);

            modelBuilder.Entity<AsiTanimi>().HasData(
                new
                {
                    Id = 1,
                    Ad = "Kedi Karma Aşısı",
                    HedefTur = AsiHedefTuru.Kedi,
                    TekrarAraligiGun = 365,
                    Aciklama = "Panlökopeni, herpes ve calici virüslerine karşı koruma sağlar.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 2,
                    Ad = "Kedi Lösemi (FeLV) Aşısı",
                    HedefTur = AsiHedefTuru.Kedi,
                    TekrarAraligiGun = 365,
                    Aciklama = "Kedi lösemi virüsüne karşı koruma sağlar.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 3,
                    Ad = "Kedi Kuduz Aşısı",
                    HedefTur = AsiHedefTuru.Kedi,
                    TekrarAraligiGun = 365,
                    Aciklama = "Kuduz hastalığına karşı yıllık aşı.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 4,
                    Ad = "Köpek Karma Aşısı",
                    HedefTur = AsiHedefTuru.Kopek,
                    TekrarAraligiGun = 365,
                    Aciklama = "Gençlik hastalığı (distemper), parvo, hepatit ve parainfluenzaya karşı koruma sağlar.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 5,
                    Ad = "Köpek Kuduz Aşısı",
                    HedefTur = AsiHedefTuru.Kopek,
                    TekrarAraligiGun = 365,
                    Aciklama = "Kuduz hastalığına karşı yıllık aşı.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 6,
                    Ad = "Köpek Bronşin Aşısı",
                    HedefTur = AsiHedefTuru.Kopek,
                    TekrarAraligiGun = 365,
                    Aciklama = "Bulaşıcı köpek öksürüğüne (kennel cough) karşı koruma sağlar.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 7,
                    Ad = "Köpek Leptospiroz Aşısı",
                    HedefTur = AsiHedefTuru.Kopek,
                    TekrarAraligiGun = 365,
                    Aciklama = "Leptospiroz bakteriyel enfeksiyonuna karşı koruma sağlar.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 8,
                    Ad = "İç Parazit Uygulaması",
                    HedefTur = AsiHedefTuru.Hepsi,
                    TekrarAraligiGun = 90,
                    Aciklama = "Bağırsak parazitlerine karşı, genellikle 3 ayda bir tekrarlanır.",
                    OlusturmaTarihi = eklenmeTarihi
                },

                new
                {
                    Id = 9,
                    Ad = "Dış Parazit Uygulaması",
                    HedefTur = AsiHedefTuru.Hepsi,
                    TekrarAraligiGun = 30,
                    Aciklama = "Pire ve keneye karşı, genellikle ayda bir tekrarlanır.",
                    OlusturmaTarihi = eklenmeTarihi
                }
            );
        }
    }
}