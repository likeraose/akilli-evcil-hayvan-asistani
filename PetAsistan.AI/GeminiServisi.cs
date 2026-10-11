using System.Net;
using System.Text;
using System.Text.Json;

namespace PetAsistan.AI
{
    // IYapayZekaServisi sözleşmesinin Google Gemini ile uygulaması
    public class GeminiServisi : IYapayZekaServisi
    {
        private const string TemelAdres = "https://generativelanguage.googleapis.com/v1beta/models/";

        // HttpClient tek bir kere oluşturulup tekrar tekrar kullanılmalı (static)
        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        private readonly string _apiAnahtari;
        private readonly string _model;

        public string SaglayiciAdi => $"Google Gemini ({_model})";

        public GeminiServisi(string apiAnahtari, string model)
        {
            if (string.IsNullOrWhiteSpace(apiAnahtari) || apiAnahtari.StartsWith("BURAYA"))
                throw new YapayZekaException(
                    "Gemini API anahtarı ayarlanmamış. Lütfen appsettings.json dosyasına kendi anahtarınızı yazın.");

            if (string.IsNullOrWhiteSpace(model))
                throw new YapayZekaException("Gemini model adı ayarlanmamış.");

            _apiAnahtari = apiAnahtari.Trim();
            _model = model.Trim();
        }

        // async: Bu metot beklemeli işler içeriyor, uygulamayı dondurmadan çalışır
        public async Task<string> YanitAlAsync(string sistemTalimati, string kullaniciMesaji)
        {
            if (string.IsNullOrWhiteSpace(kullaniciMesaji))
                throw new YapayZekaException("Yapay zekaya gönderilecek mesaj boş olamaz.");

            // 1) Gemini'nin beklediği JSON yapısını hazırla (anonim nesnelerle)
            var govde = new
            {
                systemInstruction = new
                {
                    parts = new[] { new { text = sistemTalimati } }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = kullaniciMesaji } }
                    }
                }
            };

            // 2) HTTP isteğini oluştur: adres + API anahtarı başlığı + JSON gövde
            using var istek = new HttpRequestMessage(HttpMethod.Post, $"{TemelAdres}{_model}:generateContent");
            istek.Headers.Add("x-goog-api-key", _apiAnahtari);
            istek.Content = new StringContent(JsonSerializer.Serialize(govde), Encoding.UTF8, "application/json");

            // 3) Gönder ve cevabı BEKLE (await: beklerken uygulama donmaz)
            HttpResponseMessage yanit;
            try
            {
                yanit = await Http.SendAsync(istek);
            }
            catch (HttpRequestException)
            {
                throw new YapayZekaException("Yapay zeka servisine bağlanılamadı. İnternet bağlantınızı kontrol edin.");
            }
            catch (TaskCanceledException)
            {
                throw new YapayZekaException("Yapay zeka servisi zamanında yanıt vermedi. Lütfen tekrar deneyin.");
            }

            // 4) Cevabı oku
            using (yanit)
            {
                string icerik = await yanit.Content.ReadAsStringAsync();

                if (!yanit.IsSuccessStatusCode)
                    throw new YapayZekaException(HataMesajiUret(yanit.StatusCode));

                return MetniAyikla(icerik);
            }
        }

        // Gemini'nin JSON cevabının içinden asıl metni çıkarır
        private static string MetniAyikla(string json)
        {
            using JsonDocument belge = JsonDocument.Parse(json);
            JsonElement kok = belge.RootElement;

            // Cevap yapısı: { "candidates": [ { "content": { "parts": [ { "text": "..." } ] } } ] }
            if (!kok.TryGetProperty("candidates", out JsonElement adaylar) || adaylar.GetArrayLength() == 0)
                throw new YapayZekaException("Yapay zeka bu soruya yanıt üretmedi. Sorunuzu farklı şekilde sormayı deneyin.");

            StringBuilder metin = new StringBuilder();
            JsonElement ilkAday = adaylar[0];

            if (ilkAday.TryGetProperty("content", out JsonElement icerik) &&
                icerik.TryGetProperty("parts", out JsonElement parcalar))
            {
                foreach (JsonElement parca in parcalar.EnumerateArray())
                {
                    // Modelin "düşünme" parçalarını atla, sadece asıl cevabı al
                    bool dusunceMi = parca.TryGetProperty("thought", out JsonElement d) &&
                                     d.ValueKind == JsonValueKind.True;

                    if (!dusunceMi && parca.TryGetProperty("text", out JsonElement parcaMetni))
                        metin.Append(parcaMetni.GetString());
                }
            }

            if (metin.Length == 0)
                throw new YapayZekaException("Yapay zeka bu soruya yanıt veremedi. Sorunuzu farklı şekilde sormayı deneyin.");

            return metin.ToString().Trim();
        }

        // HTTP hata kodlarını anlaşılır Türkçe mesajlara çevirir
        private string HataMesajiUret(HttpStatusCode kod)
        {
            return kod switch
            {
                HttpStatusCode.BadRequest =>
                    "Yapay zeka isteği geçersiz. API anahtarınızı ve model adını kontrol edin.",
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                    "Gemini API anahtarı geçersiz veya bu işlem için yetkisi yok.",
                HttpStatusCode.NotFound =>
                    $"'{_model}' modeli bulunamadı. appsettings.json içindeki model adını kontrol edin.",
                HttpStatusCode.TooManyRequests =>
                    "Ücretsiz kullanım sınırına ulaşıldı. Lütfen biraz bekleyip tekrar deneyin.",
                _ when (int)kod >= 500 =>
                    "Yapay zeka servisinde geçici bir sorun var. Lütfen daha sonra tekrar deneyin.",
                _ =>
                    $"Yapay zeka servisi bir hata döndürdü (kod: {(int)kod})."
            };
        }
    }
}