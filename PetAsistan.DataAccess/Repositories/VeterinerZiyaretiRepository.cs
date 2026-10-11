using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    public interface IVeterinerZiyaretiRepository : IRepository<VeterinerZiyareti>
    {
        List<VeterinerZiyareti> HayvanaGoreGetir(int hayvanId);
    }

    public class VeterinerZiyaretiRepository : GenericRepository<VeterinerZiyareti>, IVeterinerZiyaretiRepository
    {
        // OVERRIDE: Ziyaretleri hayvan ve veteriner bilgisiyle, en yeniden eskiye getir
        public override List<VeterinerZiyareti> TumunuGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.VeterinerZiyaretleri
                .AsNoTracking()
                .Include(z => z.Hayvan)
                .Include(z => z.Veteriner)
                .OrderByDescending(z => z.ZiyaretTarihi)
                .ToList();
        }

        public List<VeterinerZiyareti> HayvanaGoreGetir(int hayvanId)
        {
            using var db = new PetAsistanDbContext();
            return db.VeterinerZiyaretleri
                .AsNoTracking()
                .Include(z => z.Veteriner)
                .Where(z => z.HayvanId == hayvanId)
                .OrderByDescending(z => z.ZiyaretTarihi)
                .ToList();
        }

        // OVERRIDE: Silmeden önce bu ziyarete bağlı ilaçların bağlantısını boşalt
        public override void Sil(int id)
        {
            using var db = new PetAsistanDbContext();

            VeterinerZiyareti? ziyaret = db.VeterinerZiyaretleri.Find(id);

            if (ziyaret == null)
                throw new InvalidOperationException("Silinmek istenen kayıt bulunamadı.");

            // Bu ziyarette yazılmış ilaçlar silinmesin, sadece ziyaret bağlantısı boşalsın
            List<IlacKaydi> bagliIlaclar = db.IlacKayitlari
                .Where(i => i.VeterinerZiyaretiId == id)
                .ToList();

            foreach (IlacKaydi ilac in bagliIlaclar)
            {
                ilac.VeterinerZiyaretiId = null;
            }

            db.VeterinerZiyaretleri.Remove(ziyaret);
            db.SaveChanges();   // İkisi birlikte, tek seferde kaydedilir
        }
    }
}