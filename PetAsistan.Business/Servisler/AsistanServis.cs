using PetAsistan.AI;
using PetAsistan.DataAccess;
using PetAsistan.DataAccess.Repositories;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Yapay zeka destekli evcil hayvan asistanı
    public class AsistanServis : BaseServis
    {
        // ===== YAPAY ZEKANIN KURALLARI (sistem talimatı) =====
        private const string SistemTalimati =
@"Sen 'Akıllı Evcil Hayvan Asistanı' adlı bir masaüstü uygulamasının yardımcı asistanısın.
Evcil hayvan sahiplerine bakım, beslenme ve sağlık konularında genel bilgi verirsin.

Kurallar:
1. Her zaman Türkçe, sade ve anlaşılır cevap ver.
2. Kısa ve öz ol. Düz metin yaz; yıldız (*) veya diyez (#) gibi Markdown işaretleri kullanma. Maddeleme gerekirse satır başına tire (-) koy.
3. Kesin teşhis koyma ve ilaç dozu önerme; bunlar için veterinere yönlendir.
4. Nefes darlığı, bilinç kaybı, şiddetli kanama, nöbet, zehirlenme şüphesi gibi acil belirtilerde hemen veterinere gidilmesini söyle.
5. Sana verilen hayvan bilgilerini dikkate al, bilmediğin bilgileri uydurma.
6. Evcil hayvanlarla ilgisi olmayan sorularda, kibarca sadece evcil hayvan konularında yardımcı olabileceğini söyle.";

        // Sağlıkla ilgili cevapların sonuna KODUMUZLA eklediğimiz uyarı
        private const string VeterinerUyarisi =
            "\n\n⚠ Bu değerlendirme bilgilendirme amaçlıdır, veteriner muayenesinin yerini tutmaz.";

        private const int MaksimumMetinUzunlugu = 1500;

        private IYapayZekaServisi? _yapayZeka;
        private readonly IHayvanRepository _hayvanRepo;
        private readonly IRepository<AiSohbetKaydi> _sohbetRepo;

        public AsistanServis()
            : this(null, new HayvanRepository(), new GenericRepository<AiSohbetKaydi>())
        {
        }

        // yapayZeka null verilirse, ilk ihtiyaç anında ayar dosyasındaki Gemini kullanılır
        public AsistanServis(IYapayZekaServisi? yapayZeka,
                             IHayvanRepository hayvanRepo,
                             IRepository<AiSohbetKaydi> sohbetRepo)
        {
            _yapayZeka = yapayZeka;
            _hayvanRepo = hayvanRepo;
            _sohbetRepo = sohbetRepo;
        }

        // TEMBEL (lazy) oluşturma: Yapay zeka servisi ilk kullanıldığında oluşturulur.
        // Böylece API anahtarı eksikse hata, try/catch İÇİNDE ortaya çıkar ve anlaşılır mesaj olur.
        private IYapayZekaServisi YapayZeka
        {
            get
            {
                _yapayZeka ??= new GeminiServisi(AyarOkuyucu.GeminiApiAnahtari(), AyarOkuyucu.GeminiModeli());
                return _yapayZeka;
            }
        }

        // =====================================================
        //  4 YAPAY ZEKA ÖZELLİĞİ
        // =====================================================

        // 1) Serbest sohbet (hayvan seçilmese de çalışır)
        public Task<IslemSonucu<string>> SoruSorAsync(int? hayvanId, string soru)
        {
            return GetirAsync(async () =>
            {
                MetinKontrolEt(soru, "Lütfen bir soru yazın.");
                return await IsleAsync(AiIslemTuru.Sohbet, hayvanId, soru, soru, uyariEkle: false);
            });
        }

        // 2) Semptom ön değerlendirmesi
        public Task<IslemSonucu<string>> SemptomDegerlendirAsync(int hayvanId, string belirtiler)
        {
            return GetirAsync(async () =>
            {
                MetinKontrolEt(belirtiler, "Lütfen gözlemlediğiniz belirtileri yazın.");

                string gorev =
                    "Aşağıdaki belirtileri değerlendir.\n" +
                    "Cevabının İLK SATIRI tam olarak şu formatta olsun: 'ACİLİYET: <seviye>'. " +
                    "Seviye şunlardan biri olmalı: 'Acil - hemen veterinere gidin', 'Bugün veterinere gidin', " +
                    "'Birkaç gün içinde kontrol ettirin', 'Evde gözlem yeterli'.\n" +
                    "Ardından kesin teşhis koymadan olası genel nedenleri, evde dikkat edilmesi gerekenleri ve " +
                    "hangi belirtiler görülürse hemen veterinere gidilmesi gerektiğini yaz.\n\n" +
                    $"Belirtiler: {belirtiler}";

                return await IsleAsync(AiIslemTuru.SemptomDegerlendirme, hayvanId,
                                       $"Belirtiler: {belirtiler}", gorev, uyariEkle: true);
            });
        }

        // 3) Beslenme önerisi
        public Task<IslemSonucu<string>> BeslenmeOnerisiAsync(int hayvanId)
        {
            return GetirAsync(async () =>
            {
                string gorev =
                    "Bu hayvan için günlük beslenme önerisi hazırla: öğün sayısı, verilen kalori ihtiyacını " +
                    "temel alan yaklaşık porsiyon mantığı, su tüketimi, kesinlikle verilmemesi gereken yiyecekler " +
                    "ve kilo geçmişine göre kısa bir yorum.";

                return await IsleAsync(AiIslemTuru.BeslenmeOnerisi, hayvanId,
                                       "Beslenme önerisi istendi.", gorev, uyariEkle: false);
            });
        }

        // 4) Sağlık geçmişi özeti (veterinere götürmek için)
        public Task<IslemSonucu<string>> SaglikOzetiAsync(int hayvanId)
        {
            return GetirAsync(async () =>
            {
                string gorev =
                    "Bu hayvanın sağlık geçmişini, veteriner randevusuna götürülebilecek kısa bir özet haline getir: " +
                    "genel durum, aşı durumu (yaklaşan veya gecikmiş olanlar), devam eden tedaviler, " +
                    "son ziyaretlerin özeti ve dikkat edilmesi gereken noktalar.";

                return await IsleAsync(AiIslemTuru.SaglikOzeti, hayvanId,
                                       "Sağlık özeti istendi.", gorev, uyariEkle: true);
            });
        }

        // Geçmiş sohbetler (en yeniden eskiye, en fazla 50 kayıt)
        public IslemSonucu<List<AiSohbetKaydi>> GecmisiGetir(int? hayvanId)
        {
            return Getir(() => _sohbetRepo.Bul(s => s.HayvanId == hayvanId)
                                          .OrderByDescending(s => s.OlusturmaTarihi)
                                          .Take(50)
                                          .ToList());
        }

        // =====================================================
        //  ORTAK İŞLEYİŞ
        // =====================================================

        // Tüm özellikler bu metottan geçer: bağlam hazırla → yapay zekaya sor → kaydet → döndür
        private async Task<string> IsleAsync(AiIslemTuru islemTuru, int? hayvanId,
                                             string kayitMetni, string gorev, bool uyariEkle)
        {
            // 1) Hayvan bağlamını hazırla
            string baglam = "Belirli bir hayvan seçilmedi.";

            if (hayvanId.HasValue)
            {
                Hayvan? hayvan = _hayvanRepo.TumBilgileriyleGetir(hayvanId.Value);

                if (hayvan == null)
                    throw new DogrulamaException("Seçilen hayvan bulunamadı.");

                baglam = HayvanBaglami.Olustur(hayvan);
            }

            // 2) Mesajı birleştir (bugünün tarihi: gecikmiş aşıları anlayabilsin diye)
            string mesaj =
                $"Bugünün tarihi: {DateTime.Today:dd.MM.yyyy}\n\n" +
                $"{baglam}\n\n" +
                $"GÖREV / SORU:\n{gorev}";

            // 3) Yapay zekaya sor (await: beklerken uygulama donmaz)
            string cevap = await YapayZeka.YanitAlAsync(SistemTalimati, mesaj);

            // 4) Sağlıkla ilgili cevaplara uyarıyı KENDİMİZ ekleyelim
            if (uyariEkle)
                cevap += VeterinerUyarisi;

            // 5) Sohbet geçmişine kaydet
            _sohbetRepo.Ekle(new AiSohbetKaydi
            {
                HayvanId = hayvanId,
                IslemTuru = islemTuru,
                Soru = kayitMetni,
                Cevap = cevap
            });

            return cevap;
        }

        private static void MetinKontrolEt(string? metin, string bosMesaji)
        {
            if (string.IsNullOrWhiteSpace(metin))
                throw new DogrulamaException(bosMesaji);

            if (metin.Length > MaksimumMetinUzunlugu)
                throw new DogrulamaException($"Metin en fazla {MaksimumMetinUzunlugu} karakter olabilir.");
        }
    }
}