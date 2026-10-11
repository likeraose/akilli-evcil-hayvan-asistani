using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Aşı türleri ve aşı kayıtlarıyla ilgili tüm iş kuralları
    public class AsiServis : BaseServis
    {
        private readonly IAsiKaydiRepository _asiKaydiRepo;
        private readonly IRepository<AsiTanimi> _asiTanimiRepo;
        private readonly IHayvanRepository _hayvanRepo;
        private readonly IRepository<Veteriner> _veterinerRepo;

        public AsiServis()
            : this(new AsiKaydiRepository(),
                   new GenericRepository<AsiTanimi>(),
                   new HayvanRepository(),
                   new GenericRepository<Veteriner>())
        {
        }

        public AsiServis(IAsiKaydiRepository asiKaydiRepo,
                         IRepository<AsiTanimi> asiTanimiRepo,
                         IHayvanRepository hayvanRepo,
                         IRepository<Veteriner> veterinerRepo)
        {
            _asiKaydiRepo = asiKaydiRepo;
            _asiTanimiRepo = asiTanimiRepo;
            _hayvanRepo = hayvanRepo;
            _veterinerRepo = veterinerRepo;
        }

        // =====================================================
        //  AŞI TÜRLERİ (AsiTanimi)
        // =====================================================

        public IslemSonucu<List<AsiTanimi>> AsiTanimlariniGetir()
        {
            return Getir(() => _asiTanimiRepo.TumunuGetir()
                                             .OrderBy(a => a.Ad)
                                             .ToList());
        }

        // Sadece seçilen hayvanın türüne uygun aşıları getirir
        // (Arayüzde kediye köpek aşısı seçilemesin diye)
        public IslemSonucu<List<AsiTanimi>> HayvanaUygunAsiTanimlari(int hayvanId)
        {
            return Getir(() =>
            {
                Hayvan hayvan = HayvaniBul(hayvanId);
                AsiHedefTuru hedef = HedefTureCevir(hayvan);

                return _asiTanimiRepo
                    .Bul(a => a.HedefTur == AsiHedefTuru.Hepsi || a.HedefTur == hedef)
                    .OrderBy(a => a.Ad)
                    .ToList();
            });
        }

        public IslemSonucu AsiTanimiEkle(AsiTanimi tanim)
        {
            return Calistir(() =>
            {
                if (string.IsNullOrWhiteSpace(tanim.Ad))
                    throw new DogrulamaException("Aşı adı boş bırakılamaz.");

                if (_asiTanimiRepo.VarMi(a => a.Ad == tanim.Ad))
                    throw new DogrulamaException("Bu isimde bir aşı türü zaten var.");

                _asiTanimiRepo.Ekle(tanim);
            },
            "Aşı türü eklendi.");
        }

        public IslemSonucu AsiTanimiGuncelle(AsiTanimi tanim)
        {
            return Calistir(() =>
            {
                if (tanim.Id <= 0)
                    throw new DogrulamaException("Güncellenecek aşı türü seçilmedi.");

                if (string.IsNullOrWhiteSpace(tanim.Ad))
                    throw new DogrulamaException("Aşı adı boş bırakılamaz.");

                if (_asiTanimiRepo.VarMi(a => a.Ad == tanim.Ad && a.Id != tanim.Id))
                    throw new DogrulamaException("Bu isimde başka bir aşı türü zaten var.");

                _asiTanimiRepo.Guncelle(tanim);
            },
            "Aşı türü güncellendi.");
        }

        public IslemSonucu AsiTanimiSil(int id)
        {
            return Calistir(() =>
            {
                int kullanimSayisi = _asiKaydiRepo.Bul(k => k.AsiTanimiId == id).Count;

                if (kullanimSayisi > 0)
                    throw new DogrulamaException(
                        $"Bu aşı türü {kullanimSayisi} aşı kaydında kullanılmış, silinemez.");

                _asiTanimiRepo.Sil(id);
            },
            "Aşı türü silindi.");
        }

        // =====================================================
        //  AŞI KAYITLARI (AsiKaydi)
        // =====================================================

        public IslemSonucu<List<AsiKaydi>> TumunuGetir()
        {
            return Getir(() => _asiKaydiRepo.TumunuGetir());
        }

        public IslemSonucu<List<AsiKaydi>> HayvanaGoreGetir(int hayvanId)
        {
            return Getir(() => _asiKaydiRepo.HayvanaGoreGetir(hayvanId));
        }

        public IslemSonucu Ekle(AsiKaydi kayit)
        {
            return Calistir(() =>
            {
                AsiTanimi tanim = Kontroller(kayit);

                // İŞ KURALI: Sonraki aşı tarihi girilmediyse otomatik hesapla
                if (kayit.SonrakiAsiTarihi == null)
                {
                    kayit.AsiTanimi = tanim;
                    kayit.SonrakiTarihiHesapla();   // 2.5-A'da AsiKaydi'na yazdığımız metot
                }

                _asiKaydiRepo.Ekle(kayit);
            },
            "Aşı kaydedildi.");
        }

        public IslemSonucu Guncelle(AsiKaydi kayit)
        {
            return Calistir(() =>
            {
                if (kayit.Id <= 0)
                    throw new DogrulamaException("Güncellenecek aşı kaydı seçilmedi.");

                AsiTanimi tanim = Kontroller(kayit);

                // Sonraki tarih boş bırakıldıysa yeniden hesapla
                if (kayit.SonrakiAsiTarihi == null)
                {
                    kayit.AsiTanimi = tanim;
                    kayit.SonrakiTarihiHesapla();
                }

                _asiKaydiRepo.Guncelle(kayit);
            },
            "Aşı kaydı güncellendi.");
        }

        public IslemSonucu Sil(int id)
        {
            return Calistir(() => _asiKaydiRepo.Sil(id), "Aşı kaydı silindi.");
        }

        // =====================================================
        //  YARDIMCI METOTLAR
        // =====================================================

        // Tüm kuralları kontrol eder ve seçilen aşı türünü döndürür
        private AsiTanimi Kontroller(AsiKaydi kayit)
        {
            Hayvan hayvan = HayvaniBul(kayit.HayvanId);

            AsiTanimi? tanim = kayit.AsiTanimiId > 0
                ? _asiTanimiRepo.IdIleGetir(kayit.AsiTanimiId)
                : null;

            if (tanim == null)
                throw new DogrulamaException("Lütfen aşı türünü seçin.");

            // İŞ KURALI: Aşı, hayvanın türüne uygun olmalı
            AsiHedefTuru hayvaninTuru = HedefTureCevir(hayvan);

            if (tanim.HedefTur != AsiHedefTuru.Hepsi && tanim.HedefTur != hayvaninTuru)
                throw new DogrulamaException($"{tanim.Ad}, {hayvan.Ad} ({hayvan.TurAdi}) için uygun değil.");

            // İŞ KURALI: Aşı, doğumdan önce yapılmış olamaz
            if (kayit.UygulamaTarihi.Date < hayvan.DogumTarihi.Date)
                throw new DogrulamaException("Aşı tarihi, hayvanın doğum tarihinden önce olamaz.");

            // Veteriner seçildiyse gerçekten var mı?
            if (kayit.VeterinerId.HasValue)
            {
                int veterinerId = kayit.VeterinerId.Value;

                if (!_veterinerRepo.VarMi(v => v.Id == veterinerId))
                    throw new DogrulamaException("Seçilen veteriner bulunamadı.");
            }

            // Sonraki tarih elle girildiyse mantıklı mı?
            if (kayit.SonrakiAsiTarihi.HasValue &&
                kayit.SonrakiAsiTarihi.Value.Date <= kayit.UygulamaTarihi.Date)
            {
                throw new DogrulamaException("Sonraki aşı tarihi, uygulama tarihinden sonra olmalıdır.");
            }

            return tanim;
        }

        private Hayvan HayvaniBul(int hayvanId)
        {
            Hayvan? hayvan = hayvanId > 0 ? _hayvanRepo.IdIleGetir(hayvanId) : null;

            if (hayvan == null)
                throw new DogrulamaException("Lütfen hayvanı seçin.");

            return hayvan;
        }

        // Nesnenin GERÇEK türüne bakarak (Kedi mi, Kopek mi, Kus mu?) aşı hedef türünü bulur
        private static AsiHedefTuru HedefTureCevir(Hayvan hayvan)
        {
            return hayvan switch
            {
                Kedi => AsiHedefTuru.Kedi,
                Kopek => AsiHedefTuru.Kopek,
                Kus => AsiHedefTuru.Kus,
                _ => AsiHedefTuru.Hepsi
            };
        }
    }
}