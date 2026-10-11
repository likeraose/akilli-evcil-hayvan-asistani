using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    // IRepository<T> SÖZLEŞMESİNİ uygulayan, her tabloda çalışan genel repository.
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        // ===== OKUMA İŞLEMLERİ =====
        // virtual: Özel repository'ler (4.2) bu metotları kendilerine göre değiştirebilsin

        public virtual List<T> TumunuGetir()
        {
            using var db = new PetAsistanDbContext();
            return db.Set<T>().AsNoTracking().ToList();
        }

        public virtual List<T> Bul(Expression<Func<T, bool>> kosul)
        {
            using var db = new PetAsistanDbContext();
            return db.Set<T>().AsNoTracking().Where(kosul).ToList();
        }

        public virtual T? IdIleGetir(int id)
        {
            using var db = new PetAsistanDbContext();
            return db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public bool VarMi(Expression<Func<T, bool>> kosul)
        {
            using var db = new PetAsistanDbContext();
            return db.Set<T>().Any(kosul);
        }

        // ===== YAZMA İŞLEMLERİ =====

        public void Ekle(T kayit)
        {
            using var db = new PetAsistanDbContext();

            kayit.OlusturmaTarihi = DateTime.Now;

            // Sadece BU kaydı "eklenecek" olarak işaretle.
            // (Kayda bağlı nesneler, örn. hayvanın Sahip nesnesi, tekrar eklenmesin)
            db.Entry(kayit).State = EntityState.Added;
            db.SaveChanges();
        }

        public void Guncelle(T kayit)
        {
            using var db = new PetAsistanDbContext();

            kayit.GuncellemeTarihi = DateTime.Now;

            // Sadece BU kaydı "değiştirildi" olarak işaretle
            db.Entry(kayit).State = EntityState.Modified;
            db.SaveChanges();
        }

        public virtual void Sil(int id)
        {
            using var db = new PetAsistanDbContext();

            T? kayit = db.Set<T>().Find(id);

            if (kayit == null)
                throw new InvalidOperationException("Silinmek istenen kayıt bulunamadı.");

            db.Set<T>().Remove(kayit);
            db.SaveChanges();
        }
    }
}