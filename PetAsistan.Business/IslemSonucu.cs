namespace PetAsistan.Business
{
    // Bir işlemin sonucunu taşıyan "zarf" (ekleme, güncelleme, silme için)
    public class IslemSonucu
    {
        // protected set: Dışarıdan DEĞİŞTİRİLEMEZ, sadece bu sınıf ve alt sınıfları atayabilir
        public bool BasariliMi { get; protected set; }
        public string Mesaj { get; protected set; } = string.Empty;

        // static "fabrika" metotları: Nesneyi okunaklı bir şekilde üretir
        // Kullanım: return IslemSonucu.Basarili("Kayıt eklendi.");
        public static IslemSonucu Basarili(string mesaj)
        {
            return new IslemSonucu { BasariliMi = true, Mesaj = mesaj };
        }

        public static IslemSonucu Hatali(string mesaj)
        {
            return new IslemSonucu { BasariliMi = false, Mesaj = mesaj };
        }
    }

    // VERİ de taşıyan sonuç (listeleme / getirme işlemleri için)
    // ": IslemSonucu" → BasariliMi ve Mesaj özellikleri MİRAS olarak geliyor, üstüne Veri ekleniyor
    public class IslemSonucu<T> : IslemSonucu
    {
        public T? Veri { get; private set; }

        public static IslemSonucu<T> Basarili(T veri)
        {
            return new IslemSonucu<T> { BasariliMi = true, Veri = veri };
        }

        // new: Üst sınıftaki aynı isimli Hatali() metodunu GİZLER,
        // çünkü bu sefer IslemSonucu<T> türünde sonuç döndürmemiz gerekiyor
        public static new IslemSonucu<T> Hatali(string mesaj)
        {
            return new IslemSonucu<T> { BasariliMi = false, Mesaj = mesaj };
        }
    }
}