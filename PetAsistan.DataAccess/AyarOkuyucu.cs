using Microsoft.Extensions.Configuration;

namespace PetAsistan.DataAccess
{
    // appsettings.json dosyasından ayarları okuyan yardımcı sınıf
    public static class AyarOkuyucu
    {
        private const string DosyaAdi = "appsettings.json";

        // Ayar dosyasını bulup okur
        public static IConfiguration Oku()
        {
            // Önce programın çalıştığı klasöre (bin) bak, yoksa o anki çalışma klasörüne bak.
            // (Uygulama çalışırken ilki, migration komutlarında ikincisi işe yarar.)
            string klasor = File.Exists(Path.Combine(AppContext.BaseDirectory, DosyaAdi))
                ? AppContext.BaseDirectory
                : Directory.GetCurrentDirectory();

            return new ConfigurationBuilder()
                .SetBasePath(klasor)
                .AddJsonFile(DosyaAdi, optional: false)
                .Build();
        }

        // Veritabanı bağlantı dizesini döndürür
        public static string BaglantiDizesi()
        {
            string? baglanti = Oku().GetConnectionString("PetAsistanDb");

            if (string.IsNullOrWhiteSpace(baglanti))
                throw new InvalidOperationException(
                    "appsettings.json dosyasında 'PetAsistanDb' bağlantı dizesi bulunamadı.");

            return baglanti;
        }

        // Gemini API anahtarı (yoksa boş metin döner, kontrolü GeminiServisi yapar)
        public static string GeminiApiAnahtari()
        {
            return Oku()["Gemini:ApiKey"] ?? string.Empty;
        }

        // Gemini model adı (yazılmamışsa varsayılan model)
        public static string GeminiModeli()
        {
            return Oku()["Gemini:Model"] ?? "gemini-3.5-flash-lite";
        }
    }
}