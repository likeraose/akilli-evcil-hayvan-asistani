namespace PetAsistan.Entities
{
    // Bir hayvanın bir veterinere yaptığı tek bir ziyaret
    public class VeterinerZiyareti : BaseEntity
    {
        private DateTime _ziyaretTarihi = DateTime.Today;
        private string _sikayet = string.Empty;
        private decimal _ucret;

        // ===== İLİŞKİLER =====
        // Hangi hayvan?
        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        // Hangi veteriner?
        public int VeterinerId { get; set; }
        public Veteriner? Veteriner { get; set; }

        // ===== ZİYARET BİLGİLERİ =====

        public DateTime ZiyaretTarihi
        {
            get { return _ziyaretTarihi; }
            set
            {
                if (value.Date > DateTime.Today)
                    throw new DogrulamaException(
                        "Ziyaret tarihi gelecekte olamaz. İleri tarihli randevular için hatırlatıcı ekleyin.");

                _ziyaretTarihi = value;
            }
        }

        // Ziyaret nedeni (zorunlu). Örn: "İştahsızlık ve halsizlik"
        public string Sikayet
        {
            get { return _sikayet; }
            set { _sikayet = DogrulamaYardimcisi.ZorunluMetin(value, "Şikayet / ziyaret nedeni", 500); }
        }

        public string? Teshis { get; set; }    // Veterinerin koyduğu teşhis
        public string? Tedavi { get; set; }    // Uygulanan tedavi

        public decimal Ucret
        {
            get { return _ucret; }
            set
            {
                if (value < 0)
                    throw new DogrulamaException("Ücret negatif olamaz.");

                _ucret = value;
            }
        }

        // Veteriner tekrar kontrol istediyse
        public DateTime? SonrakiKontrolTarihi { get; set; }
    }
}