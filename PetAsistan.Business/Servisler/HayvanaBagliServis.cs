using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Hayvana bağlı kayıtlarla (ilaç, kilo, hatırlatıcı) çalışan servislerin ORTAK temeli.
    // BaseServis'ten miras alır → Calistir() ve Getir() buraya da geliyor.
    public abstract class HayvanaBagliServis : BaseServis
    {
        // protected: Sadece bu sınıf ve ondan TÜREYEN servisler erişebilir
        protected IHayvanRepository HayvanRepo { get; }

        // protected constructor: Soyut sınıf olduğu için sadece alt sınıflar çağırabilir
        protected HayvanaBagliServis(IHayvanRepository hayvanRepo)
        {
            HayvanRepo = hayvanRepo;
        }

        // Ortak yardımcı: Hayvanı bulur, yoksa anlaşılır bir hata fırlatır
        protected Hayvan HayvaniBul(int hayvanId)
        {
            Hayvan? hayvan = hayvanId > 0 ? HayvanRepo.IdIleGetir(hayvanId) : null;

            if (hayvan == null)
                throw new DogrulamaException("Lütfen hayvanı seçin.");

            return hayvan;
        }
    }
}