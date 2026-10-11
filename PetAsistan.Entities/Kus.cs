namespace PetAsistan.Entities
{
    public class Kus : Hayvan
    {
        public override string TurAdi => "Kuş";

        public override decimal MaksimumKilo => 2m;

        // Küçük kafes kuşları için yaklaşık hesap: 1 kuş yılı ≈ 8 insan yılı
        public override double InsanYasinaCevir()
        {
            return Math.Round(Yas * 8, 1);
        }

        // Kuşlarda bazal metabolizma: 78 × kilo^0,75, günlük ihtiyaç bunun yaklaşık 1,5 katı
        public override int GunlukKaloriIhtiyaci()
        {
            double bazal = 78 * Math.Pow((double)Kilo, 0.75);
            return (int)Math.Round(bazal * 1.5);
        }

        // DİKKAT: BakimTavsiyesi() burada override EDİLMEDİ!
        // Bu yüzden kuş için Hayvan sınıfındaki varsayılan (virtual) cevap kullanılacak.
    }
}