using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    public interface IAsiKaydiRepository : IRepository<AsiKaydi>
    {
        List<AsiKaydi> HayvanaGoreGetir(int hayvanId);
    }

    public class AsiKaydiRepository : GenericRepository<AsiKaydi>, IAsiKaydiRepository
    {
        // OVERRIDE: Aşı kayıtlarını hayvan, aşı türü ve veteriner bilgisiyle getir
        public override List<AsiKaydi> TumunuGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.AsiKayitlari
                .AsNoTracking()
                .Include(a => a.Hayvan)
                .Include(a => a.AsiTanimi)
                .Include(a => a.Veteriner)
                .OrderByDescending(a => a.UygulamaTarihi)
                .ToList();
        }

        public List<AsiKaydi> HayvanaGoreGetir(int hayvanId)
        {
            using var db = new PetAsistanDbContext();
            return db.AsiKayitlari
                .AsNoTracking()
                .Include(a => a.AsiTanimi)
                .Include(a => a.Veteriner)
                .Where(a => a.HayvanId == hayvanId)
                .OrderByDescending(a => a.UygulamaTarihi)
                .ToList();
        }
    }
}