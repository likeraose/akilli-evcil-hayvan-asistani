namespace PetAsistan.Entities
{
    // Kullanıcının hatalı veri girdiğini belirten özel hata sınıfı.
    // ": Exception" → C#'ın hazır Exception sınıfından MİRAS alıyor.
    public class DogrulamaException : Exception
    {
        // Yapıcı metot (constructor): "new DogrulamaException(...)" denildiğinde çalışır.
        // ": base(mesaj)" → Gelen mesajı üst sınıfa (Exception) iletir.
        public DogrulamaException(string mesaj) : base(mesaj)
        {
        }
    }
}