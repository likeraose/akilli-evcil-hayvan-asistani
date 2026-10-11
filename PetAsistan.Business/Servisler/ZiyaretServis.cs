using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    public class ZiyaretServis : BaseServis
    {
        private readonly IVeterinerZiyaretiRepository _ziyaretRepo;
        private readonly IHayvanRepository _hayvanRepo;
        private readonly IRepository<Veteriner> _veterinerRepo;

        public ZiyaretServis()
            : this(new VeterinerZiyaretiRepository(), new HayvanRepository(), new GenericRepository<Veteriner>())
        {
        }

        public ZiyaretServis(IVeterinerZiyaretiRepository ziyaretRepo,
                             IHayvanRepository hayvanRepo,
                             IRepository<Veteriner> veterinerRepo)
        {
            _ziyaretRepo = ziyaretRepo;
            _hayvanRepo = hayvanRepo;
            _veterinerRepo = veterinerRepo;
        }

        // ===== OKUMA =====

        public IslemSonucu<List<VeterinerZiyareti>> TumunuGetir()
        {
            return Getir(() => _ziyaretRepo.TumunuGetir());
        }

        public IslemSonucu<List<VeterinerZiyareti>> HayvanaGoreGetir(int hayvanId)
        {
            return Getir(() => _ziyaretRepo.HayvanaGoreGetir(hayvanId));
        }

        // ===== YAZMA =====

        public IslemSonucu Ekle(VeterinerZiyareti ziyaret)
        {
            return Calistir(() =>
            {
                Kontroller(ziyaret);
                _ziyaretRepo.Ekle(ziyaret);
            },
            "Veteriner ziyareti kaydedildi.");
        }

        public IslemSonucu Guncelle(VeterinerZiyareti ziyaret)
        {
            return Calistir(() =>
            {
                if (ziyaret.Id <= 0)
                    throw new DogrulamaException("Güncellenecek ziyaret seçilmedi.");

                Kontroller(ziyaret);
                _ziyaretRepo.Guncelle(ziyaret);
            },
            "Ziyaret bilgileri güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            // Repository'deki override edilmiş Sil, bağlı ilaçların bağlantısını kendisi boşaltıyor
            return Calistir(() => _ziyaretRepo.Sil(id), "Ziyaret kaydı silindi.");
        }

        // ===== YARDIMCI =====

        private void Kontroller(VeterinerZiyareti ziyaret)
        {
            if (ziyaret.HayvanId <= 0 || !_hayvanRepo.VarMi(h => h.Id == ziyaret.HayvanId))
                throw new DogrulamaException("Lütfen hayvanı seçin.");

            if (ziyaret.VeterinerId <= 0 || !_veterinerRepo.VarMi(v => v.Id == ziyaret.VeterinerId))
                throw new DogrulamaException("Lütfen veterineri seçin.");

            if (string.IsNullOrWhiteSpace(ziyaret.Sikayet))
                throw new DogrulamaException("Lütfen şikayeti / ziyaret nedenini yazın.");

            // 2.4'te "Business katmanında yapacağız" dediğimiz İKİ ALANLI kural:
            if (ziyaret.SonrakiKontrolTarihi.HasValue &&
                ziyaret.SonrakiKontrolTarihi.Value.Date <= ziyaret.ZiyaretTarihi.Date)
            {
                throw new DogrulamaException("Sonraki kontrol tarihi, ziyaret tarihinden sonra olmalıdır.");
            }
        }
    }
}