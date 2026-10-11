namespace PetAsistan.Entities
{
    // Bir hayvana yapılmış tek bir aşı.
    // ": BaseEntity, IHatirlatilabilir" →
    //    BaseEntity'den MİRAS alıyor, IHatirlatilabilir SÖZLEŞMESİNİ uyguluyor.
    public class AsiKaydi : BaseEntity, IHatirlatilabilir
    {
        private DateTime _uygulamaTarihi = DateTime.Today;

        // ===== İLİŞKİLER =====
        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        public int AsiTanimiId { get; set; }
        public AsiTanimi? AsiTanimi { get; set; }

        // Aşıyı yapan veteriner (zorunlu değil: int? → boş olabilir)
        public int? VeterinerId { get; set; }
        public Veteriner? Veteriner { get; set; }

        // ===== AŞI BİLGİLERİ =====

        public DateTime UygulamaTarihi
        {
            get { return _uygulamaTarihi; }
            set
            {
                if (value.Date > DateTime.Today)
                    throw new DogrulamaException("Aşı uygulama tarihi gelecekte olamaz.");

                _uygulamaTarihi = value.Date;
            }
        }

        public DateTime? SonrakiAsiTarihi { get; set; }
        public string? SeriNo { get; set; }    // Aşı şişesinin seri numarası
        public string? Notlar { get; set; }

        // Sonraki aşı tarihini, aşı türünün tekrar aralığına göre otomatik hesaplar.
        // Örn: 10 Mart'ta yapılan yıllık aşı → 10 Mart (gelecek yıl)
        public void SonrakiTarihiHesapla()
        {
            if (AsiTanimi != null)
                SonrakiAsiTarihi = UygulamaTarihi.AddDays(AsiTanimi.TekrarAraligiGun);
        }

        // ===== IHatirlatilabilir SÖZLEŞMESİNİN GEREKLERİ =====

        public DateTime? HatirlatmaTarihi => SonrakiAsiTarihi;

        public string HatirlatmaMetni()
        {
            return $"{Hayvan?.Ad} için {AsiTanimi?.Ad} aşısının zamanı geliyor.";
        }
    }
}