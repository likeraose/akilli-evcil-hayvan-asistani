using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Hayvanlarla ilgili tüm iş kuralları
    public class HayvanServis : BaseServis
    {
        private readonly IHayvanRepository _hayvanRepo;
        private readonly ISahipRepository _sahipRepo;
        private readonly IRepository<KiloKaydi> _kiloRepo;

        public HayvanServis()
            : this(new HayvanRepository(), new SahipRepository(), new GenericRepository<KiloKaydi>())
        {
        }

        public HayvanServis(IHayvanRepository hayvanRepo, ISahipRepository sahipRepo, IRepository<KiloKaydi> kiloRepo)
        {
            _hayvanRepo = hayvanRepo;
            _sahipRepo = sahipRepo;
            _kiloRepo = kiloRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<Hayvan>> TumunuGetir()
        {
            return Getir(() => _hayvanRepo.TumunuGetir());
        }

        public IslemSonucu<List<Hayvan>> SahibeGoreGetir(int sahipId)
        {
            return Getir(() => _hayvanRepo.SahibeGoreGetir(sahipId));
        }

        // Hayvanı tüm sağlık geçmişiyle birlikte getirir (detay ekranı için)
        public IslemSonucu<Hayvan> TumBilgileriyleGetir(int id)
        {
            return Getir(() =>
            {
                Hayvan? hayvan = _hayvanRepo.TumBilgileriyleGetir(id);

                if (hayvan == null)
                    throw new InvalidOperationException("Hayvan bulunamadı.");

                return hayvan;
            });
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(Hayvan hayvan)
        {
            return Calistir(() =>
            {
                OrtakKontroller(hayvan);

                _hayvanRepo.Ekle(hayvan);
                // ↑ Bu satırdan sonra hayvan.Id veritabanının verdiği numarayla dolu

                // İŞ KURALI: İlk kilo kaydını otomatik oluştur (kilo grafiğinin başlangıç noktası)
                _kiloRepo.Ekle(new KiloKaydi
                {
                    HayvanId = hayvan.Id,
                    Kilo = hayvan.Kilo,
                    OlcumTarihi = DateTime.Today,
                    Notlar = "İlk kayıt"
                });
            },
            $"{hayvan.Ad} başarıyla eklendi.");
        }

        public IslemSonucu Guncelle(Hayvan hayvan)
        {
            return Calistir(() =>
            {
                if (hayvan.Id <= 0)
                    throw new DogrulamaException("Güncellenecek hayvan seçilmedi.");

                OrtakKontroller(hayvan);

                // Veritabanındaki ESKİ halini al (kilo değişti mi diye bakmak için)
                Hayvan? eskiHali = _hayvanRepo.IdIleGetir(hayvan.Id);

                _hayvanRepo.Guncelle(hayvan);

                // İŞ KURALI: Kilo değiştiyse kilo geçmişine otomatik kayıt ekle
                if (eskiHali != null && eskiHali.Kilo != hayvan.Kilo)
                {
                    _kiloRepo.Ekle(new KiloKaydi
                    {
                        HayvanId = hayvan.Id,
                        Kilo = hayvan.Kilo,
                        OlcumTarihi = DateTime.Today,
                        Notlar = "Bilgi güncellemesi"
                    });
                }
            },
            $"{hayvan.Ad} bilgileri güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            // Cascade kuralı sayesinde aşı, ilaç, kilo, ziyaret ve hatırlatıcı kayıtları da silinir
            return Calistir(() => _hayvanRepo.Sil(id),
                "Hayvan ve tüm sağlık kayıtları silindi.");
        }

        // ===== YARDIMCI =====

        private void OrtakKontroller(Hayvan hayvan)
        {
            if (string.IsNullOrWhiteSpace(hayvan.Ad))
                throw new DogrulamaException("Hayvanın adı boş bırakılamaz.");

            if (hayvan.SahipId <= 0 || !_sahipRepo.VarMi(s => s.Id == hayvan.SahipId))
                throw new DogrulamaException("Lütfen hayvanın sahibini seçin.");

            if (hayvan.Kilo <= 0)
                throw new DogrulamaException("Lütfen hayvanın kilosunu girin.");
        }
    }
}