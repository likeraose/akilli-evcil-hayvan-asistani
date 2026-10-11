using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // ": HayvanaBagliServis" → HayvaniBul(), Calistir(), Getir() miras olarak geliyor
    public class IlacServis : HayvanaBagliServis
    {
        private readonly IRepository<IlacKaydi> _ilacRepo;
        private readonly IVeterinerZiyaretiRepository _ziyaretRepo;

        public IlacServis()
            : this(new GenericRepository<IlacKaydi>(), new HayvanRepository(), new VeterinerZiyaretiRepository())
        {
        }

        // ": base(hayvanRepo)" → Üst sınıfın (HayvanaBagliServis) constructor'ına hayvan repository'sini iletir
        public IlacServis(IRepository<IlacKaydi> ilacRepo,
                          IHayvanRepository hayvanRepo,
                          IVeterinerZiyaretiRepository ziyaretRepo)
            : base(hayvanRepo)
        {
            _ilacRepo = ilacRepo;
            _ziyaretRepo = ziyaretRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<IlacKaydi>> HayvanaGoreGetir(int hayvanId)
        {
            return Getir(() => _ilacRepo.Bul(i => i.HayvanId == hayvanId)
                                        .OrderByDescending(i => i.BaslangicTarihi)
                                        .ToList());
        }

        // Sadece DEVAM EDEN tedaviler
        public IslemSonucu<List<IlacKaydi>> DevamEdenleriGetir(int hayvanId)
        {
            return Getir(() => _ilacRepo.Bul(i => i.HayvanId == hayvanId)   // 1) Veritabanından getir
                                        .Where(i => i.DevamEdiyorMu)          // 2) Bellekte filtrele
                                        .OrderBy(i => i.BitisTarihi)
                                        .ToList());
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(IlacKaydi kayit)
        {
            return Calistir(() =>
            {
                Kontroller(kayit);
                _ilacRepo.Ekle(kayit);
            },
            "İlaç kaydı eklendi.");
        }

        public IslemSonucu Guncelle(IlacKaydi kayit)
        {
            return Calistir(() =>
            {
                if (kayit.Id <= 0)
                    throw new DogrulamaException("Güncellenecek ilaç kaydı seçilmedi.");

                Kontroller(kayit);
                _ilacRepo.Guncelle(kayit);
            },
            "İlaç kaydı güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() => _ilacRepo.Sil(id), "İlaç kaydı silindi.");
        }

        // ===== YARDIMCI =====

        private void Kontroller(IlacKaydi kayit)
        {
            Hayvan hayvan = HayvaniBul(kayit.HayvanId);   // ← Üst sınıftan miras gelen metot

            if (string.IsNullOrWhiteSpace(kayit.IlacAdi))
                throw new DogrulamaException("İlaç adı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(kayit.Doz))
                throw new DogrulamaException("Doz bilgisi boş bırakılamaz.");

            if (kayit.BaslangicTarihi.Date < hayvan.DogumTarihi.Date)
                throw new DogrulamaException("İlaç başlangıç tarihi, hayvanın doğum tarihinden önce olamaz.");

            // İki alanlı kural
            if (kayit.BitisTarihi.HasValue && kayit.BitisTarihi.Value.Date < kayit.BaslangicTarihi.Date)
                throw new DogrulamaException("Bitiş tarihi, başlangıç tarihinden önce olamaz.");

            // İlaç bir ziyarete bağlanıyorsa, o ziyaret AYNI hayvana ait olmalı
            if (kayit.VeterinerZiyaretiId.HasValue)
            {
                VeterinerZiyareti? ziyaret = _ziyaretRepo.IdIleGetir(kayit.VeterinerZiyaretiId.Value);

                if (ziyaret == null || ziyaret.HayvanId != kayit.HayvanId)
                    throw new DogrulamaException("Seçilen veteriner ziyareti bu hayvana ait değil.");
            }
        }
    }
}