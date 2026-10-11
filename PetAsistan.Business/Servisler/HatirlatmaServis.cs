using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Aşı, ziyaret, ilaç ve hatırlatıcıları TEK LİSTEDE toplayan servis.
    // IHatirlatilabilir interface'i sayesinde hepsine AYNI şekilde davranıyoruz.
    public class HatirlatmaServis : BaseServis
    {
        private readonly IHayvanRepository _hayvanRepo;

        public HatirlatmaServis() : this(new HayvanRepository())
        {
        }

        public HatirlatmaServis(IHayvanRepository hayvanRepo)
        {
            _hayvanRepo = hayvanRepo;
        }

        // Ana ekran için: TÜM hayvanların, önümüzdeki X gün içindeki (ve gecikmiş) hatırlatmaları
        public IslemSonucu<List<HatirlatmaOgesi>> YaklasanlariGetir(int gunSayisi = 30)
        {
            return Getir(() =>
            {
                DateTime sinirTarih = DateTime.Today.AddDays(gunSayisi);
                List<HatirlatmaOgesi> sonuc = new List<HatirlatmaOgesi>();

                foreach (Hayvan hayvan in _hayvanRepo.HatirlatmalarlaBirlikteGetir())
                {
                    // Aşı mı, ilaç mı, ziyaret mi... UMURSAMIYORUZ. Hepsi IHatirlatilabilir!
                    foreach (IHatirlatilabilir hatirlatma in hayvan.TumHatirlatmalar())
                    {
                        if (hatirlatma.HatirlatmaTarihi!.Value.Date <= sinirTarih)
                            sonuc.Add(OgeyeCevir(hayvan, hatirlatma));
                    }
                }

                return sonuc.OrderBy(o => o.Tarih).ToList();
            });
        }

        // Hayvan detay ekranı için: TEK bir hayvanın tüm güncel hatırlatmaları
        public IslemSonucu<List<HatirlatmaOgesi>> HayvanIcinGetir(int hayvanId)
        {
            return Getir(() =>
            {
                Hayvan? hayvan = _hayvanRepo.TumBilgileriyleGetir(hayvanId);

                if (hayvan == null)
                    throw new InvalidOperationException("Hayvan bulunamadı.");

                return hayvan.TumHatirlatmalar()
                             .Select(h => OgeyeCevir(hayvan, h))
                             .ToList();
            });
        }

        // ===== YARDIMCI =====

        // Bir IHatirlatilabilir nesnesini ekranda gösterilecek pakete çevirir
        private static HatirlatmaOgesi OgeyeCevir(Hayvan hayvan, IHatirlatilabilir hatirlatma)
        {
            return new HatirlatmaOgesi
            {
                Tarih = hatirlatma.HatirlatmaTarihi!.Value.Date,
                HayvanAdi = hayvan.Ad,
                Tur = KaynakAdi(hatirlatma),
                Mesaj = hatirlatma.HatirlatmaMetni()   // ← Her sınıf KENDİ metnini üretiyor (çok biçimlilik)
            };
        }

        // Nesnenin gerçek türüne göre kaynak adı
        private static string KaynakAdi(IHatirlatilabilir hatirlatma)
        {
            return hatirlatma switch
            {
                AsiKaydi => "Aşı",
                VeterinerZiyareti => "Veteriner",
                IlacKaydi => "İlaç",
                Hatirlatici => "Hatırlatıcı",
                _ => "Diğer"
            };
        }
    }
}