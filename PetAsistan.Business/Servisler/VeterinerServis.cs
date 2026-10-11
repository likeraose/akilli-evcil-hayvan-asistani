using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    public class VeterinerServis : BaseServis
    {
        private readonly IRepository<Veteriner> _veterinerRepo;
        private readonly IVeterinerZiyaretiRepository _ziyaretRepo;

        public VeterinerServis()
            : this(new GenericRepository<Veteriner>(), new VeterinerZiyaretiRepository())
        {
        }

        public VeterinerServis(IRepository<Veteriner> veterinerRepo, IVeterinerZiyaretiRepository ziyaretRepo)
        {
            _veterinerRepo = veterinerRepo;
            _ziyaretRepo = ziyaretRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<Veteriner>> TumunuGetir()
        {
            return Getir(() => _veterinerRepo.TumunuGetir()
                                             .OrderBy(v => v.VeterinerAdi)
                                             .ToList());
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(Veteriner veteriner)
        {
            return Calistir(() =>
            {
                ZorunluAlanlariKontrolEt(veteriner);
                _veterinerRepo.Ekle(veteriner);
            },
            "Veteriner başarıyla eklendi.");
        }

        public IslemSonucu Guncelle(Veteriner veteriner)
        {
            return Calistir(() =>
            {
                if (veteriner.Id <= 0)
                    throw new DogrulamaException("Güncellenecek veteriner seçilmedi.");

                ZorunluAlanlariKontrolEt(veteriner);
                _veterinerRepo.Guncelle(veteriner);
            },
            "Veteriner bilgileri güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() =>
            {
                int ziyaretSayisi = _ziyaretRepo.Bul(z => z.VeterinerId == id).Count;

                if (ziyaretSayisi > 0)
                    throw new DogrulamaException(
                        $"Bu veterinerin {ziyaretSayisi} ziyaret kaydı var. Sağlık geçmişi kaybolmasın diye silinemez.");

                _veterinerRepo.Sil(id);
            },
            "Veteriner silindi.");
        }

        // ===== YARDIMCI =====

        private void ZorunluAlanlariKontrolEt(Veteriner veteriner)
        {
            if (string.IsNullOrWhiteSpace(veteriner.VeterinerAdi))
                throw new DogrulamaException("Veteriner adı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(veteriner.KlinikAdi))
                throw new DogrulamaException("Klinik adı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(veteriner.Telefon))
                throw new DogrulamaException("Telefon numarası boş bırakılamaz.");
        }
    }
}