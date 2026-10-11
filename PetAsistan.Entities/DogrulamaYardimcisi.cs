namespace PetAsistan.Entities
{
    // Tekrar eden doğrulama kodlarını tek yerde toplayan yardımcı sınıf.
    // static: Nesnesi üretilmez, doğrudan "DogrulamaYardimcisi.Telefon(...)" şeklinde kullanılır.
    public static class DogrulamaYardimcisi
    {
        // Zorunlu metin alanlarını kontrol eder ve temizlenmiş halini döndürür
        public static string ZorunluMetin(string? deger, string alanAdi, int maksimumUzunluk)
        {
            if (string.IsNullOrWhiteSpace(deger))
                throw new DogrulamaException($"{alanAdi} boş bırakılamaz.");

            string temiz = deger.Trim();

            if (temiz.Length > maksimumUzunluk)
                throw new DogrulamaException($"{alanAdi} en fazla {maksimumUzunluk} karakter olabilir.");

            return temiz;
        }

        // Telefon numarasını kontrol eder ve boşluksuz/tiresiz halini döndürür
        public static string Telefon(string? deger)
        {
            if (string.IsNullOrWhiteSpace(deger))
                throw new DogrulamaException("Telefon numarası boş bırakılamaz.");

            string temiz = deger.Replace(" ", "").Replace("-", "");

            if (temiz.Length < 10 || temiz.Length > 11 || !temiz.All(char.IsDigit))
                throw new DogrulamaException(
                    "Telefon numarası 10-11 haneli olmalı ve sadece rakam içermelidir. Örnek: 05321234567");

            return temiz;
        }
    }
}