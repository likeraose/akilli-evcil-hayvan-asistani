using System.Linq.Expressions;
using PetAsistan.Entities;

namespace PetAsistan.DataAccess.Repositories
{
    // SÖZLEŞME: Her repository bu işlemleri yapabilmeli.
    // <T>          → "T" yerine sonradan bir tür gelecek (Sahip, Hayvan...)
    // where T : BaseEntity → T, mutlaka BaseEntity'den türemiş bir sınıf olmalı
    public interface IRepository<T> where T : BaseEntity
    {
        List<T> TumunuGetir();                              // Listele
        List<T> Bul(Expression<Func<T, bool>> kosul);       // Koşula göre filtrele
        T? IdIleGetir(int id);                              // Tek kayıt getir
        bool VarMi(Expression<Func<T, bool>> kosul);        // Böyle bir kayıt var mı?
        void Ekle(T kayit);                                 // Ekle
        void Guncelle(T kayit);                             // Güncelle
        void Sil(int id);                                   // Sil
    }
}