using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    public class KiloServis : HayvanaBagliServis
    {
        private readonly IRepository<KiloKaydi> _kiloRepo;

        public KiloServis() : this(new GenericRepository<KiloKaydi>(), new HayvanRepository())
        {
        }

        public KiloServis(IRepository<KiloKaydi> kiloRepo, IHayvanRepository hayvanRepo)
            : base(hayvanRepo)
        {
            _kiloRepo = kiloRepo;
        }

        // ===== OKUMA =====

        // Eskiden yeniye sıralı (grafik soldan sağa çizilsin diye)
        public IslemSonucu<List<KiloKaydi>> HayvanaGoreGetir(int hayvanId)
        {
            return Getir(() => _kiloRepo.Bul(k => k.HayvanId == hayvanId)
                                        .OrderBy(k => k.OlcumTarihi)
                                        .ToList());
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(KiloKaydi kayit)
        {
            return Calistir(() =>
            {
                Kontroller(kayit);
                _kiloRepo.Ekle(kayit);
                GuncelKiloyuSenkronizeEt(kayit.HayvanId);
            },
            "Kilo ölçümü kaydedildi.");
        }

        public IslemSonucu Guncelle(KiloKaydi kayit)
        {
            return Calistir(() =>
            {
                if (kayit.Id <= 0)
                    throw new DogrulamaException("Güncellenecek ölçüm seçilmedi.");

                Kontroller(kayit);
                _kiloRepo.Guncelle(kayit);
                GuncelKiloyuSenkronizeEt(kayit.HayvanId);
            },
            "Kilo ölçümü güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() =>
            {
                // Silmeden ÖNCE hangi hayvana ait olduğunu öğren
                KiloKaydi? kayit = _kiloRepo.IdIleGetir(id);

                if (kayit == null)
                    throw new InvalidOperationException("Silinmek istenen ölçüm bulunamadı.");

                _kiloRepo.Sil(id);
                GuncelKiloyuSenkronizeEt(kayit.HayvanId);
            },
            "Kilo ölçümü silindi.");
        }

        // ===== YARDIMCI =====

        private void Kontroller(KiloKaydi kayit)
        {
            Hayvan hayvan = HayvaniBul(kayit.HayvanId);

            if (kayit.Kilo <= 0)
                throw new DogrulamaException("Lütfen kiloyu girin.");

            // 2.5-B'de söz verdiğimiz kural: TÜRE ÖZEL üst sınır.
            // MaksimumKilo abstract → Kedi ise 15, Köpek ise 100, Kuş ise 2 (çok biçimlilik!)
            if (kayit.Kilo > hayvan.MaksimumKilo)
                throw new DogrulamaException($"{hayvan.TurAdi} için en fazla {hayvan.MaksimumKilo} kg girilebilir.");

            if (kayit.OlcumTarihi.Date < hayvan.DogumTarihi.Date)
                throw new DogrulamaException("Ölçüm tarihi, hayvanın doğum tarihinden önce olamaz.");
        }

        // İŞ KURALI: Hayvanın güncel kilosu = en son ölçümdeki kilo
        private void GuncelKiloyuSenkronizeEt(int hayvanId)
        {
            KiloKaydi? sonOlcum = _kiloRepo.Bul(k => k.HayvanId == hayvanId)
                                           .OrderByDescending(k => k.OlcumTarihi)
                                           .ThenByDescending(k => k.Id)
                                           .FirstOrDefault();

            if (sonOlcum == null)
                return;   // Hiç ölçüm kalmadıysa hayvanın kilosuna dokunma

            Hayvan hayvan = HayvaniBul(hayvanId);

            if (hayvan.Kilo != sonOlcum.Kilo)
            {
                hayvan.Kilo = sonOlcum.Kilo;
                HayvanRepo.Guncelle(hayvan);
            }
        }
    }
}