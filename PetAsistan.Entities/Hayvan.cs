namespace PetAsistan.Entities
{
    // Tüm evcil hayvanların ortak temeli.
    // abstract: "Hayvan" diye bir nesne üretilemez; ya Kedi, ya Kopek, ya Kus üretilir.
    public abstract class Hayvan : BaseEntity
    {
        // ===== KASA: private alanlar =====
        private string _ad = string.Empty;
        private DateTime _dogumTarihi = DateTime.Today;
        private decimal _kilo;

        // ===== ORTAK ÖZELLİKLER (kapsüllenmiş) =====

        public string Ad
        {
            get { return _ad; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DogrulamaException("Hayvanın adı boş bırakılamaz.");

                if (value.Trim().Length > 50)
                    throw new DogrulamaException("Hayvanın adı en fazla 50 karakter olabilir.");

                _ad = value.Trim();
            }
        }

        public DateTime DogumTarihi
        {
            get { return _dogumTarihi; }
            set
            {
                if (value.Date > DateTime.Today)
                    throw new DogrulamaException("Doğum tarihi gelecekte bir tarih olamaz.");

                if (value.Date < DateTime.Today.AddYears(-50))
                    throw new DogrulamaException("Doğum tarihi 50 yıldan daha eski olamaz.");

                _dogumTarihi = value.Date;
            }
        }

        public decimal Kilo
        {
            get { return _kilo; }
            set
            {
                if (value <= 0)
                    throw new DogrulamaException("Kilo sıfırdan büyük olmalıdır.");

                // MaksimumKilo her tür için farklı! (aşağıda abstract olarak tanımlı)
                if (value > MaksimumKilo)
                    throw new DogrulamaException($"{TurAdi} için en fazla {MaksimumKilo} kg girilebilir.");

                _kilo = value;
            }
        }

        // Kuralı olmayan özellikler kısa yazımla
        public string? Cins { get; set; }          // Örn: "Tekir", "Golden Retriever"
        public Cinsiyet Cinsiyet { get; set; }     // Enum: Disi / Erkek
        public bool KisirMi { get; set; }          // Kısırlaştırılmış mı? (kalori hesabını etkiler)
        public byte[]? Fotograf { get; set; }      // Resim dosyası veritabanında byte dizisi olarak tutulur
        public string? Notlar { get; set; }

        // ===== SAHİP İLİŞKİSİ =====
        // Her hayvanın bir sahibi var. SahipId, veritabanında Sahipler tablosuna bağlanır.
        public int SahipId { get; set; }
        public Sahip? Sahip { get; set; }

        // Bu hayvanın veteriner ziyaretleri (1 hayvan → N ziyaret)
        public List<VeterinerZiyareti> Ziyaretler { get; set; } = new List<VeterinerZiyareti>();

        // Bu hayvana yapılan aşılar (1 hayvan → N aşı kaydı)
        public List<AsiKaydi> AsiKayitlari { get; set; } = new List<AsiKaydi>();

        // ===== HESAPLANAN ÖZELLİK (veritabanına kaydedilmez) =====
        // Yaşı doğum tarihinden otomatik hesaplar. Örn: 2.5 yaş
        public double Yas => Math.Round((DateTime.Today - DogumTarihi).TotalDays / 365.25, 1);

        // ===== ABSTRACT ÜYELER: Her tür KENDİSİ doldurmak ZORUNDA =====

        public abstract string TurAdi { get; }          // "Kedi", "Köpek", "Kuş"
        public abstract decimal MaksimumKilo { get; }   // Türe göre makul en yüksek kilo
        public abstract double InsanYasinaCevir();      // Türe göre farklı formül
        public abstract int GunlukKaloriIhtiyaci();     // Türe göre farklı formül

        // ===== VIRTUAL ÜYE: Varsayılan cevap var, tür isterse değiştirir =====
        public virtual string BakimTavsiyesi()
        {
            return "Düzenli veteriner kontrollerini ve aşı takvimini aksatmayın.";
        }

        // Listelerde "Pamuk (Kedi)" şeklinde görünsün
        public override string ToString() => $"{Ad} ({TurAdi})";
    }
}