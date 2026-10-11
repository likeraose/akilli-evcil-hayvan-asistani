namespace PetAsistan.Entities
{
    // ": Hayvan" → Hayvan'ın tüm özelliklerini (Ad, Kilo, DogumTarihi...) miras alır
    public class Kedi : Hayvan
    {
        // Sadece kedilere özgü özellik
        public bool EvIciMi { get; set; } = true;

        // ===== Hayvan'daki abstract üyeleri DOLDURUYORUZ (override) =====

        public override string TurAdi => "Kedi";

        public override decimal MaksimumKilo => 15m;

        // Kedi yaşı → insan yaşı
        // İlk yıl = 15 insan yılı, ikinci yıl = +9, sonraki her yıl = +4
        public override double InsanYasinaCevir()
        {
            double insanYasi;

            if (Yas <= 1)
                insanYasi = Yas * 15;
            else if (Yas <= 2)
                insanYasi = 15 + (Yas - 1) * 9;
            else
                insanYasi = 24 + (Yas - 2) * 4;

            return Math.Round(insanYasi, 1);
        }

        // Günlük kalori = RER × çarpan
        public override int GunlukKaloriIhtiyaci()
        {
            double rer = 70 * Math.Pow((double)Kilo, 0.75);
            double carpan;

            if (Yas < 1)
                carpan = 2.5;       // Yavru kedi: büyüdüğü için çok enerji harcar
            else if (KisirMi)
                carpan = 1.2;       // Kısır yetişkin
            else
                carpan = 1.4;       // Kısır olmayan yetişkin

            if (!EvIciMi)
                carpan += 0.2;      // Dışarı çıkan kedi daha hareketlidir

            return (int)Math.Round(rer * carpan);
        }

        // virtual olan metodu değiştiriyoruz.
        // base.BakimTavsiyesi() → Hayvan sınıfındaki ORİJİNAL cevabı da sona ekliyoruz.
        public override string BakimTavsiyesi()
        {
            return "Kumunu her gün temizleyin, bol su içmesini sağlayın ve tüylerini düzenli tarayın. "
                   + base.BakimTavsiyesi();
        }
    }
}