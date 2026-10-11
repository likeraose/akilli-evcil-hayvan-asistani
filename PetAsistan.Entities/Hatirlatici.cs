namespace PetAsistan.Entities
{
    // Kullanıcının kendi eklediği hatırlatıcı. Örn: "Tırnak kesimi", "Pati Klinik randevusu"
    public class Hatirlatici : BaseEntity, IHatirlatilabilir
    {
        private string _baslik = string.Empty;

        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        public string Baslik
        {
            get { return _baslik; }
            set { _baslik = DogrulamaYardimcisi.ZorunluMetin(value, "Hatırlatıcı başlığı", 100); }
        }

        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Today.AddDays(1);
        public HatirlaticiTuru Tur { get; set; } = HatirlaticiTuru.Diger;
        public bool TamamlandiMi { get; set; }

        // ===== IHatirlatilabilir SÖZLEŞMESİ =====

        // Tamamlanan hatırlatıcılar artık gösterilmesin
        public DateTime? HatirlatmaTarihi => TamamlandiMi ? null : Tarih;

        public string HatirlatmaMetni()
        {
            return $"{Hayvan?.Ad}: {Baslik}";
        }
    }
}