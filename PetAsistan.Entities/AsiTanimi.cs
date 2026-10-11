namespace PetAsistan.Entities
{
    // Aşı TÜRÜ. Örn: "Kuduz", "Karma", "İç parazit"
    public class AsiTanimi : BaseEntity
    {
        private string _ad = string.Empty;
        private int _tekrarAraligiGun = 365;

        public string Ad
        {
            get { return _ad; }
            set { _ad = DogrulamaYardimcisi.ZorunluMetin(value, "Aşı adı", 100); }
        }

        // Bu aşı hangi türe uygulanır?
        public AsiHedefTuru HedefTur { get; set; } = AsiHedefTuru.Hepsi;

        // Kaç günde bir tekrarlanır? Örn: 365 = yılda bir
        public int TekrarAraligiGun
        {
            get { return _tekrarAraligiGun; }
            set
            {
                if (value <= 0 || value > 3650)
                    throw new DogrulamaException("Tekrar aralığı 1 ile 3650 gün arasında olmalıdır.");

                _tekrarAraligiGun = value;
            }
        }

        public string? Aciklama { get; set; }

        // Bu aşının yapıldığı tüm kayıtlar
        public List<AsiKaydi> AsiKayitlari { get; set; } = new List<AsiKaydi>();

        public override string ToString() => Ad;
    }
}