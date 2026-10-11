namespace PetAsistan.Entities
{
    public class Veteriner : BaseEntity
    {
        private string _veterinerAdi = string.Empty;
        private string _klinikAdi = string.Empty;
        private string _telefon = string.Empty;

        // Kontroller artık tek satır: işi yardımcı sınıf yapıyor
        public string VeterinerAdi
        {
            get { return _veterinerAdi; }
            set { _veterinerAdi = DogrulamaYardimcisi.ZorunluMetin(value, "Veteriner adı", 100); }
        }

        public string KlinikAdi
        {
            get { return _klinikAdi; }
            set { _klinikAdi = DogrulamaYardimcisi.ZorunluMetin(value, "Klinik adı", 100); }
        }

        public string Telefon
        {
            get { return _telefon; }
            set { _telefon = DogrulamaYardimcisi.Telefon(value); }
        }

        public string? Adres { get; set; }

        // Bu veterinerin kabul ettiği ziyaretler (1 veteriner → N ziyaret)
        public List<VeterinerZiyareti> Ziyaretler { get; set; } = new List<VeterinerZiyareti>();

        // Listelerde "Dr. Ayşe Kaya - Pati Klinik" şeklinde görünsün
        public override string ToString() => $"{VeterinerAdi} - {KlinikAdi}";
    }
}