namespace PetAsistan.Entities
{
    public class Kopek : Hayvan
    {
        // Sadece köpeklere özgü özellik (enum)
        public KopekBoyutu Boyut { get; set; } = KopekBoyutu.Orta;

        public override string TurAdi => "Köpek";

        public override decimal MaksimumKilo => 100m;

        // Köpek yaşı → insan yaşı
        // İlk 2 yıl kediyle aynı (15 + 9), sonrası BOYUTA göre değişir
        public override double InsanYasinaCevir()
        {
            if (Yas <= 1)
                return Math.Round(Yas * 15, 1);

            if (Yas <= 2)
                return Math.Round(15 + (Yas - 1) * 9, 1);

            // switch ifadesi: Boyut'a göre yıllık artışı seç
            int yillikArtis = Boyut switch
            {
                KopekBoyutu.Kucuk => 4,
                KopekBoyutu.Orta => 5,
                KopekBoyutu.Buyuk => 7,   // Büyük köpekler daha hızlı yaşlanır
                _ => 5                    // _ : "hiçbiri değilse" demek
            };

            return Math.Round(24 + (Yas - 2) * yillikArtis, 1);
        }

        public override int GunlukKaloriIhtiyaci()
        {
            double rer = 70 * Math.Pow((double)Kilo, 0.75);
            double carpan;

            if (Yas < 1)
                carpan = 3.0;       // Yavru köpek
            else if (KisirMi)
                carpan = 1.6;
            else
                carpan = 1.8;

            return (int)Math.Round(rer * carpan);
        }

        public override string BakimTavsiyesi()
        {
            string tavsiye = "Her gün en az iki kez yürüyüşe çıkarın ve tırnaklarını düzenli kontrol edin. ";

            if (Boyut == KopekBoyutu.Buyuk)
                tavsiye += "Büyük ırk olduğu için eklem sağlığına ve kilosuna ayrıca dikkat edin. ";

            return tavsiye + base.BakimTavsiyesi();
        }
    }
}