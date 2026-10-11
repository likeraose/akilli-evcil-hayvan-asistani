namespace PetAsistan.Entities
{
    // Evcil hayvan sahibi.
    // ": BaseEntity" → Id, OlusturmaTarihi, GuncellemeTarihi otomatik geliyor.
    public class Sahip : BaseEntity
    {
        // ===== KASA: private alanlar (dışarıdan erişilemez) =====
        private string _ad = string.Empty;
        private string _soyad = string.Empty;
        private string _telefon = string.Empty;
        private string? _eposta;

        // ===== MAKİNE: public property'ler (kontrol ederek kabul eder) =====

        public string Ad
        {
            get { return _ad; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DogrulamaException("Ad alanı boş bırakılamaz.");

                if (value.Trim().Length > 50)
                    throw new DogrulamaException("Ad en fazla 50 karakter olabilir.");

                _ad = value.Trim();
            }
        }

        public string Soyad
        {
            get { return _soyad; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DogrulamaException("Soyad alanı boş bırakılamaz.");

                if (value.Trim().Length > 50)
                    throw new DogrulamaException("Soyad en fazla 50 karakter olabilir.");

                _soyad = value.Trim();
            }
        }

        public string Telefon
        {
            get { return _telefon; }
            set { _telefon = DogrulamaYardimcisi.Telefon(value); }
        }
        // E-posta zorunlu değil, boş bırakılabilir (string? → null olabilir)
        public string? Eposta
        {
            get { return _eposta; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _eposta = null;
                    return;
                }

                if (!value.Contains('@') || !value.Contains('.'))
                    throw new DogrulamaException("Geçerli bir e-posta adresi giriniz. Örnek: ornek@mail.com");

                _eposta = value.Trim();
            }
        }

        // Adres için özel bir kural yok, bu yüzden kısa yazımı kullanıyoruz
        public string? Adres { get; set; }

        // Bu sahibin hayvanları (1 sahip → N hayvan)
        // List<Hayvan>: Hayvan nesnelerinden oluşan bir liste (generic koleksiyon)
        public List<Hayvan> Hayvanlar { get; set; } = new List<Hayvan>();

        // SADECE OKUNABİLİR property: set yok, dışarıdan değiştirilemez.
        // Ad ve Soyad'dan otomatik hesaplanır. (Veritabanına kaydedilmez.)
        public string AdSoyad => $"{Ad} {Soyad}";

        // override: Üst sınıftan gelen ToString() metodunu YENİDEN tanımlıyoruz.
        // Böylece listelerde "PetAsistan.Entities.Sahip" yerine "Ahmet Yılmaz" görünür.
        public override string ToString() => AdSoyad;
    }
}