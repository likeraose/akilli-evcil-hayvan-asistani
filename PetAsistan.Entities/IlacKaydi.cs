namespace PetAsistan.Entities
{
    // Hayvanın kullandığı bir ilaç / tedavi
    public class IlacKaydi : BaseEntity, IHatirlatilabilir
    {
        private string _ilacAdi = string.Empty;
        private string _doz = string.Empty;
        private int _gunlukKullanimSayisi = 1;

        // ===== İLİŞKİLER =====
        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        // İlaç bir veteriner ziyaretinde yazıldıysa o ziyarete bağlanır (zorunlu değil)
        public int? VeterinerZiyaretiId { get; set; }
        public VeterinerZiyareti? VeterinerZiyareti { get; set; }

        // ===== İLAÇ BİLGİLERİ =====

        public string IlacAdi
        {
            get { return _ilacAdi; }
            set { _ilacAdi = DogrulamaYardimcisi.ZorunluMetin(value, "İlaç adı", 100); }
        }

        // Örn: "1 tablet", "5 ml", "yarım tablet"
        public string Doz
        {
            get { return _doz; }
            set { _doz = DogrulamaYardimcisi.ZorunluMetin(value, "Doz", 100); }
        }

        public int GunlukKullanimSayisi
        {
            get { return _gunlukKullanimSayisi; }
            set
            {
                if (value < 1 || value > 12)
                    throw new DogrulamaException("Günlük kullanım sayısı 1 ile 12 arasında olmalıdır.");

                _gunlukKullanimSayisi = value;
            }
        }

        public DateTime BaslangicTarihi { get; set; } = DateTime.Today;
        public DateTime? BitisTarihi { get; set; }     // Boşsa: süresiz kullanım
        public string? Notlar { get; set; }

        // Hesaplanan özellik: Tedavi hâlâ devam ediyor mu?
        public bool DevamEdiyorMu => BitisTarihi == null || BitisTarihi.Value.Date >= DateTime.Today;

        // ===== IHatirlatilabilir SÖZLEŞMESİ =====

        // Devam eden bir tedavinin bitiş tarihini hatırlat, bitmiş tedaviler için hatırlatma yok
        public DateTime? HatirlatmaTarihi => DevamEdiyorMu ? BitisTarihi : null;

        public string HatirlatmaMetni()
        {
            return $"{Hayvan?.Ad} için {IlacAdi} tedavisi {BitisTarihi:dd.MM.yyyy} tarihinde bitiyor.";
        }
    }
}