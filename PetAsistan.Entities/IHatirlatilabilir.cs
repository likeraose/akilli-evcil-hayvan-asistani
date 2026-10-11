namespace PetAsistan.Entities
{
    // SÖZLEŞME: Bu interface'i uygulayan her sınıf,
    // aşağıdaki iki üyeyi kendi içinde tanımlamak ZORUNDADIR.
    // İsimlendirme alışkanlığı: Interface adları "I" harfiyle başlar.
    public interface IHatirlatilabilir
    {
        // Ne zaman hatırlatılacak? (null ise hatırlatma yok)
        DateTime? HatirlatmaTarihi { get; }

        // Kullanıcıya gösterilecek hatırlatma cümlesi
        string HatirlatmaMetni();
    }
}