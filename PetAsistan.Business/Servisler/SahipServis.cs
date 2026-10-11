using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Sahiplerle ilgili tüm iş kuralları.
    // ": BaseServis" → Calistir(), Getir() ve hata çevirisi MİRAS olarak geliyor.
    public class SahipServis : BaseServis
    {
        // readonly: Sadece constructor'da atanabilir, sonra değiştirilemez
        private readonly ISahipRepository _sahipRepo;
        private readonly IHayvanRepository _hayvanRepo;

        // Kolaylık constructor'ı: Varsayılan repository'leri kullanır.
        // ": this(...)" → Alttaki asıl constructor'ı çağırır
        public SahipServis() : this(new SahipRepository(), new HayvanRepository())
        {
        }

        // Asıl constructor: Repository'leri DIŞARIDAN, INTERFACE olarak alır
        public SahipServis(ISahipRepository sahipRepo, IHayvanRepository hayvanRepo)
        {
            _sahipRepo = sahipRepo;
            _hayvanRepo = hayvanRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<Sahip>> TumunuGetir()
        {
            return Getir(() => _sahipRepo.TumunuGetir());
        }

        public IslemSonucu<Sahip?> IdIleGetir(int id)
        {
            return Getir(() => _sahipRepo.IdIleGetir(id));
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(Sahip sahip)
        {
            return Calistir(() =>
            {
                ZorunluAlanlariKontrolEt(sahip);

                if (_sahipRepo.TelefonKayitliMi(sahip.Telefon))
                    throw new DogrulamaException("Bu telefon numarası başka bir sahipte kayıtlı.");

                _sahipRepo.Ekle(sahip);
            },
            "Sahip başarıyla eklendi.");
        }

        public IslemSonucu Guncelle(Sahip sahip)
        {
            return Calistir(() =>
            {
                if (sahip.Id <= 0)
                    throw new DogrulamaException("Güncellenecek sahip seçilmedi.");

                ZorunluAlanlariKontrolEt(sahip);

                // Kendi kaydı HARİÇ, başka birinde bu telefon var mı?
                if (_sahipRepo.TelefonKayitliMi(sahip.Telefon, sahip.Id))
                    throw new DogrulamaException("Bu telefon numarası başka bir sahipte kayıtlı.");

                _sahipRepo.Guncelle(sahip);
            },
            "Sahip bilgileri güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() =>
            {
                // SQL hatası beklemeden, önce biz kontrol edip anlaşılır mesaj veriyoruz
                int hayvanSayisi = _hayvanRepo.Bul(h => h.SahipId == id).Count;

                if (hayvanSayisi > 0)
                    throw new DogrulamaException(
                        $"Bu sahibin {hayvanSayisi} kayıtlı hayvanı var. Önce hayvanlarını silin veya başka bir sahibe aktarın.");

                _sahipRepo.Sil(id);
            },
            "Sahip silindi.");
        }

        // ===== YARDIMCI (sadece bu sınıfın içinde kullanılır) =====

        private void ZorunluAlanlariKontrolEt(Sahip sahip)
        {
            if (string.IsNullOrWhiteSpace(sahip.Ad))
                throw new DogrulamaException("Ad alanı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(sahip.Soyad))
                throw new DogrulamaException("Soyad alanı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(sahip.Telefon))
                throw new DogrulamaException("Telefon numarası boş bırakılamaz.");
        }
    }
}