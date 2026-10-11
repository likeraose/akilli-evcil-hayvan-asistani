namespace PetAsistan.Entities
{
    // Tüm varlık sınıflarının ortak temeli.
    // abstract: Bu sınıftan doğrudan nesne üretilemez, sadece miras alınır.
    public abstract class BaseEntity
    {
        // Her kaydın benzersiz numarası (veritabanında otomatik artar)
        public int Id { get; set; }

        // Kayıt eklendiğinde o anki tarih otomatik atanır
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // ? işareti: Bu alan boş (null) olabilir.
        // Çünkü yeni eklenen bir kayıt henüz hiç güncellenmemiştir.
        public DateTime? GuncellemeTarihi { get; set; }
    }
}