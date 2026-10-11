using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    // SÖZLEŞME: IRepository<Hayvan>'daki her şey + hayvanlara özgü ek metotlar.
    // (Interface de başka bir interface'ten miras alabilir!)
    public interface IHayvanRepository : IRepository<Hayvan>
    {
        List<Hayvan> SahibeGoreGetir(int sahipId);
        Hayvan? TumBilgileriyleGetir(int id);
        List<Hayvan> HatirlatmalarlaBirlikteGetir();
    }

    // GenericRepository<Hayvan>'dan MİRAS alır → Ekle, Guncelle, Sil hazır geliyor.
    // IHayvanRepository sözleşmesini UYGULAR.
    public class HayvanRepository : GenericRepository<Hayvan>, IHayvanRepository
    {
        // OVERRIDE: Hayvanları SAHİPLERİYLE birlikte getir
        public override List<Hayvan> TumunuGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.Hayvanlar
                .AsNoTracking()
                .Include(h => h.Sahip)
                .OrderBy(h => h.Ad)
                .ToList();
        }

        // Belirli bir sahibin hayvanları
        public List<Hayvan> SahibeGoreGetir(int sahipId)
        {
            using var db = new PetAsistanDbContext();
            return db.Hayvanlar
                .AsNoTracking()
                .Include(h => h.Sahip)
                .Where(h => h.SahipId == sahipId)
                .OrderBy(h => h.Ad)
                .ToList();
        }

        // Bir hayvanı TÜM sağlık geçmişiyle birlikte getir
        // (Hayvan detay ekranı ve yapay zeka asistanı için)
        public Hayvan? TumBilgileriyleGetir(int id)
        {
            using var db = new PetAsistanDbContext();
            return db.Hayvanlar
                .AsNoTrackingWithIdentityResolution()
                .Include(h => h.Sahip)
                .Include(h => h.Ziyaretler).ThenInclude(z => z.Veteriner)
                .Include(h => h.AsiKayitlari).ThenInclude(a => a.AsiTanimi)
                .Include(h => h.IlacKayitlari)
                .Include(h => h.KiloKayitlari)
                .Include(h => h.Hatirlaticilar)
                .AsSplitQuery()
                .FirstOrDefault(h => h.Id == id);
        }

        // Tüm hayvanları, hatırlatma üreten kayıtlarıyla birlikte getir
        // (Ana ekrandaki "Yaklaşan hatırlatmalar" listesi için)
        public List<Hayvan> HatirlatmalarlaBirlikteGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.Hayvanlar
                .AsNoTrackingWithIdentityResolution()
                .Include(h => h.Sahip)
                .Include(h => h.Ziyaretler).ThenInclude(z => z.Veteriner)
                .Include(h => h.AsiKayitlari).ThenInclude(a => a.AsiTanimi)
                .Include(h => h.IlacKayitlari)
                .Include(h => h.Hatirlaticilar)
                .AsSplitQuery()
                .ToList();
        }
    }
}