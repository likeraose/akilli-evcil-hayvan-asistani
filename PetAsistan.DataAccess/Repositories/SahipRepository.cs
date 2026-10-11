using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    public interface ISahipRepository : IRepository<Sahip>
    {
        bool TelefonKayitliMi(string telefon, int haricTutulacakId = 0);
    }

    public class SahipRepository : GenericRepository<Sahip>, ISahipRepository
    {
        // OVERRIDE: Sahipleri HAYVANLARIYLA birlikte getir
        public override List<Sahip> TumunuGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.Sahipler
                .AsNoTracking()
                .Include(s => s.Hayvanlar)
                .OrderBy(s => s.Ad)
                .ThenBy(s => s.Soyad)
                .ToList();
        }

        // Bu telefon numarası başka bir sahipte kayıtlı mı?
        // haricTutulacakId: Güncelleme yaparken sahibin KENDİ kaydını saymamak için.
        // (= 0 → parametre verilmezse varsayılan değer 0 olur)
        public bool TelefonKayitliMi(string telefon, int haricTutulacakId = 0)
        {
            return VarMi(s => s.Telefon == telefon && s.Id != haricTutulacakId);
        }
    }
}