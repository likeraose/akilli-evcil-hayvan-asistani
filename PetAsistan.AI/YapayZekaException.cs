namespace PetAsistan.AI
{
    // Yapay zeka servisiyle ilgili sorunlarda fırlatılan, kullanıcıya gösterilebilir hata
    public class YapayZekaException : Exception
    {
        public YapayZekaException(string mesaj) : base(mesaj)
        {
        }
    }
}