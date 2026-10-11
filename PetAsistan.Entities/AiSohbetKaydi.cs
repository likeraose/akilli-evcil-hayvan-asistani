namespace PetAsistan.Entities
{
    // Yapay zekaya sorulan bir soru ve aldığı cevap
    public class AiSohbetKaydi : BaseEntity
    {
        private string _soru = string.Empty;

        // Soru belirli bir hayvanla ilgili olmayabilir (genel soru) → int?
        public int? HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        public AiIslemTuru IslemTuru { get; set; } = AiIslemTuru.Sohbet;

        public string Soru
        {
            get { return _soru; }
            set { _soru = DogrulamaYardimcisi.ZorunluMetin(value, "Soru", 2000); }
        }

        // Yapay zekanın cevabı (uzun olabilir, kural yok)
        public string Cevap { get; set; } = string.Empty;

        // Sorunun ne zaman sorulduğu → BaseEntity'deki OlusturmaTarihi zaten bunu tutuyor!
    }
}