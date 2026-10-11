namespace PetAsistan.Entities
{
    // Hayvanın cinsiyeti (sadece bu iki değer seçilebilir)
    public enum Cinsiyet
    {
        Disi = 0,
        Erkek = 1
    }

    // Köpeklerin boyutu (insan yaşı hesabını etkiliyor)
    public enum KopekBoyutu
    {
        Kucuk = 0,
        Orta = 1,
        Buyuk = 2
    }

    // Aşının hangi tür hayvanlar için olduğu
    public enum AsiHedefTuru
    {
        Hepsi = 0,
        Kedi = 1,
        Kopek = 2,
        Kus = 3
    }
}