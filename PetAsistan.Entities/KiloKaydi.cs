namespace PetAsistan.Entities
{
    // Belirli bir tarihteki kilo ölçümü (kilo geçmişi / grafik için)
    public class KiloKaydi : BaseEntity
    {
        private DateTime _olcumTarihi = DateTime.Today;
        private decimal _kilo;

        public int HayvanId { get; set; }
        public Hayvan? Hayvan { get; set; }

        public DateTime OlcumTarihi
        {
            get { return _olcumTarihi; }
            set
            {
                if (value.Date > DateTime.Today)
                    throw new DogrulamaException("Ölçüm tarihi gelecekte olamaz.");

                _olcumTarihi = value.Date;
            }
        }

        public decimal Kilo
        {
            get { return _kilo; }
            set
            {
                if (value <= 0)
                    throw new DogrulamaException("Kilo sıfırdan büyük olmalıdır.");

                // Türe özel üst sınır (kedi en fazla 15 kg vb.) Business katmanında kontrol edilecek
                _kilo = value;
            }
        }

        public string? Notlar { get; set; }
    }
}