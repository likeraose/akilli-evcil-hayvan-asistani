namespace PetAsistan.AI
{
    // SÖZLEŞME: Her yapay zeka servisi (Gemini, Ollama, OpenAI...) bunları sağlamalı
    public interface IYapayZekaServisi
    {
        // Ekranda göstermek için: "Google Gemini (gemini-3.5-flash-lite)" gibi
        string SaglayiciAdi { get; }

        // sistemTalimati: Yapay zekanın "rolü" (örn: "Sen bir evcil hayvan asistanısın...")
        // kullaniciMesaji: Asıl soru
        // Task<string>: "İleride bir string döndürecek" (async işlem)
        Task<string> YanitAlAsync(string sistemTalimati, string kullaniciMesaji);
    }
}