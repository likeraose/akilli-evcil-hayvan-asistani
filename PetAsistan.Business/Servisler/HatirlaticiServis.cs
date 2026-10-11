using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Kullanıcının kendi eklediği hatırlatıcılar
    public class HatirlaticiServis : HayvanaBagliServis
    {
        private readonly IRepository<Hatirlatici> _hatirlaticiRepo;

        public HatirlaticiServis() : this(new GenericRepository<Hatirlatici>(), new HayvanRepository())
        {
        }

        public HatirlaticiServis(IRepository<Hatirlatici> hatirlaticiRepo, IHayvanRepository hayvanRepo)
            : base(hayvanRepo)
        {
            _hatirlaticiRepo = hatirlaticiRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<Hatirlatici>> HayvanaGoreGetir(int hayvanId)
        {
            return Getir(() => _hatirlaticiRepo.Bul(h => h.HayvanId == hayvanId)
                                               .OrderBy(h => h.TamamlandiMi)   // Önce tamamlanmamışlar
                                               .ThenBy(h => h.Tarih)
                                               .ToList());
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(Hatirlatici hatirlatici)
        {
            return Calistir(() =>
            {
                Kontroller(hatirlatici);

                // Yeni bir hatırlatıcı geçmiş bir tarihe kurulamaz
                if (hatirlatici.Tarih.Date < DateTime.Today)
                    throw new DogrulamaException("Hatırlatıcı tarihi geçmişte olamaz.");

                _hatirlaticiRepo.Ekle(hatirlatici);
            },
            "Hatırlatıcı eklendi.");
        }

        public IslemSonucu Guncelle(Hatirlatici hatirlatici)
        {
            return Calistir(() =>
            {
                if (hatirlatici.Id <= 0)
                    throw new DogrulamaException("Güncellenecek hatırlatıcı seçilmedi.");

                Kontroller(hatirlatici);
                _hatirlaticiRepo.Guncelle(hatirlatici);
            },
            "Hatırlatıcı güncellendi.");
        }

        // "Yaptım" butonu için: Hatırlatıcıyı tamamlandı olarak işaretle
        public IslemSonucu TamamlandiIsaretle(int id)
        {
            return Calistir(() =>
            {
                Hatirlatici? hatirlatici = _hatirlaticiRepo.IdIleGetir(id);

                if (hatirlatici == null)
                    throw new InvalidOperationException("Hatırlatıcı bulunamadı.");

                hatirlatici.TamamlandiMi = true;
                _hatirlaticiRepo.Guncelle(hatirlatici);
            },
            "Hatırlatıcı tamamlandı olarak işaretlendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() => _hatirlaticiRepo.Sil(id), "Hatırlatıcı silindi.");
        }

        // ===== YARDIMCI =====

        private void Kontroller(Hatirlatici hatirlatici)
        {
            HayvaniBul(hatirlatici.HayvanId);   // Hayvan seçilmiş mi? (Üst sınıftan miras)

            if (string.IsNullOrWhiteSpace(hatirlatici.Baslik))
                throw new DogrulamaException("Hatırlatıcı başlığı boş bırakılamaz.");
        }
    }
}