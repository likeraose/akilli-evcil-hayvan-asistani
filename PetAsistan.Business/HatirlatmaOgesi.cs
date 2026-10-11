namespace PetAsistan.Business
{
    // DTO: Sadece ekranda göstermek için hazırlanmış bir "paket".
    // Veritabanında tablosu YOK; aşı, ziyaret, ilaç ve hatırlatıcılardan derleniyor.
    public class HatirlatmaOgesi
    {
        public DateTime Tarih { get; set; }
        public string HayvanAdi { get; set; } = string.Empty;
        public string Tur { get; set; } = string.Empty;      // "Aşı", "Veteriner", "İlaç", "Hatırlatıcı"
        public string Mesaj { get; set; } = string.Empty;

        // Hesaplanan özellikler
        public int KalanGun => (Tarih.Date - DateTime.Today).Days;

        public string Durum
        {
            get
            {
                if (KalanGun < 0) return "Gecikmiş";
                if (KalanGun == 0) return "Bugün";
                if (KalanGun <= 7) return "Bu hafta";
                return "Yaklaşıyor";
            }
        }
    }
}